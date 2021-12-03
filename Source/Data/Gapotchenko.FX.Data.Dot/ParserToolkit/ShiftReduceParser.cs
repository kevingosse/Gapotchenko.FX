﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Gapotchenko.FX.Data.Dot.ParserToolkit
{
    /// <summary>
    /// Abstract class for GPPG shift-reduce parsers.
    /// Parsers generated by GPPG derive from this base
    /// class, overriding the abstract Initialize() and
    /// DoAction() methods.
    /// </summary>
    /// <typeparam name="TValue">Semantic value type.</typeparam>
    /// <typeparam name="TSpan">Location type.</typeparam>
    abstract class ShiftReduceParser<TValue, TSpan>
        where TSpan : IMerge<TSpan>, new()
    {
        protected abstract State[] States { get; }
        protected abstract Rule[] Rules { get; }
        protected abstract string[] NonTerms { get; }

        protected void InitStates(State[] states) { }
        protected void InitRules(Rule[] states) { }
        protected void InitNonTerminals(string[] states) { }

        // ==============================================================
        //                    TECHNICAL EXPLANATION.
        //   Why the next two fields are not exposed via properties.
        // ==============================================================
        // These fields are of the generic parameter types, and are
        // frequently instantiated as struct types in derived classes.
        // Semantic actions are defined in the derived classes and refer
        // to instance fields of these structs.  In such cases the code
        // "get_CurrentSemanticValue().myField = blah;" will fail since
        // the getter pushes the value of the field, not the reference.
        // So, in the presence of properties, gppg would need to encode
        // such field accesses as ... 
        //  "tmp = get_CurrentSemanticValue(); // Fetch value
        //   tmp.myField = blah;               // update
        //   set_CurrentSemanticValue(tmp); "  // Write update back.
        // There is no issue if TValue is restricted to be a ref type.
        // The same explanation applies to scanner.yylval.
        // ==============================================================
        /// <summary>
        /// The current value of the "$$" symbolic variable in the parser
        /// </summary>
        protected TValue? CurrentSemanticValue;

        /// <summary>
        /// The current value of the "@$" symbolic variable in the parser
        /// </summary>
        protected TSpan? CurrentLocationSpan;
        protected int NextToken;

        TSpan? LastSpan;
        State? FsaState;
        bool _recovering;
        int _tokensSinceLastError;

        PushdownPrefixState<State> StateStack = new PushdownPrefixState<State>();

        /// <summary>
        /// The stack of semantic value (YYSTYPE) values.
        /// </summary>
        protected PushdownPrefixState<TValue?> ValueStack { get; } = new();

        /// <summary>
        /// The stack of location value (YYLTYPE) varlues.
        /// </summary>
        protected PushdownPrefixState<TSpan?> LocationStack { get; } = new();

        int _errorToken;
        int _endOfFileToken;

        /// <summary>
        /// Initialization method to allow derived classes
        /// to insert the special value for the error and EOF tokens.
        /// </summary>
        /// <param name="err">The error state ordinal</param>
        /// <param name="end">The EOF stat ordinal</param>
        protected void InitSpecialTokens(int err, int end)
        {
            _errorToken = err;
            _endOfFileToken = end;
        }

        #region YYAbort, YYAccept etcetera.

        sealed class AcceptException : Exception
        {
            public AcceptException() { }
        }

        sealed class AbortException : Exception
        {
            public AbortException() { }
        }

        sealed class ErrorException : Exception
        {
            public ErrorException() { }
        }

        // The following methods are only called from within
        // a semantic action. The thrown exceptions can never
        // propagate outside the ShiftReduceParser class in 
        // which they are nested.

        /// <summary>
        /// Force parser to terminate, returning "true"
        /// </summary>
        protected static void YYAccept() => throw new AcceptException();

        /// <summary>
        /// Force parser to terminate, returning "false"
        /// </summary>
        protected static void YYAbort() => throw new AbortException();

        /// <summary>
        /// Force parser to terminate, returning
        /// "false" if error recovery fails.
        /// </summary>
        protected static void YYError() => throw new ErrorException();

        /// <summary>
        /// Check if parser in error recovery state.
        /// </summary>
        protected bool YYRecovering => _recovering;
        #endregion

        /// <summary>
        /// Abstract base method. ShiftReduceParser calls this
        /// to initialize the base class data structures.  Concrete
        /// parser classes must override this method.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Main entry point of the Shift-Reduce Parser.
        /// </summary>
        /// <returns>True if parse succeeds, else false for
        /// unrecoverable errors</returns>
        public bool Parse()
        {
            Initialize();	// allow derived classes to instantiate rules, states and nonTerminals

            NextToken = 0;
            FsaState = States![0];

            StateStack.Push(FsaState);
            ValueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);

            while (true)
            {
                int action = FsaState.defaultAction;

                if (FsaState.ParserTable != null)
                {
                    if (NextToken == 0)
                    {
                        // We save the last token span, so that the location span
                        // of production right hand sides that begin or end with a
                        // nullable production will be correct.
                        LastSpan = yylloc;
                        NextToken = yylex();
                    }

                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];
                }

                if (action > 0)         // shift
                {
                    Shift(action);
                }
                else if (action < 0)   // reduce
                {
                    try
                    {
                        Reduce(-action);
                        if (action == -1)	// accept
                            return true;
                    }
                    catch (Exception x)
                    {
                        if (x is AbortException)
                            return false;
                        else if (x is AcceptException)
                            return true;
                        else if (x is ErrorException && !ErrorRecovery())
                            return false;
                        else
                            throw;  // Rethrow x, preserving information.

                    }
                }
                else if (action == 0)   // error
                    if (!ErrorRecovery())
                        return false;
            }
        }

        void Shift(int stateIndex)
        {
            FsaState = States![stateIndex];

            ValueStack.Push(yylval);
            StateStack.Push(FsaState);
            LocationStack.Push(yylloc);

            if (_recovering)
            {
                if (NextToken != _errorToken)
                    _tokensSinceLastError++;

                if (_tokensSinceLastError > 5)
                    _recovering = false;
            }

            if (NextToken != _endOfFileToken)
                NextToken = 0;
        }

        void Reduce(int ruleNumber)
        {
            Rule rule = Rules[ruleNumber];
            int rhLen = rule.RightHandSide.Length;
            //
            //  Default actions for unit productions.
            //
            if (rhLen == 1)
            {
                CurrentSemanticValue = ValueStack.TopElement();   // Default action: $$ = $1;
                CurrentLocationSpan = LocationStack.TopElement(); // Default action "@$ = @1;
            }
            else if (rhLen == 0)
            {
                // Create a new blank value.
                // Explicit semantic action may mutate this value
                CurrentSemanticValue = default(TValue);
                // The location span for an empty production will start with the
                // beginning of the next lexeme, and end with the finish of the
                // previous lexeme.  This gives the correct behaviour when this
                // nonsense value is used in later Merge operations.
                CurrentLocationSpan = (yylloc != null && LastSpan != null ?
                    yylloc.Merge(LastSpan) :
                    default(TSpan));
            }
            else
            {
                // Default action: $$ = $1;
                CurrentSemanticValue = ValueStack[LocationStack.Depth - rhLen];
                //  Default action "@$ = @1.Merge(@N)" for location info.
                var at1 = LocationStack[LocationStack.Depth - rhLen];
                var atN = LocationStack[LocationStack.Depth - 1];
                CurrentLocationSpan =
                    ((at1 != null && atN != null) ? at1.Merge(atN) : default(TSpan));
            }

            DoAction(ruleNumber);

            for (int i = 0; i < rule.RightHandSide.Length; i++)
            {
                StateStack.Pop();
                ValueStack.Pop();
                LocationStack.Pop();
            }
            FsaState = StateStack.TopElement();

            if (FsaState.Goto?.ContainsKey(rule.LeftHandSide) == true)
                FsaState = States[FsaState.Goto[rule.LeftHandSide]];

            StateStack.Push(FsaState);
            ValueStack.Push(CurrentSemanticValue);
            LocationStack.Push(CurrentLocationSpan);
        }

        /// <summary>
        /// Execute the selected action from array.
        /// Must be overriden in derived classes.
        /// </summary>
        /// <param name="actionNumber">Index of the action to perform</param>
        protected abstract void DoAction(int actionNumber);

        bool ErrorRecovery()
        {
            bool discard;

            if (!_recovering) // if not recovering from previous error
                ReportError();

            if (!FindErrorRecoveryState())
                return false;
            //
            //  The interim fix for the "looping in error recovery"
            //  artifact involved moving the setting of the recovering 
            //  bool until after invalid tokens have been discarded.
            //
            ShiftErrorToken();
            discard = DiscardInvalidTokens();
            _recovering = true;
            _tokensSinceLastError = 0;
            return discard;
        }

        void ReportError()
        {
            StringBuilder errorMsg = new StringBuilder();
            errorMsg.AppendFormat("Syntax error, unexpected {0}", TerminalToString(NextToken));

            Debug.Assert(FsaState?.ParserTable is not null);

            if (FsaState.ParserTable.Count < 7)
            {
                bool first = true;
                foreach (int terminal in FsaState.ParserTable.Keys)
                {
                    if (first)
                        errorMsg.Append(", expecting ");
                    else
                        errorMsg.Append(", or ");

                    errorMsg.Append(TerminalToString(terminal));
                    first = false;
                }
            }
            yyerror(errorMsg.ToString());
        }

        void ShiftErrorToken()
        {
            int old_next = NextToken;
            NextToken = _errorToken;

            Debug.Assert(FsaState?.ParserTable is not null);

            Shift(FsaState.ParserTable[NextToken]);

            NextToken = old_next;
        }

        bool FindErrorRecoveryState()
        {
            Debug.Assert(FsaState is not null);

            while (true)    // pop states until one found that accepts error token
            {
                if (FsaState.ParserTable != null &&
                    FsaState.ParserTable.ContainsKey(_errorToken) &&
                    FsaState.ParserTable[_errorToken] > 0) // shift
                    return true;

                StateStack.Pop();
                ValueStack.Pop();
                LocationStack.Pop();

                if (StateStack.IsEmpty())
                {
                    return false;
                }
                else
                {
                    FsaState = StateStack.TopElement();
                }
            }
        }

        bool DiscardInvalidTokens()
        {
            Debug.Assert(FsaState is not null);

            int action = FsaState.defaultAction;

            if (FsaState.ParserTable != null)
            {
                // Discard tokens until find one that works ...
                while (true)
                {
                    if (NextToken == 0)
                    {
                        NextToken = yylex();
                    }
                    if (NextToken == _endOfFileToken)
                        return false;

                    if (FsaState.ParserTable.ContainsKey(NextToken))
                        action = FsaState.ParserTable[NextToken];

                    if (action != 0)
                        return true;
                    else
                    {
                        NextToken = 0;
                    }
                }
            }
            else if (_recovering && _tokensSinceLastError == 0)
            {
                // 
                //  Boolean recovering is not set until after the first
                //  error token has been shifted.  Thus if we get back 
                //  here with recovering set and no tokens read we are
                //  looping on the same error recovery action.  This 
                //  happens if current_state.ParserTable is null because
                //  the state has an LR(0) reduction, but not all
                //  lookahead tokens are valid.  This only occurs for
                //  error productions that *end* on "error".
                //
                //  This action discards tokens one at a time until
                //  the looping stops.  Another attack would be to always
                //  use the LALR(1) table if a production ends on "error"
                //
                if (NextToken == _endOfFileToken)
                    return false;
                NextToken = 0;
                return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Traditional YACC method.  Discards the next input token.
        /// </summary>
        protected void yyclearin() { NextToken = 0; }

        /// <summary>
        /// Tradional YACC method. Clear the "recovering" flag.
        /// </summary>
        protected void yyerrok() { _recovering = false; }

        /// <summary>
        /// OBSOLETE FOR VERSION 1.4.0
        /// Method used by derived types to insert new
        /// state instances in the "states" array.
        /// </summary>
        /// <param name="stateNumber">index of the state</param>
        /// <param name="state">data for the state</param>
        protected void AddState(int stateNumber, State state)
        {
            States[stateNumber] = state;
            state.number = stateNumber;
        }

        /// <summary>
        /// Abstract state class naming terminal symbols.
        /// This is overridden by derived classes with the
        /// name (or alias) to be used in error messages.
        /// </summary>
        /// <param name="terminal">The terminal ordinal</param>
        /// <returns></returns>
        protected abstract string TerminalToString(int terminal);

        /// <summary>
        /// Return text representation of argument character
        /// </summary>
        /// <param name="input">The character to convert</param>
        /// <returns>String representation of the character</returns>
        protected static string CharToString(char input)
        {
            return input switch
            {
                '\a' => @"'\a'",
                '\b' => @"'\b'",
                '\f' => @"'\f'",
                '\n' => @"'\n'",
                '\r' => @"'\r'",
                '\t' => @"'\t'",
                '\v' => @"'\v'",
                '\0' => @"'\0'",
                _ => string.Format(CultureInfo.InvariantCulture, "'{0}'", input),
            };
        }

        protected abstract TSpan? yylloc { get; }
        protected abstract TValue yylval { get; }
        protected abstract int yylex();
        protected abstract void yyerror(string message);
    }
}
