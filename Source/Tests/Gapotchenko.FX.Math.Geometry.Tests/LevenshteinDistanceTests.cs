﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Math.Geometry.Tests
{
    [TestClass]
    public class LevenshteinDistanceTests
    {
        [TestMethod]
        public void LevenshteinDistance_Basics()
        {
            Assert.AreEqual(4, EditDistance("abra", ""));
            Assert.AreEqual(4, EditDistance("", "abra"));
            Assert.AreEqual(0, EditDistance("abra", "abra"));
            Assert.AreEqual(0, EditDistance("", ""));

            Assert.AreEqual(1, EditDistance("abr", "abra"));
            Assert.AreEqual(1, EditDistance("abra", "abr"));
            Assert.AreEqual(1, EditDistance("abra", "abrr"));
        }

        [TestMethod]
        public void LevenshteinDistance_MaxDistance()
        {
            for (var maxDistance = 0; maxDistance <= 16; ++maxDistance)
            {
                Assert.AreEqual(
                    maxDistance,
                    EditDistance("abcdefghijklmnop", "ponmlkjihgfedcba", maxDistance));
            }
        }

        static int EditDistance(string a, string b, int? maxDistance = default)
        {
            return StringMetrics.LevenshteinDistance(a, b, maxDistance);
        }
    }
}
