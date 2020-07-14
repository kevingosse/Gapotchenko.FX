﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

#nullable enable

namespace Gapotchenko.FX.Diagnostics.Implementation.Windows
{
    sealed class ProcessMemoryAdapter : IProcessMemoryAdapter
    {
        public ProcessMemoryAdapter(IntPtr hProcess)
        {
            m_hProcess = hProcess;
        }

        IntPtr m_hProcess;

        public int PageSize => SystemInfo.PageSize;

        public unsafe int ReadMemory(UniPtr address, byte[] buffer, int offset, int count, bool throwOnError)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length)
                throw new ArgumentException();

            var actualCount = IntPtr.Zero;
            bool result;

            fixed (byte* p = buffer)
            {
                result = NativeMethods.ReadProcessMemory(
                    m_hProcess,
                    address,
                    p + offset,
                    new IntPtr(count),
                    ref actualCount);
            }

            if (!result)
            {
                if (throwOnError)
                    throw new Win32Exception(Marshal.GetLastWin32Error());

                return -1;
            }

            return actualCount.ToInt32();
        }
    }
}
