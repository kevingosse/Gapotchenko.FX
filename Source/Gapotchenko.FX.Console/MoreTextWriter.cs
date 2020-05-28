﻿using Gapotchenko.FX.Text;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Console
{
    using Console = System.Console;

    /// <summary>
    /// Automatically manages console behavior when written data exceeds the size of a console area visible to the user.
    /// The behavior is very similar to 'more' command line utility.
    /// </summary>
    /// <remarks>
    /// <see cref="MoreTextWriter"/> handles redirected console streams automatically.
    /// For those no continue prompts are generated.
    /// </remarks>
    public class MoreTextWriter : TextWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoreTextWriter"/> class.
        /// </summary>
        public MoreTextWriter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreTextWriter"/> class.
        /// </summary>
        /// <param name="baseTextWriter">The underlying text writer.</param>
        public MoreTextWriter(TextWriter baseTextWriter) :
            this()
        {
            BaseTextWriter = baseTextWriter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether automatic management of console behavior is enabled
        /// when written data exceeds the size of a console area visible to the user.
        /// </summary>
        public bool Enabled { get; set; } = true;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_CachedScreenHeight = -1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int ScreenHeight
        {
            get
            {
                int v = m_CachedScreenHeight;
                if (v == -1)
                {
                    v = _GetScreenHeightCore();
                    m_CachedScreenHeight = v;
                }
                return v;
            }
        }

        [DebuggerHidden]
        static int _GetScreenHeightCore()
        {
            try
            {
                return Console.WindowHeight;
            }
            catch
            {
                return 0;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        TextWriter m_BaseTextWriter;

        /// <summary>
        /// Gets or sets the underlying text writer.
        /// </summary>
        public TextWriter BaseTextWriter
        {
            get
            {
                return m_BaseTextWriter;
            }
            set
            {
                if (value == m_BaseTextWriter)
                    return;

                if (m_BaseTextWriter == this)
                    throw new ArgumentException("Cannot set the base text writer to itself.", nameof(value));

                m_BaseTextWriter = value;
                RecalculateSkipCriteria();
            }
        }

        /// <summary>
        /// Discards any information about console state that has been cached.
        /// </summary>
        /// <remarks>
        /// Call this method after <see cref="Console.SetOut(TextWriter)"/> or <see cref="Console.SetError(TextWriter)"/> call.
        /// </remarks>
        public void Refresh()
        {
            m_CachedScreenHeight = -1;
            RecalculateSkipCriteria();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool m_Skip;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool m_SkipCriteriaNeedsProbing;

        void RecalculateSkipCriteria()
        {
            m_SkipCriteriaNeedsProbing = false;
            m_Skip = ScreenHeight == 0;
            if (m_Skip)
                return;

            if (m_BaseTextWriter == null)
            {
                m_Skip = true;
                return;
            }

            if (!Environment.UserInteractive)
            {
                m_Skip = true;
                return;
            }

            if (m_BaseTextWriter != Console.Error &&
                m_BaseTextWriter != Console.Out)
            {
                m_Skip = true;
                return;
            }

            m_Skip = false;
            m_SkipCriteriaNeedsProbing = true;
        }

        /// <summary>
        /// Returns the character encoding in which the output is written.
        /// </summary>
        public override Encoding Encoding => m_BaseTextWriter.Encoding;

        void ValidateBaseTextWriter()
        {
            if (m_BaseTextWriter == null)
                throw new InvalidOperationException("Base text writer is not set.");
        }

        /// <summary>
        /// Writes a character to the text string or stream.
        /// </summary>
        /// <param name="value">The character to write to the text stream.</param>
        public override void Write(char value)
        {
            if (m_Skip || !Enabled)
            {
                ValidateBaseTextWriter();
                m_BaseTextWriter.Write(value);
            }
            else
            {
                Write(new string(value, 1));
            }
        }

        /// <summary>
        /// Writes a character array to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write to the text stream.</param>
        public override void Write(char[] buffer)
        {
            if (m_Skip || !Enabled)
            {
                ValidateBaseTextWriter();
                m_BaseTextWriter.Write(buffer);
            }
            else
            {
                Write(new string(buffer));
            }
        }

        /// <summary>
        /// Writes a subarray of characters to the text string or stream.
        /// </summary>
        /// <param name="buffer">The character array to write data from.</param>
        /// <param name="index">The character position in the buffer at which to start retrieving data.</param>
        /// <param name="count">The number of characters to write.</param>
        public override void Write(char[] buffer, int index, int count)
        {
            if (m_Skip || !Enabled)
            {
                ValidateBaseTextWriter();
                m_BaseTextWriter.Write(buffer, index, count);
            }
            else
            {
                Write(new string(buffer, index, count));
            }
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write. If value is null, only the line terminator is written.</param>
        public override void WriteLine(string value)
        {
            if (!Enabled)
            {
                ValidateBaseTextWriter();
                m_BaseTextWriter.WriteLine(value);
            }

            Write(value);
            Write(CoreNewLine);
        }

        void _BaseWrite(string value)
        {
            ValidateBaseTextWriter();

            if (!m_SkipCriteriaNeedsProbing || !Enabled)
            {
                m_BaseTextWriter.Write(value);
            }
            else
            {
                // Make a probe to find out whether the output stream is directly rendered to the console or redirected to another destination.

                // If a string is empty or consists only of control characters that MAY NOT affect console canvas then
                // probing is deferred till the next better character sequence is encountered.
                bool canProbeNow =
                    !value.EndsWith('\r') &&
                    value.AsSpan().Trim(new[] { '\n', '\r', '\b' }).Length != 0;

                if (!canProbeNow)
                {
                    m_BaseTextWriter.Write(value);
                    return;
                }

                int top = Console.CursorTop;
                int left = Console.CursorLeft;

                m_BaseTextWriter.Write(value);
                m_BaseTextWriter.Flush();

                bool consoleChanged = Console.CursorTop != top || Console.CursorLeft != left;

                m_Skip = !consoleChanged;
                m_SkipCriteriaNeedsProbing = false;
            }
        }

        /// <summary>
        /// Writes a string to the text string or stream.
        /// </summary>
        /// <param name="value">The string to write.</param>
        public override void Write(string value)
        {
            ValidateBaseTextWriter();

            if (m_Skip || !Enabled)
            {
                m_BaseTextWriter.Write(value);
                return;
            }

            string[] lines = value.Split('\n');

            int n = lines.Length;
            for (int i = 0; i < n; i++)
            {
                string line = lines[i];

                if (line.Length > 0)
                    _BaseWrite(line);

                if (i != n - 1)
                {
                    _BaseWrite("\n");
                    OnNewLine();
                }
            }
        }

        /// <summary>
        /// Clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            base.Flush();

            if (m_BaseTextWriter != null)
                m_BaseTextWriter.Flush();
        }

        /// <summary>
        /// Asynchronously clears all buffers for the current writer and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        public async override Task FlushAsync()
        {
            await base.FlushAsync().ConfigureAwait(false);

            if (m_BaseTextWriter != null)
                await m_BaseTextWriter.FlushAsync().ConfigureAwait(false);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_WrittenLineCount;

        static class CursorRecovery
        {
            static CursorRecovery()
            {
                Console.CancelKeyPress += Console_CancelKeyPress;
            }

            static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
            {
                Console.CursorVisible = true;
            }

            public static void Activate()
            {
            }
        }

        void HandleUI()
        {
            ValidateBaseTextWriter();

            int left = Console.CursorLeft;
            ShowPrompt(m_BaseTextWriter);
            m_BaseTextWriter.Flush();
            int promptLength = Console.CursorLeft - left;

            bool savedCursorVisible = Console.CursorVisible;
            CursorRecovery.Activate();
            Console.CursorVisible = false;
            try
            {
                for (; ; )
                {
                    ConsoleKeyInfo consoleKeyInfo;
                    try
                    {
                        consoleKeyInfo = Console.ReadKey(true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Input stream is redirected.
                        m_Skip = true;
                        break;
                    }

                    var action = GetInteractiveAction(consoleKeyInfo.Key);
                    switch (action)
                    {
                        case InteractiveAction.ScrollToNextPage:
                            m_WrittenLineCount = 0;
                            break;

                        case InteractiveAction.ScrollToNextLine:
                            --m_WrittenLineCount;
                            break;

                        default:
                            continue;
                    }

                    break;
                }

                m_BaseTextWriter.Write('\r');
                for (int i = 0; i < promptLength; ++i)
                    m_BaseTextWriter.Write(' ');
                m_BaseTextWriter.Write('\r');
                m_BaseTextWriter.Flush();
            }
            finally
            {
                Console.CursorVisible = savedCursorVisible;
            }
        }

        /// <summary>
        /// Shows a prompt.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        protected virtual void ShowPrompt(TextWriter textWriter)
        {
            textWriter.Write("(Press <Page Down> to scroll page, <Down Arrow> to scroll line)");
        }

        /// <summary>
        /// Defines an interactive action.
        /// </summary>
        protected enum InteractiveAction
        {
            /// <summary>
            /// No action.
            /// </summary>
            None,

            /// <summary>
            /// Scroll to next page.
            /// </summary>
            ScrollToNextPage,

            /// <summary>
            /// Scroll to next line.
            /// </summary>
            ScrollToNextLine
        }

        /// <summary>
        /// Gets an interactive action for the specified console key.
        /// </summary>
        /// <param name="key">The console key.</param>
        /// <returns>The interactive action.</returns>
        protected virtual InteractiveAction GetInteractiveAction(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.PageDown:
                case ConsoleKey.Spacebar:
                    return InteractiveAction.ScrollToNextPage;

                case ConsoleKey.DownArrow:
                case ConsoleKey.Enter:
                    return InteractiveAction.ScrollToNextLine;

                default:
                    return InteractiveAction.None;
            }
        }

        void OnNewLine()
        {
            if (++m_WrittenLineCount >= ScreenHeight - 1)
                HandleUI();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="MoreTextWriter"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_BaseTextWriter != null)
                    m_BaseTextWriter.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets a value indicating whether a <see cref="MoreTextWriter"/> is interactive.
        /// </summary>
        public bool IsInteractive => !m_Skip;
    }
}
