﻿using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Base58 and Base58Check encoding implementations according to https://en.bitcoin.it/wiki/Base58Check_encoding.
    /// </summary>
    public sealed class Base58 : GenericBase58
    {
        private Base58() :
            base(new TextDataEncodingAlphabet("123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz"))
        {
        }

        /// <inheritdoc/>
        public override string Name => "Base58";

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase58 m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="Base58"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IBase58 Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new Base58();
                return m_Instance;
            }
        }
    }
}