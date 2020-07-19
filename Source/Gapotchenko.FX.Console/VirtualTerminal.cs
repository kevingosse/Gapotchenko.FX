﻿using Gapotchenko.FX.Console.Emulation;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

#nullable enable

namespace Gapotchenko.FX.Console
{
    using Console = System.Console;

    /// <summary>
    /// Provides operations for VT100-compatible virtual console terminal.
    /// </summary>
    public static class VirtualTerminal
    {
        /// <summary>
        /// Enables virtual terminal processing for console output.
        /// Please note that every call to <see cref="EnableProcessing()"/> should be matched by a corresponding call to <see cref="RestoreProcessing"/>
        /// unless virtual terminal processing is globally enabled.
        /// </summary>
        public static void EnableProcessing() => EnableProcessing(VirtualTerminalProcessingMode.Auto);

        /// <summary>
        /// Enables virtual terminal processing for console output.
        /// Please note that every call to <see cref="EnableProcessing(VirtualTerminalProcessingMode)"/> should be matched by a corresponding call to <see cref="RestoreProcessing"/>
        /// unless virtual terminal processing is globally enabled.
        /// </summary>
        /// <param name="mode">The processing mode.</param>
        public static void EnableProcessing(VirtualTerminalProcessingMode mode)
        {
            if (Interlocked.Increment(ref m_ProcessingEntranceCounter) == 1)
                _EnableProcessingCore(mode);
        }

        /// <summary>
        /// Enables virtual terminal processing for console output.
        /// Please note that every call to <see cref="EnableProcessing(TextWriter)"/> should be matched by a corresponding call to <see cref="RestoreProcessing"/>
        /// unless virtual terminal processing is globally enabled.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <returns>
        /// A text writer wrapped by the virtual terminal emulator,
        /// or <paramref name="textWriter"/> if virtual terminal processing is natively supported by the platform.
        /// </returns>
        [return: NotNullIfNotNull("textWriter")]
        public static TextWriter? EnableProcessing(TextWriter? textWriter) => EnableProcessing(textWriter, VirtualTerminalProcessingMode.Auto);

        /// <summary>
        /// Enables virtual terminal processing for console output.
        /// Please note that every call to <see cref="EnableProcessing(TextWriter)"/> should be matched by a corresponding call to <see cref="RestoreProcessing"/>
        /// unless virtual terminal processing is globally enabled.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        /// <param name="mode">The processing mode.</param>
        /// <returns>
        /// A text writer wrapped by the virtual terminal emulator,
        /// or <paramref name="textWriter"/> if virtual terminal processing is natively supported by the platform.
        /// </returns>
        [return: NotNullIfNotNull("textWriter")]
        public static TextWriter? EnableProcessing(TextWriter? textWriter, VirtualTerminalProcessingMode mode)
        {
            EnableProcessing(mode);
            return _EmulateVTConsole(textWriter);
        }

        /// <summary>
        /// Restores virtual terminal processing for console output to a previous state.
        /// Please note that every call to <see cref="EnableProcessing()"/> should be matched by a corresponding call to <see cref="RestoreProcessing"/>
        /// unless virtual terminal processing is globally enabled.
        /// </summary>
        public static void RestoreProcessing()
        {
            int counter = Interlocked.Decrement(ref m_ProcessingEntranceCounter);
            if (counter == 0)
            {
                _RestoreProcessingCore();
            }
            else if (counter < 0)
            {
                Interlocked.Increment(ref m_ProcessingEntranceCounter);
                throw new InvalidOperationException();
            }
        }

        static int m_ProcessingEntranceCounter;

        static bool _TryEnablePlatformVTProcessing(out bool prevState)
        {
            var platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
            {
                // Unix-like OSes have inherent support for VT processing.
                prevState = true;
                return true;
            }

            prevState = false;

            var hOut = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
            if (hOut == NativeMethods.INVALID_HANDLE_VALUE)
                return false;

            uint mode;
            if (!NativeMethods.GetConsoleMode(hOut, out mode))
                return false;

            if ((mode & (uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0)
            {
                prevState = true;
                return true;
            }
            else
            {
                mode |= (uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                return NativeMethods.SetConsoleMode(hOut, mode);
            }
        }

        static bool _TryRestorePlatformVTProcessing(bool prevState)
        {
            var platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
            {
                // Unix-like OSes have inherent support for VT processing.
                // There is no simple way to disable it.
                return prevState;
            }

            var hOut = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
            if (hOut == NativeMethods.INVALID_HANDLE_VALUE)
                return false;

            uint mode;
            if (!NativeMethods.GetConsoleMode(hOut, out mode))
                return false;

            bool curState = (mode & (uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
            if (curState == prevState)
                return true;

            if (prevState)
                mode |= (uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            else
                mode &= ~(uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            return NativeMethods.SetConsoleMode(hOut, mode);
        }

        static bool _IsPlatformVTProcessingEnabled()
        {
            var platform = Environment.OSVersion.Platform;
            if (platform == PlatformID.Unix || platform == PlatformID.MacOSX)
            {
                // Unix-like OSes have inherent support for VT processing.
                return true;
            }

            var hOut = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE);
            if (hOut == NativeMethods.INVALID_HANDLE_VALUE)
                return false;

            uint mode;
            if (!NativeMethods.GetConsoleMode(hOut, out mode))
                return false;

            return (mode & (uint)NativeMethods.ConsoleOutputMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0;
        }

        static readonly object m_SyncRoot = new object();
        static bool m_IsEmulated;
        static bool m_NativeVTProcessingPrevState;
        static TextWriter? m_PrevConsoleOut;
        static TextWriter? m_PrevConsoleError;

        static void _EnableProcessingCore(VirtualTerminalProcessingMode mode)
        {
            lock (m_SyncRoot)
            {
                switch (mode)
                {
                    case VirtualTerminalProcessingMode.Auto:
                        if (_TryEnablePlatformVTProcessing(out m_NativeVTProcessingPrevState))
                        {
                            m_IsEmulated = false;
                            return;
                        }
                        goto case VirtualTerminalProcessingMode.Emulation;

                    case VirtualTerminalProcessingMode.Emulation:
                        m_IsEmulated = true;
                        Console.SetOut(_EmulateVTConsole(m_PrevConsoleOut = Console.Out));
                        Console.SetError(_EmulateVTConsole(m_PrevConsoleError = Console.Error));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode));
                }
            }
        }

        static void _RestoreProcessingCore()
        {
            lock (m_SyncRoot)
            {
                if (m_IsEmulated)
                {
                    Console.SetOut(m_PrevConsoleOut);
                    Console.SetError(m_PrevConsoleError);

                    m_IsEmulated = false;
                    m_PrevConsoleOut = null;
                    m_PrevConsoleError = null;
                }
                else
                {
                    _TryRestorePlatformVTProcessing(m_NativeVTProcessingPrevState);
                }
            }
        }

        [return: NotNullIfNotNull("textWriter")]
        static TextWriter? _EmulateVTConsole(TextWriter? textWriter)
        {
            if (m_IsEmulated)
            {
                if (textWriter == null)
                    return null;
                else
                    return new VirtualTerminalWriter(new VirtualTerminalConsoleOutputBackend(textWriter));
            }
            else
            {
                return textWriter;
            }
        }

        /// <summary>
        /// Gets the current state of virtual terminal processing.
        /// </summary>
        public static VirtualTerminalProcessingState ProcessingState
        {
            get
            {
                if (m_IsEmulated)
                    return VirtualTerminalProcessingState.Emulation;
                else if (_IsPlatformVTProcessingEnabled())
                    return VirtualTerminalProcessingState.Platform;
                else
                    return VirtualTerminalProcessingState.Disabled;
            }
        }
    }
}
