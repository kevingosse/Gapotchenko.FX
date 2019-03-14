﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gapotchenko.FX.Collections.Generic
{
    /// <summary>
    /// Generic list extensions.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Performs in-place stable sort of the elements in entire <see cref="List{T}"/> using the default comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list.</param>
        public static void StableSort<T>(this List<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count < 2)
                return;

            var sortedList = list.OrderBy(Empty.IdentityFunc).ToList();

            list.Clear();
            list.AddRange(sortedList);
        }

        /// <summary>
        /// Clones the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>A cloned list.</returns>
        public static List<T> Clone<T>(this List<T> list)
        {
            if (list == null)
                return null;
            else
                return new List<T>(list);
        }
    }
}