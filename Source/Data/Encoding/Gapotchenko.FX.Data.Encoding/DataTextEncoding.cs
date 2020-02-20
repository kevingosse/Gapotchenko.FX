﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    using Encoding = System.Text.Encoding;

    /// <summary>
    /// The base class for <see cref="IDataTextEncoding"/> implementations.
    /// </summary>
    public abstract class DataTextEncoding : DataEncoding, IDataTextEncoding
    {
        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data) => GetString(data, DataEncodingOptions.None);

        /// <inheritdoc/>
        public string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options) => data == null ? null : GetStringCore(data, options);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        protected virtual string GetStringCore(ReadOnlySpan<byte> data, DataEncodingOptions options)
        {
            var sw = new StringWriter();

            var context = CreateEncoderContext(options);
            context.Encode(data, sw);
            context.Encode(null, sw);

            return sw.ToString();
        }

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s) => GetBytes(s, DataEncodingOptions.None);

        /// <inheritdoc/>
        public byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options) => s == null ? null : GetBytesCore(s, options);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        protected virtual byte[] GetBytesCore(ReadOnlySpan<char> s, DataEncodingOptions options)
        {
            var ms = new MemoryStream();

            var context = CreateDecoderContext(options);
            context.Decode(s, ms);
            context.Decode(null, ms);

            return ms.ToArray();
        }

        /// <inheritdoc/>
        public abstract bool IsCaseSensitive { get; }

        /// <inheritdoc/>
        public int Padding => PaddingCore;

        /// <summary>
        /// Gets the number of characters used for padding of encoded string representation.
        /// </summary>
        protected virtual int PaddingCore => 1;

        /// <inheritdoc/>
        public string Pad(ReadOnlySpan<char> s) => s == null ? null : PadCore(s);

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        protected virtual string PadCore(ReadOnlySpan<char> s) => s.ToString();

        /// <inheritdoc/>
        public ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s) => s == null ? null : UnpadCore(s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected virtual ReadOnlySpan<char> UnpadCore(ReadOnlySpan<char> s) => s;

        /// <inheritdoc/>
        public bool IsPadded(ReadOnlySpan<char> s) => s.Length % Padding == 0;

        /// <inheritdoc/>
        protected override byte[] EncodeDataCore(ReadOnlySpan<byte> data) => Encoding.ASCII.GetBytes(GetString(data));

        /// <inheritdoc/>
        protected override byte[] DecodeDataCore(ReadOnlySpan<byte> data) =>
            GetBytes(
                Encoding.ASCII.GetString(
#if TFF_MEMORY && !TFF_MEMORY_OOB
                    data
#else
                    data.ToArray()
#endif
                )
                .AsSpan());

        /// <summary>
        /// The encoder context.
        /// </summary>
        protected interface IEncoderContext
        {
            /// <summary>
            /// Encodes a block of data.
            /// </summary>
            /// <param name="input">
            /// The input.
            /// The <c>null</c> value signals a final block.
            /// </param>
            /// <param name="output">The output.</param>
            void Encode(ReadOnlySpan<byte> input, TextWriter output);
        }

        /// <summary>
        /// Creates encoder context.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The encoder context.</returns>
        protected abstract IEncoderContext CreateEncoderContext(DataEncodingOptions options);

        /// <summary>
        /// The decoder context.
        /// </summary>
        protected interface IDecoderContext
        {
            /// <summary>
            /// Decodes a block of data.
            /// </summary>
            /// <param name="input">
            /// The input.
            /// The <c>null</c> value signals a final block.
            /// </param>
            /// <param name="output">The output.</param>
            void Decode(ReadOnlySpan<char> input, Stream output);
        }

        /// <summary>
        /// Creates decoder context.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The decoder context.</returns>
        protected abstract IDecoderContext CreateDecoderContext(DataEncodingOptions options);

        /// <inheritdoc/>
        public Stream CreateEncoder(TextWriter textWriter, DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (textWriter == null)
                throw new ArgumentNullException(nameof(textWriter));

            var context = CreateEncoderContext(options);
            return new EncoderStream(textWriter, context, options, 1024);
        }

        /// <inheritdoc/>
        public Stream CreateDecoder(TextReader textReader, DataEncodingOptions options = DataEncodingOptions.None)
        {
            throw new NotImplementedException();
        }

        sealed class EncoderStream : Stream
        {
            public EncoderStream(TextWriter textWriter, IEncoderContext context, DataEncodingOptions options, int bufferSize)
            {
                m_TextWriter = textWriter;
                m_Context = context;
                m_BufferSize = bufferSize;

                m_OwnsTextWriter = (options & DataEncodingOptions.NoOwnership) == 0;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (m_OwnsTextWriter)
                        m_TextWriter.Dispose();
                }
            }

            TextWriter m_TextWriter;
            IEncoderContext m_Context;
            bool m_OwnsTextWriter;

            StringBuilder m_Buffer;
            StringWriter m_BufferWriter;
            int m_BufferSize;

            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => true;

            public override long Length => throw new NotSupportedException();

            public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

            public override void SetLength(long value) => throw new NotSupportedException();

            public override void Close()
            {
                FlushFinalBlock();
                base.Close();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                m_Context.Encode(new ReadOnlySpan<byte>(buffer, offset, count), m_TextWriter);
            }

            public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                if (count == 0)
                    return;

                var input = new ReadOnlyMemory<byte>(buffer, offset, count);

                EnsureBufferCreated();
                foreach (var chunk in SplitMemoryIntoChunks(input, m_BufferSize))
                {
                    m_Context.Encode(chunk.Span, m_BufferWriter);
                    await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            static IEnumerable<ReadOnlyMemory<T>> SplitMemoryIntoChunks<T>(ReadOnlyMemory<T> memory, int chunkSize)
            {
                int memoryLength = memory.Length;
                if (memoryLength <= chunkSize)
                {
                    yield return memory;
                    yield break;
                }

                int chunkOffset = 0;
                while (chunkOffset < memoryLength)
                {
                    int chunkLength = Math.Min(chunkSize, memoryLength - chunkOffset);
                    yield return memory.Slice(chunkOffset, chunkLength);
                    chunkOffset += chunkLength;
                }
            }

            public override void Flush()
            {
                FlushFinalBlock();
                m_TextWriter.Flush();
            }

            void FlushFinalBlock()
            {
                FlushBuffer();
                m_Context.Encode(null, m_TextWriter);
            }

            async Task FlushFinalBlockAsync(CancellationToken cancellationToken = default)
            {
                EnsureBufferCreated();
                m_Context.Encode(null, m_BufferWriter);
                await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
            }

            void EnsureBufferCreated()
            {
                if (m_Buffer != null)
                    return;

                m_Buffer = new StringBuilder();
                m_BufferWriter = new StringWriter(m_Buffer)
                {
                    NewLine = m_TextWriter.NewLine
                };
            }

            void FlushBuffer()
            {
                if (m_Buffer == null)
                    return;

                m_BufferWriter.Flush();
                if (m_Buffer.Length != 0)
                {
#if TFF_TEXT_IO_STRINGBUILDER
                    m_TextWriter.Write(m_Buffer);
#else
                    m_TextWriter.Write(m_Buffer.ToString());
#endif
                    m_Buffer.Clear();
                }
            }

            async Task FlushBufferAsync(CancellationToken cancellationToken)
            {
                if (m_Buffer == null)
                    return;

                m_BufferWriter.Flush();
                if (m_Buffer.Length != 0)
                {
#if TFF_TEXT_IO_STRINGBUILDER
                    await m_TextWriter.WriteAsync(m_Buffer, cancellationToken).ConfigureAwait(false);
#elif TFF_TEXT_IO_CANCELLATION
                    await m_TextWriter.WriteAsync(m_Buffer.ToString().AsMemory(), cancellationToken).ConfigureAwait(false);
#else
                    await m_TextWriter.WriteAsync(m_Buffer.ToString()).ConfigureAwait(false);
#endif
                    m_Buffer.Clear();
                }
            }
        }

        #region Implementation Helpers

        string Pad(ReadOnlySpan<char> s, char paddingChar, bool right)
        {
            if (s == null)
                return null;

            int padding = Padding;

            int width =
                padding < 2 ?
                    0 :
                    (s.Length + padding - 1) / padding * padding;

            string output = s.ToString();

            if (width == 0)
                return output;
            else if (right)
                return output.PadRight(width, paddingChar);
            else
                return output.PadLeft(width, paddingChar);
        }

        /// <summary>
        /// Pads the encoded string to the right.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadRight(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, true);

        /// <summary>
        /// Pads the encoded string to the left.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The padded encoded string.</returns>
        protected string PadLeft(ReadOnlySpan<char> s, char paddingChar) => Pad(s, paddingChar, false);

        /// <summary>
        /// Unpads the encoded string from the right side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadRight(ReadOnlySpan<char> s, char paddingChar) => s.TrimEnd(paddingChar);

        /// <summary>
        /// Unpads the encoded string from the left side.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <param name="paddingChar">The padding character.</param>
        /// <returns>The unpadded encoded string.</returns>
        protected ReadOnlySpan<char> UnpadLeft(ReadOnlySpan<char> s, char paddingChar) => s.TrimStart(paddingChar);

        #endregion
    }
}
