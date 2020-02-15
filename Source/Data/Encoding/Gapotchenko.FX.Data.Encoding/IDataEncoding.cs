﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines interface of a data encoding.
    /// </summary>
    public interface IDataEncoding
    {
        /// <summary>
        /// Gets the encoding name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the average encoding efficiency.
        /// The efficiency is the ratio between number of bits in the input and the number of bits in the encoded output.
        /// </summary>
        float Efficiency { get; }

        /// <summary>
        /// Gets the maximum encoding efficiency.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        float MaxEfficiency { get; }

        /// <summary>
        /// Gets the minimum encoding efficiency.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        float MinEfficiency { get; }

        /// <summary>
        /// Encodes the data.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <returns>The encoded output data.</returns>
        byte[] EncodeData(ReadOnlySpan<byte> data);

        /// <summary>
        /// Decodes the data.
        /// </summary>
        /// <param name="data">The encoded input data.</param>
        /// <returns>The decoded output data.</returns>
        byte[] DecodeData(ReadOnlySpan<byte> data);
    }
}