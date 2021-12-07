﻿using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Data.Dot.Serialization;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Formats whitespace in syntax trees.
    /// </summary>
    public static class DotFormatter
    {
        const string DefaultIndentation = "    ";
        const string DefaultEOL = "\r\n";

        /// <summary>
        /// Replaces whitespaces and end of line trivia with regularly formatted trivia.
        /// </summary>
        public static void NormalizeWhitespace<TNode>(
            this TNode node,
            string indentation = DefaultIndentation,
            string eol = DefaultEOL)
            where TNode : DotNode
        {
            node.Accept(new WhitespaceEraser());
            node.Accept(new SyntaxNormalizer(indentation, eol));
        }

        sealed class WhitespaceEraser : DotDomWalker
        {
            public WhitespaceEraser() : base(DotDomWalkerDepth.NodesAndTokens)
            { }

            public override void VisitToken(DotToken token)
            {
                if (token.HasLeadingTrivia)
                {
                    token.LeadingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
                }

                if (token.HasTrailingTrivia)
                {
                    token.TrailingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
                }
            }
        }
    }
}