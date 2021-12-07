﻿namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents a <see cref="DotDomVisitor"/> that descends into an entire <see cref="DotNode"/> hierarchy
    /// visiting each node and its children in depth-first order.
    /// </summary>
    public abstract class DotDomWalker : DotDomVisitor
    {
        /// <summary>
        /// Gets a value indicating how deep <see cref="DotDomWalker"/> should descend.
        /// </summary>
        public DotDomWalkerDepth Depth { get; }

        /// <summary>
        /// Creates a new walker instance.
        /// </summary>
        /// <param name="depth">The value indicating how deep <see cref="DotDomWalker"/> should descend.</param>
        protected DotDomWalker(DotDomWalkerDepth depth = DotDomWalkerDepth.Nodes)
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
            if (Depth >= DotDomWalkerDepth.NodesTokensAndTrivia)
            {
                VisitLeadingTrivia(token);
                VisitTrailingTrivia(token);
            }
        }

        /// <inheritdoc/>
        protected override void DefaultVisit(DotNode node)
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
                    if (Depth >= DotDomWalkerDepth.Nodes)
                    {
                        asNode.Accept(this);
                    }
                }
                else
                {
                    if (Depth >= DotDomWalkerDepth.NodesAndTokens)
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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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