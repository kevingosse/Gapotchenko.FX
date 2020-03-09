﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Gapotchenko.FX.Data.Encoding.Test.Bench
{
    using Encoding = System.Text.Encoding;

    public static class TextDataEncodingTestBench
    {
        #region Data

        public static void TestVector(
            ITextDataEncoding dataEncoding,
            string raw,
            string encoded,
            DataEncodingOptions options = DataEncodingOptions.None,
            Encoding textEncoding = null)
        {
            if (dataEncoding == null)
                throw new ArgumentNullException(nameof(dataEncoding));
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            // -----------------------------------------------------------------
            // Parameters normalization
            // -----------------------------------------------------------------

            if (textEncoding == null)
                textEncoding = Encoding.UTF8;

            // -----------------------------------------------------------------
            // Data preparation
            // -----------------------------------------------------------------

            var rawBytes = textEncoding.GetBytes(raw);

            // -----------------------------------------------------------------

            TestVector(dataEncoding, rawBytes, encoded, options, textEncoding);
        }

        public static void TestVector(
            ITextDataEncoding dataEncoding,
            ReadOnlySpan<byte> raw,
            string encoded,
            DataEncodingOptions options = DataEncodingOptions.None,
            Encoding textEncoding = null)
        {
            if (dataEncoding == null)
                throw new ArgumentNullException(nameof(dataEncoding));
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            // -----------------------------------------------------------------
            // Parameters normalization
            // -----------------------------------------------------------------

            if (textEncoding == null)
                textEncoding = Encoding.UTF8;

            // -----------------------------------------------------------------
            // Data preparation
            // -----------------------------------------------------------------

            var encodedBytes = textEncoding.GetBytes(encoded);

            // -----------------------------------------------------------------
            // Check text-based data encoding API
            // -----------------------------------------------------------------

            string actualEncoded = dataEncoding.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded, "Encoding error.");

            var actualDecoded = dataEncoding.GetBytes(actualEncoded.AsSpan(), options);
            if (!raw.SequenceEqual(actualDecoded))
            {
                Assert.AreEqual(
                    Base16.GetString(raw, DataEncodingOptions.Indent),
                    Base16.GetString(actualDecoded, DataEncodingOptions.Indent),
                    "Decoding error.");
            }

            // -----------------------------------------------------------------
            // Check padding operations
            // -----------------------------------------------------------------

            var actualEncodedUnpadded = dataEncoding.Unpad(actualEncoded.AsSpan()).ToString();
            string actualEncodedRepadded = dataEncoding.Pad(actualEncodedUnpadded.AsSpan());

            bool prefersPadding = dataEncoding.PrefersPadding;

            if (prefersPadding)
                Assert.AreEqual(actualEncoded, actualEncodedRepadded);
            Assert.IsTrue(actualEncodedRepadded.Length % dataEncoding.Padding == 0);

            if (!dataEncoding.CanPad)
            {
                Assert.AreEqual(actualEncodedUnpadded, actualEncoded);
                Assert.AreEqual(actualEncodedUnpadded, actualEncodedRepadded);
            }

            string actualEncodedOverpadded = dataEncoding.Pad(actualEncoded.AsSpan());
            if (prefersPadding)
                Assert.AreEqual(actualEncoded, actualEncodedOverpadded);

            string actualEncodedUnderpadded = dataEncoding.Unpad(actualEncodedUnpadded.AsSpan()).ToString();
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedUnderpadded);

            Assert.IsTrue(raw.SequenceEqual(dataEncoding.GetBytes(actualEncodedUnpadded.AsSpan(), options)), "Cannot decode unpadded string.");

            string actualEncodedWithoutPadding = dataEncoding.GetString(raw, options | DataEncodingOptions.Unpad);
            Assert.AreEqual(actualEncodedUnpadded, actualEncodedWithoutPadding, "DataTextEncodingOptions.NoPadding is not honored.");

            // -----------------------------------------------------------------
            // Check the general data encoding API
            // -----------------------------------------------------------------

            var actualEncodedBytes = dataEncoding.EncodeData(raw, options);
            Assert.IsTrue(encodedBytes.SequenceEqual(actualEncodedBytes));

            var actualDecodedBytes = dataEncoding.DecodeData(actualEncodedBytes, options);
            Assert.IsTrue(raw.SequenceEqual(actualDecodedBytes));

            // -----------------------------------------------------------------
            // Check maximum count calculations
            // -----------------------------------------------------------------

            int maxCharCount = dataEncoding.GetMaxCharCount(raw.Length, options);
            Assert.IsTrue(actualEncoded.Length <= maxCharCount, "GetMaxCharCount returned an invalid value.");

            int maxByteCount = dataEncoding.GetMaxByteCount(actualEncoded.Length, options);
            Assert.IsTrue(actualDecoded.Length <= maxByteCount, "GetMaxByteCount returned an invalid value.");

            // -----------------------------------------------------------------
            // Check actual encoding efficiency and boundaries
            // -----------------------------------------------------------------

            int actualEncodedBytesCount = textEncoding.GetByteCount(actualEncodedUnpadded);
            if (actualEncodedBytesCount > 0)
            {
                if ((options & DataEncodingOptions.Compress) == 0)
                {
                    float actualEfficiencyCeiling = (float)raw.Length / actualEncodedBytesCount;
                    Assert.IsTrue(actualEfficiencyCeiling <= dataEncoding.MaxEfficiency, "Max encoding efficiency violated.");
                }

                if (actualEncodedBytesCount > 1)
                {
                    int rawBytesCount = raw.Length;

                    if ((options & DataEncodingOptions.Checksum) != 0)
                        rawBytesCount += 4;

                    float actualEfficiencyFloor = (float)rawBytesCount / (actualEncodedBytesCount - 1);
                    Assert.IsTrue(actualEfficiencyFloor >= dataEncoding.MinEfficiency, "Min encoding efficiency violated.");
                }
            }
        }

        static void AssertRoundTrip(ReadOnlySpan<byte> expected, ReadOnlySpan<byte> actual)
        {
            if (!expected.SequenceEqual(actual))
            {
                Assert.Fail(
                    "Encoding round trip error for data block {0}. Actual decoded data are {1}.",
                    Base16.GetString(expected, DataEncodingOptions.Indent),
                    Base16.GetString(actual, DataEncodingOptions.Indent));
            }
        }

        public static void RoundTrip(ITextDataEncoding encoding, ReadOnlySpan<byte> raw, DataEncodingOptions options = default)
        {
            string actualEncoded = encoding.GetString(raw, options);
            var actualDecoded = encoding.GetBytes(actualEncoded, options);
            AssertRoundTrip(raw, actualDecoded);
        }

        public static void RandomRoundTrip(ITextDataEncoding encoding, int maxByteCount, int iterations, DataEncodingOptions options = default)
        {
            var buffer = new byte[maxByteCount];

            for (int i = 0; i < iterations; ++i)
            {
                int n = RandomNumberGenerator.GetInt32(buffer.Length + 1);
                var span = buffer.AsSpan(0, n);

                RandomNumberGenerator.Fill(span);
                RoundTrip(encoding, span, options);
            }
        }

        #endregion

        #region BigInteger

        public static void TestVector(
            INumericTextDataEncoding dataEncoding,
            BigInteger raw,
            string encoded,
            DataEncodingOptions options = DataEncodingOptions.None)
        {
            if (dataEncoding == null)
                throw new ArgumentNullException(nameof(dataEncoding));
            if (raw == null)
                throw new ArgumentNullException(nameof(raw));
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            // -----------------------------------------------------------------
            // Check text-based numerics encoding API
            // -----------------------------------------------------------------

            string actualEncoded = dataEncoding.GetString(raw, options);
            Assert.AreEqual(encoded, actualEncoded, "Encoding error.");

            var actualDecoded = dataEncoding.GetBigInteger(actualEncoded.AsSpan(), options);
            Assert.AreEqual(raw, actualDecoded, "Decoding error.");
        }

        #endregion
    }
}
