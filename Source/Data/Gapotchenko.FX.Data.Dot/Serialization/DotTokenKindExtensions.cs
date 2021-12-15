﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Gapotchenko.FX.Data.Dot.Serialization
{
    static class DotTokenKindExtensions
    {
        public static string GetDefaultValue(this DotTokenKind token)
        {
            if (!TryGetDefaultValue(token, out var defaultValue))
                throw new ArgumentException("Token text cannot deducted from the kind.", nameof(token));
            return defaultValue;
        }

        public static bool TryGetDefaultValue(this DotTokenKind token, [NotNullWhen(true)] out string? value)
        {
            value = token switch
            {
                DotTokenKind.Digraph => "digraph",
                DotTokenKind.Graph => "graph",
                DotTokenKind.Arrow => "->",
                DotTokenKind.Subgraph => "subgraph",
                DotTokenKind.Node => "node",
                DotTokenKind.Edge => "edge",
                DotTokenKind.Whitespace => " ",
                DotTokenKind.ScopeStart => "{",
                DotTokenKind.ScopeEnd => "}",
                DotTokenKind.Semicolon => ";",
                DotTokenKind.Equal => "=",
                DotTokenKind.ListStart => "[",
                DotTokenKind.ListEnd => "]",
                DotTokenKind.Comma => ",",
                DotTokenKind.Colon => ":",
                _ => null,
            };

            return value is not null;
        }
    }
}