﻿# Gapotchenko.FX.Math.Topology

[![License](https://img.shields.io/badge/license-MIT-green.svg)](../../LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Gapotchenko.FX.Math.Topology.svg)](https://www.nuget.org/packages/Gapotchenko.FX.Math.Topology)

The module provides data structures and primitives for working with abstract topologies.

In its classical meaning, the science of topology is concerned with the properties of geometric objects.
However, such a definition may be considered as too narrow by some.
Math has no boundaries,
so topology should not be viewed as limited to just one unique flavor of objects (geometric, physical); it can really be anything.

## Graph

### Basics

`Graph<T>` provided by `Gapotchenko.FX.Math.Topology` represents a strongly-typed directional graph of objects.
The objects correspond to mathematical abstractions called graph vertices and each of the related pairs of vertices is called an edge.
A graph can be viewed as a structure that contains two sets: set of vertices and set of edges.
Vertices define "what" graph contains and edges define "how" those vertices are connected.

Let's take a look at the simplest graph that contains just two vertices:

``` c#
using Gapotchenko.FX.Math.Topology;

var g = new Graph<int>
{
    Vertices = { 1, 2 }
};
```

If we could visualize that graph then it would look like this:

![Simple graph with two isolated vertices](../../Documentation/Assets/Math/Topology/simple-graph-2-0.svg?raw=true)

Now let's add one more vertex `3` plus an edge that goes from vertex `1` to vertex `2`:

``` c#
var g = new Graph<int>
{
    Vertices = { 1, 2, 3 },
    Edges = { (1, 2) }  // <-- an edge has (from, to) notation
};
```

Our new graph looks like this:

![Simple graph with three vertices and one edge](../../Documentation/Assets/Math/Topology/simple-graph-3-1.svg?raw=true)

The vertices already defined in edges can be omitted for brevity:

``` c#
var h = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

Console.WriteLine(g.GraphEquals(h)); // will print "True"
```

It is worth mentioning that the graph provides its vertices as an `ISet<T>`, so the usual operations on a set apply to the vertices as well:

``` c#
var g = new Graph<int>
{
    Vertices = { 3 },
    Edges = { (1, 2) }
};

g.Vertices.UnionWith(new[] { 3, 4, 5 });
```

The example above produces the following graph:

![Simple graph with five vertices and one edge](../../Documentation/Assets/Math/Topology/simple-graph-5-1.svg?raw=true)

The same `ISet<T>` model applies to the graph edges: they are treated as a set too.

### Operations

Now once we have the basics in place, let's take a look at graph operations.
Consider the graph:

``` c#
var g = new Graph<int>
{
    Edges =
    {
        (7, 5), (7, 6),
        (6, 3), (6, 4),
        (5, 2), (5, 4),
        (3, 1),
        (2, 1),
        (1, 0)
    }
};
```

which looks like this:

![Graph with eight vertices and nine edges](../../Documentation/Assets/Math/Topology/graph-8-9.svg?raw=true)

Let's transpose the graph (i.e. reverse the direction of its edges):

``` c#
var h = g.GetTransposition();
```

Transposed graph `h` renders as:

![Transposed graph with eight vertices and nine edges](../../Documentation/Assets/Math/Topology/graph-8-9-t.svg?raw=true)

Note that graph `h` is a new instance of `Graph<T>`.

But what if we want to transpose the graph `g` in place?
Every graph operation has a corresponding in-place variant, so for transposition it will be:

``` c#
g.Transpose();
```

In this way, a developer can freely choose between immutable, mutable, or combined data models when working on a particular task at hand.

Graph transposition is just one example but there are plenty of other operations available.
They all work in the same manner and follow the same model:

| Operation | Description | Immutable Function | In-place Method |
| --- | --- | --- | --- |
| Transposition | Reverses the direction of edges | `GetTransposition` | `Transpose` |
| Transitive reduction | Prunes the transitive relations that have shorter paths | `GetTransitiveReduction` | `ReduceTransitions` |
| Reflexive reduction | Removes the loops (also called self-loops or buckles) | `GetReflexiveReduction` | `ReduceReflexes` |
| Subgraph | Produces a vertex-induced or edge-induced subgraph | `GetSubgraph` | `Subgraph` |
| Intersect | Produces a graph containing vertices and edges that are present in both the current and a specified graphs | `Intersect` | `IntersectWith` |
| Union | Produces a graph containing all vertices and edges that are present in the current graph, in the specified graph, or in both | `Union` | `UnionWith` |
| Except | Produces a  graph containing vertices and edges that are present in the current graph but not in the specified graph | `Except` | `ExceptWith` |

### Topological Sorting

Topological sort of a graph is a linear ordering of its vertices such that
for every directed edge `u` → `v`,
`u` comes before `v` in the ordering.

The canonical application of topological sorting is in scheduling a sequence of jobs or tasks based on their dependencies.
The jobs are represented by vertices, and there is an edge from `x` to `y` if job `x` must be completed before job `y` can be started.
Then, a topological sort gives an order in which to perform the jobs.

Let's take a look at example graph:

![Graph with eight vertices and nine edges](../../Documentation/Assets/Math/Topology/graph-8-9.svg?raw=true)

Let's assume that vertices represent the jobs, and edges define their dependencies.
In this way, job `0` depends on job `1` and thus cannot be started unless job `1` is finished.
In turn, job `1` cannot be started unless jobs `2` and `3` are finished, and so on.
In what order should the jobs be executed?

To answer that question, let's use `OrderTopologically` method:

``` c#
using Gapotchenko.FX.Math.Topology;

var g = new Graph<int>
{
    Edges =
    {
        (7, 5), (7, 6),
        (6, 3), (6, 4),
        (5, 2), (5, 4),
        (3, 1),
        (2, 1),
        (1, 0)
    }
};

var ordering = g.OrderTopologically();
Console.WriteLine(string.Join(", ", ordering));
```

The result is:

```
7, 5, 6, 2, 3, 4, 1, 0
```

`OrderTopologically` method works on directed acyclic graphs only.
If a graph is cyclic, then `CircularDependencyException` is raised.

### Stable Topological Sorting of a Graph

Graph is a data structure similar to a set: it does not guarantee to preserve the order in which the elements were added.
As a result, topological sorting may return different orderings for otherwise equal graphs.

To overcome that limitation, it may be beneficial to use topological sorting with subsequent ordering by some other criteria.
Such approach makes the topological sorting stable.
It can be achieved by leveraging the standard `IOrderedEnumerable<T>` LINQ semantics of the operation, like so:

``` c#
g.OrderTopologically().ThenBy(…)
```

### Stable Topological Sorting of a Sequence

Sorting a sequence of elements in topological order is another play on topological sorting idea.

Say we have a sequence of elements `{A, B, C, D, E, F}`. Some elements depend on others:

- A depends on B
- B depends on D

Objective: sort the sequence so that its elements are ordered according to their dependencies.
The resulting sequence should have a minimal edit distance to the original one.
In other words, sequence should be topologically sorted while preserving the original order of elements whenever it is possible.

`Gapotchenko.FX.Math.Topology` provides an extension method for `IEnumerable<T>` that allows to achieve that:

``` c#
using Gapotchenko.FX.Math.Topology;

string seq = "ABCDEF";

// Dependency function.
static bool df(char a, char b) =>
    (a + " depends on " + b) switch
    {
        "A depends on B" or
        "B depends on D" => true,
        _ => false
    };

var ordering = seq.OrderTopologicallyBy(x => x, df);
Console.WriteLine(string.Join(", ", ordering));  // <- prints "D, B, A, C, E, F"
```

Unlike its graph sibling, `OrderTopologicallyBy` method tolerates circular dependencies by ignoring them.
They are resolved according to the original order of elements in the sequence.

`OrderTopologicallyBy` method allows subseqent sorting by following the standard `IOrderedEnumerable<T>` LINQ convention:

``` c#
seq.OrderTopologicallyBy(…).ThenBy(…)
```

## Usage

`Gapotchenko.FX.Math.Topology` module is available as a [NuGet package](https://nuget.org/packages/Gapotchenko.FX.Math.Topology):

```
PM> Install-Package Gapotchenko.FX.Math.Topology
```

## Other Modules

Let's continue with a look at some other modules provided by Gapotchenko.FX:

- [Gapotchenko.FX](../Gapotchenko.FX)
- [Gapotchenko.FX.AppModel.Information](../Gapotchenko.FX.AppModel.Information)
- [Gapotchenko.FX.Collections](../Gapotchenko.FX.Collections)
- [Gapotchenko.FX.Console](../Gapotchenko.FX.Console)
- [Gapotchenko.FX.Diagnostics](../Gapotchenko.FX.Diagnostics.CommandLine)
- [Gapotchenko.FX.IO](../Gapotchenko.FX.IO)
- [Gapotchenko.FX.Linq](../Gapotchenko.FX.Linq)
- [Gapotchenko.FX.Math](../Gapotchenko.FX.Math)
  - [Gapotchenko.FX.Math.Combinatorics](../Gapotchenko.FX.Math.Combinatorics)
  - [Gapotchenko.FX.Math.Geometry](../Gapotchenko.FX.Math.Geometry)
  - &#x27B4; [Gapotchenko.FX.Math.Topology](../Gapotchenko.FX.Math.Topology)
- [Gapotchenko.FX.Memory](../Gapotchenko.FX.Memory)
- [Gapotchenko.FX.Text](../Gapotchenko.FX.Text)
- [Gapotchenko.FX.Threading](../Gapotchenko.FX.Threading)

Or look at the [full list of modules](..#available-modules).
