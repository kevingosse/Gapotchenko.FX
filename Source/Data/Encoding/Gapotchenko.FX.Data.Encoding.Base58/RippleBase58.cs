﻿using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Provides Ripple Base58 encoding implementation.
    /// </summary>
    public sealed class RippleBase58 : GenericBase58
    {
        private RippleBase58() :
            base(new TextDataEncodingAlphabet("rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz"))
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static volatile IBase58? m_Instance;

        /// <summary>
        /// Returns a default instance of <see cref="RippleBase58"/> encoding.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [CLSCompliant(false)]
        public static IBase58 Instance => m_Instance ??= new RippleBase58();
    }
}
