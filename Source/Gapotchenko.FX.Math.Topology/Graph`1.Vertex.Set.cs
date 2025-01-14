﻿using Gapotchenko.FX.Collections.Generic.Kit;
using Gapotchenko.FX.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gapotchenko.FX.Math.Topology
{
    partial class Graph<TVertex>
    {
        /// <summary>
        /// Represents a set of graph vertices.
        /// </summary>
        public sealed class VertexSet : SetBase<TVertex>
        {
            internal VertexSet(Graph<TVertex> graph)
            {
                m_Graph = graph;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            readonly Graph<TVertex> m_Graph;

            /// <inheritdoc/>
            public override IEqualityComparer<TVertex> Comparer => m_Graph.VertexComparer;

            /// <inheritdoc/>
            public override int Count => m_Graph.m_CachedOrder ??= GetEnumerator().Rest().Count();

            /// <inheritdoc/>
            public override bool Add(TVertex vertex)
            {
                if (Contains(vertex))
                    return false;
                m_Graph.m_AdjacencyList.Add(vertex, null);
                ++m_Graph.m_CachedOrder;
                m_Graph.IncrementVersion();
                return true;
            }

            /// <inheritdoc/>
            public override bool Remove(TVertex vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;

                bool hit = adjacencyList.Remove(vertex);

                foreach (var i in adjacencyList)
                {
                    var adjacencyRow = i.Value;
                    if (adjacencyRow != null)
                        hit |= adjacencyRow.Remove(vertex);
                }

                if (hit)
                {
                    --m_Graph.m_CachedOrder;
                    m_Graph.m_CachedSize = null;
                    m_Graph.InvalidateCachedRelations();
                    m_Graph.IncrementVersion();
                }

                return hit;
            }

            /// <inheritdoc/>
            public override void Clear() => m_Graph.Clear();

            /// <inheritdoc/>
            public override bool Contains(TVertex vertex)
            {
                var adjacencyList = m_Graph.m_AdjacencyList;
                return
                    adjacencyList.ContainsKey(vertex) ||
                    adjacencyList.Any(x => x.Value?.Contains(vertex) ?? false);
            }

            /// <inheritdoc/>
            public override IEnumerator<TVertex> GetEnumerator()
            {
                var version = m_Graph.m_Version;

                var query = m_Graph.m_AdjacencyList
                    .SelectMany(x => (x.Value ?? Enumerable.Empty<TVertex>()).Prepend(x.Key))
                    .Distinct(m_Graph.VertexComparer);

                foreach (var i in query)
                {
                    if (m_Graph.m_Version != version)
                        ModificationGuard.Throw();

                    yield return i;
                }
            }
        }
    }
}
