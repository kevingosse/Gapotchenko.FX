﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotSyntaxVisitor"/> that descends an entire <see cref="DotNode"/> graph
    /// visiting each node and its child nodes and tokens in depth-first order.
    /// </summary>
    public abstract class DotSyntaxWalker : DotSyntaxVisitor
    {
        /// <summary>
        /// Syntax the <see cref="DotSyntaxWalker"/> should descend into.
        /// </summary>
        protected SyntaxWalkerDepth Depth { get; }

        /// <summary>
        /// Creates a new walker instance.
        /// </summary>
        /// <param name="depth">Syntax the <see cref="DotSyntaxWalker"/> should descend into.</param>
        protected DotSyntaxWalker(SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node)
        {
            Depth = depth;
        }

        /// <summary>
        /// Called when the walker visits a token.
        /// </summary>
        /// <remarks>
        /// This method may be overridden if subclasses want
        /// to handle the token. Overrides should call back into this base method if they want the 
        /// trivia of this token to be visited.
        /// </remarks>
        /// <param name="token">The current token that the walker is visiting.</param>
        public virtual void VisitToken(DotToken token)
        {
            if (Depth >= SyntaxWalkerDepth.Trivia)
            {
                VisitLeadingTrivia(token);
                VisitTrailingTrivia(token);
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void DefaultVisit(DotNode node)
        {
            var childCnt = node.ChildNodesAndTokens.Count;
            int i = 0;

            do
            {
                var child = DotChildNodeList.ItemInternal(node, i);
                i++;

                var asNode = child.AsNode();
                if (asNode is not null)
                {
                    if (Depth >= SyntaxWalkerDepth.Node)
                    {
                        asNode.Accept(this);
                    }
                }
                else
                {
                    if (Depth >= SyntaxWalkerDepth.Token)
                    {
                        var token = child.AsToken();
                        if (token is not null)
                        {
                            VisitToken(token);
                        }
                    }
                }
            } while (i < childCnt);
        }

        public virtual void VisitLeadingTrivia(DotToken token)
        {
            if (token.HasLeadingTrivia)
            {
                foreach (var tr in token.LeadingTrivia)
                {
                    VisitTrivia(tr);
                }
            }
        }

        public virtual void VisitTrailingTrivia(DotToken token)
        {
            if (token.HasTrailingTrivia)
            {
                foreach (var tr in token.TrailingTrivia)
                {
                    VisitTrivia(tr);
                }
            }
        }

        public virtual void VisitTrivia(DotTrivia trivia)
        {
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
