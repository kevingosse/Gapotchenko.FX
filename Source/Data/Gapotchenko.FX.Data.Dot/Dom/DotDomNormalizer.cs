﻿using Gapotchenko.FX.Collections.Generic;
using Gapotchenko.FX.Data.Dot.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    sealed class DotDomNormalizer : DotDomWalker
    {
        readonly string _indentation;
        readonly string _eol;

        public DotDomNormalizer(string indentation, string eol)
            : base(DotDomWalkerDepth.NodesAndTokens)
        {
            _indentation = indentation;
            _eol = eol;
        }

        int _indents = 0;

        public override void VisitToken(DotSignificantToken token)
        {
            base.VisitToken(token);

            TrimEnd(token);
            token.TrailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, " "));
        }

        int _depth = -1;

        public override void VisitDotStatementListNode(DotStatementListNode node)
        {
            _depth++;

            if (_depth is 0)
            {
                _indents++;
            }

            base.VisitDotStatementListNode(node);

            if (_depth is 0)
            {
                _indents--;

                PlaceEndOfLine(node.OpenBraceToken);
                PlaceEndOfLine(node.CloseBraceToken);
            }

            _depth--;
        }

        bool _nestedStatement = false;

        /// <inheritdoc/>
        protected override void DefaultVisit(DotNode node)
        {
            var shouldIndent = !_nestedStatement && node is DotStatementNode && _indents != 0;

            if (shouldIndent)
            {
                _nestedStatement = true;
            }

            base.DefaultVisit(node);

            if (shouldIndent)
            {
                _nestedStatement = false;
                node.LeadingTrivia.InsertRange(0, CreateIndentation());
                PlaceEndOfLine(node);
            }
        }

        IEnumerable<DotTrivia> CreateIndentation()
        {
            for (int i = 0; i < _indents; i++)
            {
                yield return new DotTrivia(DotTokenKind.Whitespace, _indentation);
            }
        }

        void PlaceEndOfLine(DotNode? node)
        {
            if (node is not null)
            {
                var trailingTrivia = node.TrailingTrivia;
                trailingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
                trailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, _eol));
            }
        }

        void PlaceEndOfLine(DotSignificantToken? token)
        {
            if (token is not null)
            {
                TrimEnd(token);
                token.TrailingTrivia.Add(new DotTrivia(DotTokenKind.Whitespace, _eol));
            }
        }

        static void TrimStart(DotSignificantToken token)
        {
            if (token.HasLeadingTrivia)
            {
                token.LeadingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
            }
        }

        static void TrimEnd(DotSignificantToken token)
        {
            if (token.HasTrailingTrivia)
            {
                token.TrailingTrivia.RemoveAll(t => t.Kind is DotTokenKind.Whitespace);
            }
        }

        public override void VisitDotAttributeListNode(DotAttributeListNode node)
        {
            base.VisitDotAttributeListNode(node);

            if (node.OpenBraceToken?.HasTrailingTrivia == true)
            {
                TrimEnd(node.OpenBraceToken);
            }

            var lastAttribute = node.Attributes?.LastOrDefault();
            if (lastAttribute is not null)
            {
                var lastToken = DotDomNavigator.TryGetLastToken(lastAttribute);
                if (lastToken is not null)
                {
                    TrimEnd(lastToken);
                }
            }
        }
    }
}