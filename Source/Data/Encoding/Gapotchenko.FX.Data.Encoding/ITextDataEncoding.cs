﻿using System;
using System.IO;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines the interface of a binary-to-text encoding.
    /// </summary>
    public interface ITextDataEncoding : IDataEncoding
    {
        /// <summary>
        /// Gets a value indicating whether encoding is case-sensitive.
        /// </summary>
        bool IsCaseSensitive { get; }

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        string GetString(ReadOnlySpan<byte> data);

        /// <summary>
        /// Encodes an array of bytes to its equivalent string representation with specified options.
        /// </summary>
        /// <param name="data">The input array of bytes.</param>
        /// <param name="options">The options.</param>
        /// <returns>The string representation of the contents of <paramref name="data"/>.</returns>
        string GetString(ReadOnlySpan<byte> data, DataEncodingOptions options);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        byte[] GetBytes(ReadOnlySpan<char> s);

        /// <summary>
        /// Decodes the specified string to an equivalent array of bytes with specified options.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <param name="options">The options.</param>
        /// <returns>An array of bytes that is equivalent to <paramref name="s"/>.</returns>
        byte[] GetBytes(ReadOnlySpan<char> s, DataEncodingOptions options);

        /// <summary>
        /// Creates a streaming encoder.
        /// </summary>
        /// <param name="textWriter">The output text writer of an encoder.</param>
        /// <param name="options">The options.</param>
        /// <returns>The stream.</returns>
        Stream CreateEncoder(TextWriter textWriter, DataEncodingOptions options = DataEncodingOptions.None);

        /// <summary>
        /// Creates a streaming decoder.
        /// </summary>
        /// <param name="textReader">The input text reader of a decoder.</param>
        /// <param name="options">The options.</param>
        /// <returns>The stream.</returns>
        Stream CreateDecoder(TextReader textReader, DataEncodingOptions options = DataEncodingOptions.None);

        /// <summary>
        /// Pads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to pad.</param>
        /// <returns>The padded encoded string.</returns>
        string Pad(ReadOnlySpan<char> s);

        /// <summary>
        /// Unpads the encoded string.
        /// </summary>
        /// <param name="s">The encoded string to unpad.</param>
        /// <returns>The unpadded encoded string.</returns>
        ReadOnlySpan<char> Unpad(ReadOnlySpan<char> s);

        /// <summary>
        /// Gets a value indicating whether the specified encoded string is padded.
        /// </summary>
        /// <param name="s">The encoded string.</param>
        /// <returns><c>true</c> when specified encoded string is padded; otherwise, <c>false</c>.</returns>
        bool IsPadded(ReadOnlySpan<char> s);
    }
}