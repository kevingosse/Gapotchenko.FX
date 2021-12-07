﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    /// <summary>
    /// Represents DOT document vertex.
    /// </summary>
    public sealed class DotVertexNode : DotStatementNode
    {
        /// <summary>
        /// Gets or sets a vertex identifier.
        /// </summary>
        public DotVertexIdentifierNode? Identifier { get; set; }

        /// <summary>
        /// Gets or sets a list of attributes.
        /// </summary>
        public DotNodeList<DotAttributeListNode>? Attributes { get; set; }

        internal override int SlotCount => 3;

        internal override SyntaxSlot GetSlot(int i) => i switch
        {
            0 => Identifier,
            1 => new SyntaxSlot(Attributes),
            2 => SemicolonToken,
            _ => throw new ArgumentOutOfRangeException(nameof(i))
        };

        /// <inheritdoc />
        public override void Accept(DotDomVisitor visitor)
        {
            visitor.VisitDotVertexNode(this);
        }
    }
}