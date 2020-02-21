﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides a generic implementation of Base64 encoding.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class GenericBase64 : DataTextEncoding, IBase64
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericBase64"/> class with the specified alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        protected GenericBase64(DataTextEncodingAlphabet alphabet)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));

            if (alphabet.Size != Radix)
            {
                throw new ArgumentException(
                    string.Format("The alphabet size of {0} encoding should be {1}.", Name, Radix),
                    nameof(alphabet));
            }

            Alphabet = alphabet;
        }

        /// <summary>
        /// The encoding alphabet.
        /// </summary>
        protected readonly DataTextEncodingAlphabet Alphabet;

        /// <inheritdoc/>
        public int Radix => 64;

        /// <summary>
        /// Base64 encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        public new const float Efficiency = 0.75f;

        /// <inheritdoc/>
        protected override float EfficiencyCore => Efficiency;

        abstract class CodecContextBase
        {
            public CodecContextBase(DataTextEncodingAlphabet alphabet, DataEncodingOptions options)
            {
                m_Alphabet = alphabet;
                m_Options = options;
            }

            protected readonly DataTextEncodingAlphabet m_Alphabet;
            protected readonly DataEncodingOptions m_Options;

            protected int m_Bits;
            protected int m_Modulus;
            protected bool m_Eof;

            protected const int Mask6Bits = 0x3f;
            protected const int Mask4Bits = 0x0f;
            protected const int Mask2Bits = 0x03;
        }

        sealed class EncoderContext : CodecContextBase, IEncoderContext
        {
            public EncoderContext(DataTextEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            int m_LinePosition;

            void IncrementLinePosition(int delta)
            {
                m_LinePosition += delta;
            }

            void InsertLineBreak(TextWriter output)
            {
                if (m_LinePosition >= 76)
                {
                    m_LinePosition = 0;

                    if ((m_Options & DataEncodingOptions.Indent) != 0)
                        output.WriteLine();
                }
            }

            public void Encode(ReadOnlySpan<byte> input, TextWriter output)
            {
                if (m_Eof)
                    return;

                var alphabet = m_Alphabet;

                if (input == null)
                {
                    m_Eof = true;
                    switch (m_Modulus)
                    {
                        case 0:
                            // Nothing to do.
                            break;

                        case 1:
                            InsertLineBreak(output);

                            // 8 bits = 6 + 2
                            output.Write(alphabet[(m_Bits >> 2) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 4) & Mask6Bits]); // 2 bits

                            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
                            {
                                output.Write(PaddingChar);
                                output.Write(PaddingChar);
                            }
                            break;

                        case 2:
                            InsertLineBreak(output);

                            // 16 bits = 6 + 6 + 4
                            output.Write(alphabet[(m_Bits >> 10) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits >> 4) & Mask6Bits]); // 6 bits
                            output.Write(alphabet[(m_Bits << 2) & Mask6Bits]); // 4 bits

                            if ((m_Options & DataEncodingOptions.NoPadding) == 0)
                                output.Write(PaddingChar);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
                else
                {
                    foreach (var b in input)
                    {
                        // Accumulate data bits.
                        m_Bits = (m_Bits << 8) | b;

                        if (++m_Modulus == 3)
                        {
                            m_Modulus = 0;

                            InsertLineBreak(output);

                            // 3 bytes = 24 bits = 4 * 6 bits
                            output.Write(alphabet[(m_Bits >> 18) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 12) & Mask6Bits]);
                            output.Write(alphabet[(m_Bits >> 6) & Mask6Bits]);
                            output.Write(alphabet[m_Bits & Mask6Bits]);

                            IncrementLinePosition(4);
                        }
                    }
                }
            }
        }

        sealed class DecoderContext : CodecContextBase, IDecoderContext
        {
            public DecoderContext(DataTextEncodingAlphabet alphabet, DataEncodingOptions options) :
                base(alphabet, options)
            {
            }

            public void Decode(ReadOnlySpan<char> input, Stream output)
            {
                if (m_Eof)
                    return;

                if (input == null)
                    m_Eof = true;

                var alphabet = m_Alphabet;

                foreach (var c in input)
                {
                    if (c == PaddingChar)
                    {
                        FlushDecode(output);
                        continue;
                    }

                    int b = alphabet.IndexOf(c);
                    if (b == -1)
                    {
                        if ((m_Options & DataEncodingOptions.Relaxed) == 0)
                        {
                            if (!char.IsWhiteSpace(c))
                                throw new InvalidDataException("Encountered a non-Base64 character.");
                        }
                        continue;
                    }

                    // Accumulate data bits.
                    m_Bits = (m_Bits << 6) | b;

                    if (++m_Modulus == 4)
                    {
                        m_Modulus = 0;

                        output.WriteByte((byte)(m_Bits >> 16));
                        output.WriteByte((byte)(m_Bits >> 8));
                        output.WriteByte((byte)m_Bits);
                    }
                }

                if (m_Eof)
                {
                    if ((m_Options & DataEncodingOptions.RequirePadding) != 0)
                    {
                        if (m_Modulus != 0)
                            throw new InvalidDataException("Invalid Base64 padding.");
                    }

                    FlushDecode(output);
                }
            }

            void FlushDecode(Stream output)
            {
                if (m_Modulus == 0)
                    return;

                switch (m_Modulus)
                {
                    case 1:
                        // 6 bits
                        ValidateIncompleteByte();
                        break;

                    case 2:
                        // 2 * 6 bits = 12 = 8 + 4
                        ValidateLastSymbol(Mask4Bits);
                        output.WriteByte((byte)(m_Bits >> 4));
                        break;

                    case 3:
                        // 3 * 6 bits = 18 = 8 + 8 + 2
                        ValidateLastSymbol(Mask2Bits);
                        output.WriteByte((byte)(m_Bits >> 10));
                        output.WriteByte((byte)(m_Bits >> 2));
                        break;

                    default:
                        throw new InvalidOperationException();
                }

                m_Modulus = 0;
            }

            void ValidateIncompleteByte()
            {
                if ((m_Options & DataEncodingOptions.Relaxed) == 0)
                    throw new InvalidDataException("Cannot decode the last byte due to missing Base64 symbol.");
            }

            void ValidateLastSymbol(int zeroMask)
            {
                if ((m_Options & DataEncodingOptions.Relaxed) == 0 && (m_Bits & zeroMask) != 0)
                    throw new InvalidDataException("The insignificant bits of the last Base64 symbol are expected to be zero.");
            }
        }

        /// <inheritdoc/>
        protected override IEncoderContext CreateEncoderContext(DataEncodingOptions options) => new EncoderContext(Alphabet, options);

        /// <inheritdoc/>
        protected override IDecoderContext CreateDecoderContext(DataEncodingOptions options) => new DecoderContext(Alphabet, options);

        /// <inheritdoc/>
        public override bool IsCaseSensitive => true;

        /// <inheritdoc/>
        protected override int PaddingCore => 4;

        /// <summary>
        /// The padding character.
        /// </summary>
        protected const char PaddingChar = '=';

        /// <inheritdoc/>
        protected override string PadCore(ReadOnlySpan<char> s) => PadRight(s, PaddingChar);

        /// <inheritdoc/>
        protected override ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => UnpadRight(s, PaddingChar);
    }
}
