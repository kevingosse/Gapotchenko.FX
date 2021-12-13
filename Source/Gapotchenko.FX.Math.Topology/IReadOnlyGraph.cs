﻿using System;
using System.Collections.Generic;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Defines an interface for a read-only graph.
    /// </para>
    /// <para>
    /// Graph is a set of vertices and edges.
    /// Vertices represent the objects and edges represents the relations between them.
    /// </para>
    /// </summary>
    /// <typeparam name="T">Vertex type.</typeparam>
    public interface IReadOnlyGraph<T> : ICloneable<IReadOnlyGraph<T>>
    {
        /// <summary>
        /// Gets a set containing the vertices of the graph.
        /// </summary>
        IReadOnlySet<T> Vertices { get; }

        /// <summary>
        /// Gets a set containing the edges of the graph.
        /// </summary>
        IReadOnlySet<GraphEdge<T>> Edges { get; }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A path consists of one or more edges with or without intermediate vertices.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <returns><c>true</c> when the specified source vertex can reach the target; otherwise, <c>false</c>.</returns>
        bool ContainsPath(T from, T to);

        /// <summary>
        /// Gets the vertices adjacent to a specified vertex.
        /// </summary>
        /// <param name="vertex">The vertex to find the adjacent vertices for.</param>
        /// <returns>Sequence of vertices adjacent to vertex <paramref name="vertex"/>.</returns>
        IEnumerable<T> VerticesAdjacentTo(T vertex);

        /// <summary>
        /// Gets a value indicating whether the graph contains a cycle.
        /// </summary>
        bool IsCyclic { get; }

        /// <summary>
        /// Gets a graph transposition by reversing its edge directions.
        /// </summary>
        /// <returns>The transposed graph.</returns>
        IReadOnlyGraph<T> GetTransposition();

        /// <summary>
        /// Gets a transitively reduced graph.
        /// </summary>
        /// <returns>The transitively reduced graph.</returns>
        IReadOnlyGraph<T> GetTransitiveReduction();

        /// <summary>
        /// Gets a reflexively reduced graph.
        /// </summary>
        /// <returns>The reflexively reduced graph.</returns>
        IReadOnlyGraph<T> GetReflexiveReduction();

        /// <summary>
        /// Determines whether the current graph and the specified graph contain the same vertices and edges.
        /// </summary>
        /// <param name="other">The graph to compare to the current graph.</param>
        /// <returns><see langword="true"/> if the current graph is equal to other; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">other is <see langword="null"/>.</exception>
        bool GraphEquals(IReadOnlyGraph<T> other);
    }
}
