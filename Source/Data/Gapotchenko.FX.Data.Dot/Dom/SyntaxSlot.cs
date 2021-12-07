﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Dot.Dom
{
    struct SyntaxSlot
    {
        readonly DotToken? _token;
        readonly DotNode? _node;
        readonly IReadOnlyList<DotNode>? _list;

        public SyntaxSlot(DotToken? token)
            : this(token, null, null)
        { }

        public SyntaxSlot(DotNode? node)
            : this(null, node, null)
        { }

        public SyntaxSlot(DotNodeOrToken nodeOrToken)
            : this(
                  nodeOrToken.IsToken ? nodeOrToken.AsToken() : null,
                  nodeOrToken.IsNode ? nodeOrToken.AsNode() : null,
                  null)
        { }

        public SyntaxSlot(IReadOnlyList<DotNode>? list)
            : this(null, null, list)
        { }

        SyntaxSlot(
            DotToken? token,
            DotNode? node,
            IReadOnlyList<DotNode>? list)
        {
            if (token is not null)
            {
                _token = token;
                IsToken = true;
            }
            else
            {
                _token = null;
                IsToken = false;
            }

            if (node is not null)
            {
                _node = node;
                IsNode = true;
            }
            else
            {
                _node = null;
                IsNode = false;
            }

            if (list is not null)
            {
                _list = list;
                IsList = true;
            }
            else
            {
                _list = null;
                IsList = false;
            }
        }

        public static implicit operator SyntaxSlot(DotToken? token) =>
            new SyntaxSlot(token);
        public static implicit operator SyntaxSlot(DotNode? node) =>
            new SyntaxSlot(node);
        public static implicit operator SyntaxSlot(DotNodeOrToken nodeOrToken) =>
            new SyntaxSlot(nodeOrToken);

        public bool IsDefault => !IsNode && !IsToken && !IsList;

        public bool IsNode { get; }
        public bool IsToken { get; }
        public bool IsList { get; }

        [MemberNotNullWhen(true, nameof(_token))]
        public DotToken? AsToken() => _token;

        [MemberNotNullWhen(true, nameof(_node))]
        public DotNode? AsNode() => _node;

        public IReadOnlyList<DotNode>? AsList() => _list;

        public int SlotCount
        {
            get
            {
                return
                    IsNode ? _node!.SlotCount :
                    IsList ? _list is ISyntaxSlotProvider slotProvider ? slotProvider.SlotCount : _list!.Count :
                    0;
            }
        }

        public SyntaxSlot GetSlot(int index)
        {
            return
                IsNode ? _node!.GetSlot(index) :
                IsList ? _list is ISyntaxSlotProvider slotProvider ? slotProvider.GetSlot(index) : _list![index] :
                throw new InvalidOperationException("A current slot has no child slots.");
        }
    }
}