﻿using Gapotchenko.FX.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gapotchenko.FX.Math.Combinatorics
{
    /// <summary>
    /// Defines Cartesian product operations.
    /// </summary>
    public static class CartesianProduct
    {
        /// <summary>
        /// Calculates Cartesian product cardinality for the specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>Calculated Cartesian product cardinality for the specified factor lengths.</returns>
        public static int Cardinality(IEnumerable<int> factorLengths)
        {
            if (factorLengths == null)
                throw new ArgumentNullException(nameof(factorLengths));

            bool hasFactor = false;
            int cardinality = 1;

            foreach (var length in factorLengths)
            {
                if (length == 0)
                    return 0;

                cardinality *= length;
                hasFactor = true;
            }

            if (!hasFactor)
                return 0;

            return cardinality;
        }

        /// <summary>
        /// Calculates Cartesian product cardinality for the specified factor lengths.
        /// </summary>
        /// <param name="factorLengths">The factor lengths.</param>
        /// <returns>Calculated Cartesian product cardinality for specified factor lengths.</returns>
        public static int Cardinality(params int[] factorLengths) => Cardinality((IEnumerable<int>)factorLengths);

        /// <summary>
        /// Generates Cartesian product of the specified factors.
        /// </summary>
        /// <typeparam name="T">The factor type.</typeparam>
        /// <param name="factors">The factors.</param>
        /// <returns>Cartesian product of the specified factors.</returns>
        public static IEnumerable<IEnumerable<T>> Of<T>(IEnumerable<IEnumerable<T>> factors)
        {
            if (factors == null)
                throw new ArgumentNullException(nameof(factors));

            var sourceArray = factors.AsReadOnly();
            if (sourceArray.Count == 0)
                yield break;

            var enumerators = sourceArray.Select(x => x.GetEnumerator()).ToArray();

            int n = enumerators.Length;

            foreach (var i in enumerators)
            {
                if (!i.MoveNext())
                    yield break;
            }

            for (; ; )
            {
                var result = new T[n];
                for (int i = 0; i != n; i++)
                    result[i] = enumerators[i].Current;

                yield return result;

                for (int i = 0; i != n; i++)
                {
                    var enumerator = enumerators[i];
                    if (enumerator.MoveNext())
                        break;

                    var newEnumerator = sourceArray[i].GetEnumerator();
                    if (!newEnumerator.MoveNext())
                        throw new InvalidOperationException("Cartesian product pool has been emptied unexpectedly.");

                    enumerators[i] = newEnumerator;

                    if (i == n - 1)
                        yield break;
                }
            }
        }

        /// <summary>
        /// Generates Cartesian product of specified factors.
        /// </summary>
        /// <typeparam name="T">Type of factor items.</typeparam>
        /// <param name="factors">The factors.</param>
        /// <returns>Cartesian product of specified factors.</returns>
        public static IEnumerable<IEnumerable<T>> Of<T>(params IEnumerable<T>[] factors) => Of((IEnumerable<IEnumerable<T>>)factors);
    }
}