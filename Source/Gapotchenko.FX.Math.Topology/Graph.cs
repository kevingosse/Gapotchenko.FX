﻿using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Math.Topology
{
    /// <summary>
    /// <para>
    /// Represents a strongly-typed directional graph of objects.
    /// </para>
    /// <para>
    /// Graph is a set of vertices and edges.
    /// Vertices represent the objects, and edges represent the relations between them.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of vertices in the graph.</typeparam>
    [DebuggerDisplay("Order = {Vertices.Count}, Size = {Edges.Count}")]
    [DebuggerTypeProxy(typeof(GraphDebugView<>))]
    public partial class Graph<T> : IGraph<T>
        where T : notnull
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that is empty and uses the default equality comparer for graph vertices.
        /// </summary>
        public Graph() :
            this((IEqualityComparer<T>?)null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that is empty and uses the specified equality comparer for graph vertices.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEqualityComparer<T>? comparer)
        {
            m_AdjacencyList = new(comparer);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whose vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        public Graph(IReadOnlyGraph<T> graph) :
            this(graph, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices and edges copied from the specified <see cref="IReadOnlyGraph{T}"/>.
        /// </summary>
        /// <param name="graph">The <see cref="IReadOnlyGraph{T}"/> whose vertices and edges are copied to the new <see cref="Graph{T}"/>.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IReadOnlyGraph<T> graph, IEqualityComparer<T>? comparer) :
            this(comparer)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            Edges.UnionWith(graph.Edges);
            Vertices.UnionWith(graph.Vertices);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the default equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        public Graph(IEnumerable<T> vertices, GraphIncidenceFunction<T> incidenceFunction) :
            this(vertices, incidenceFunction, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        public Graph(IEnumerable<T> vertices, GraphIncidenceFunction<T> incidenceFunction, IEqualityComparer<T>? comparer) :
            this(vertices, incidenceFunction, comparer, GraphIncidenceOptions.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices
        /// and contains vertices copied from the specified collection
        /// and edges defined by the specified incidence function
        /// with the given options.
        /// </summary>
        /// <param name="vertices">The collection of graph vertices.</param>
        /// <param name="incidenceFunction">The graph incidence function.</param>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing vertices in the graph,
        /// or <see langword="null"/> to use the default <see cref="IEqualityComparer{T}"/> implementation.
        /// </param>
        /// <param name="options">The graph incidence options.</param>
        public Graph(IEnumerable<T> vertices, GraphIncidenceFunction<T> incidenceFunction, IEqualityComparer<T>? comparer, GraphIncidenceOptions options) :
            this(comparer)
        {
            if (vertices == null)
                throw new ArgumentNullException(nameof(vertices));
            if (incidenceFunction == null)
                throw new ArgumentNullException(nameof(incidenceFunction));

            bool reflexiveReducion = (options & GraphIncidenceOptions.ReflexiveReduction) != 0;

            var list = vertices.AsReadOnlyList();
            int count = list.Count;

            for (int i = 0; i < count; ++i)
            {
                var from = list[i];

                bool edge = false;
                for (int j = 0; j < count; ++j)
                {
                    if (reflexiveReducion && i == j)
                        continue;

                    var to = list[j];

                    if (incidenceFunction(from, to))
                    {
                        Edges.Add(from, to);
                        edge = true;
                    }
                }

                if (!edge)
                    Vertices.Add(from);
            }
        }

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of vertices for the graph.
        /// </summary>
        public IEqualityComparer<T> Comparer => m_AdjacencyList.Comparer;

        /// <summary>
        /// Graph adjacency row represents a set of vertices that relate to another vertex.
        /// </summary>
        protected internal sealed class AdjacencyRow : HashSet<T>
        {
            /// <summary>
            /// Initializes a new instance of <see cref="Graph{T}"/> class that uses the specified equality comparer for vertices.
            /// </summary>
            /// <param name="comparer">The comparer.</param>
            public AdjacencyRow(IEqualityComparer<T>? comparer) :
                base(comparer)
            {
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                var sb = new StringBuilder();
                sb.Append("{ ");

                bool first = true;
                foreach (var i in this)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(", ");

                    sb.Append(i);
                }

                sb.Append(" }");
                return sb.ToString();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Dictionary<T, AdjacencyRow?> m_AdjacencyList;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VertexSet? m_CachedVertices;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        VertexSet VerticesCore => m_CachedVertices ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISet<T> Vertices => VerticesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<T> IReadOnlyGraph<T>.Vertices => VerticesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EdgeSet? m_CachedEdges;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        EdgeSet EdgesCore => m_CachedEdges ??= new(this);

        /// <inheritdoc/>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ISet<GraphEdge<T>> Edges => EdgesCore;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlySet<GraphEdge<T>> IReadOnlyGraph<T>.Edges => EdgesCore;

        /// <summary>
        /// Cached number of vertices.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int? m_CachedOrder = 0;

        /// <summary>
        /// Cached number of edges.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int? m_CachedSize = 0;

        struct ReachibilityTraverser
        {
            public ReachibilityTraverser(Graph<T> graph, T destination, bool adjacent)
            {
                m_Graph = graph;
                m_Destination = destination;

                m_VisitedNodes = new HashSet<T>(graph.Comparer);
                m_Adjacent = adjacent;
            }

            readonly Graph<T> m_Graph;
            readonly T m_Destination;

            readonly HashSet<T> m_VisitedNodes;
            bool m_Adjacent;

            public bool CanBeReachedFrom(T source)
            {
                if (!m_VisitedNodes.Add(source))
                    return false;

                if (m_Graph.m_AdjacencyList.TryGetValue(source, out var adjRow) &&
                    adjRow != null)
                {
                    if (m_Adjacent)
                    {
                        if (adjRow.Contains(m_Destination))
                            return true;
                    }
                    else
                    {
                        m_Adjacent = true;
                    }

                    foreach (var i in adjRow)
                    {
                        if (CanBeReachedFrom(i))
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// <para>
        /// Gets a value indicating whether there is a transitive path from a specified source vertex to a destination.
        /// </para>
        /// <para>
        /// A transitive path consists of two or more edges with at least one intermediate vertex.
        /// </para>
        /// </summary>
        /// <param name="from">The source vertex.</param>
        /// <param name="to">The target vertex.</param>
        /// <returns><c>true</c> when the specified source vertex can reach the target via one or more intermediate vertices; otherwise, <c>false</c>.</returns>
        bool HasTransitivePath(T from, T to) => new ReachibilityTraverser(this, to, false).CanBeReachedFrom(from);

        /// <inheritdoc/>
        public bool HasPath(T from, T to) => Edges.Contains(from, to) || HasTransitivePath(from, to);

        /// <inheritdoc/>
        public bool IsVertexIsolated(T vertex)
        {
            var adjList = m_AdjacencyList;

            if (adjList.TryGetValue(vertex, out var adjRow) &&
                adjRow?.Count > 0)
            {
                return false;
            }

            foreach (var i in adjList)
            {
                adjRow = i.Value;
                if (adjRow == null)
                    continue;

                if (adjRow.Contains(vertex))
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            if (m_AdjacencyList.Count == 0)
                return;

            m_AdjacencyList.Clear();
            m_CachedOrder = 0;
            m_CachedSize = 0;

            IncrementVersion();
        }

        /// <inheritdoc/>
        public IEnumerable<T> VerticesAdjacentTo(T vertex)
        {
            var mg = new ModificationGuard(this);
            m_AdjacencyList.TryGetValue(vertex, out var adjRow);
            return mg.Protect(adjRow) ?? Enumerable.Empty<T>();
        }

        readonly struct ModificationGuard
        {
            public ModificationGuard(Graph<T> graph)
            {
                m_Graph = graph;
                m_Version = graph.m_Version;
            }

            readonly Graph<T> m_Graph;
            readonly int m_Version;

            [DoesNotReturn]
            public static void Throw() =>
                throw new InvalidOperationException("Graph was modified; enumeration operation may not execute.");

            public void Checkpoint()
            {
                if (m_Graph.m_Version != m_Version)
                    Throw();
            }

            [return: NotNullIfNotNull("source")]
            public IEnumerable<T>? Protect(IEnumerable<T>? source) => source == null ? null : ProtectCore(source);

            IEnumerable<T> ProtectCore(IEnumerable<T> source)
            {
                foreach (var i in source)
                {
                    Checkpoint();
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Creates a new graph instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new graph instance.</returns>
        protected Graph<T> NewGraph() => new(Comparer);

        /// <summary>
        /// <para>
        /// Gets the graph adjacency list.
        /// </para>
        /// <para>
        /// The list consists of a number of rows, each of them representing a set of vertices that relate to another vertex.
        /// </para>
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected internal IDictionary<T, AdjacencyRow?> AdjacencyList => m_AdjacencyList;

        /// <summary>
        /// Creates a new adjacency row instance inheriting parent class settings such as comparer.
        /// </summary>
        /// <returns>The new adjacency row instance.</returns>
        protected AdjacencyRow NewAdjacencyRow() => new(Comparer);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int m_Version;

        void IncrementVersion() => ++m_Version;

        /// <summary>
        /// Invalidates the cache.
        /// This method should be called if <see cref="AdjacencyList"/> is manipulated directly.
        /// </summary>
        protected void InvalidateCache()
        {
            m_CachedOrder = null;
            m_CachedSize = null;
            IncrementVersion();
        }
    }
}
