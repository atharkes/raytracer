﻿using System.Collections.Generic;
using WhittedRaytracer.Raytracing.AccelerationStructures;
using WhittedRaytracer.Raytracing.AccelerationStructures.BVH;
using WhittedRaytracer.Raytracing.SceneObjects;
using WhittedRaytracer.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Raytracing.AccelerationStructure.SBVH {
    public class SBVHTreeTest {
        readonly ITestOutputHelper output;

        public SBVHTreeTest(ITestOutputHelper output) {
            this.output = output;
        }

        static SBVHTree RandomSBVH(int primitiveAmount) {
            List<Primitive> primitives = new List<Primitive>(primitiveAmount);
            for (int i = 0; i < primitiveAmount; i++) {
                primitives.Add(Utils.Random.Primitive(100f, 100f));
            }
            return new SBVHTree(primitives);
        }

        [Fact]
        public void Constructor() {
            RandomSBVH(1_000);
        }

        [Fact]
        public void NodeCount() {
            const int primitiveCount = 1_000;
            for (int i = 0; i < 10; i++) {
                SBVHTree bvh = RandomSBVH(primitiveCount);
                int nodeCount = CountNodes(bvh.Root);
                output.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
                Assert.True(nodeCount <= primitiveCount * 4);
            }

            int CountNodes(IBVHNode node) {
                if (node.Leaf) return 1;
                else return CountNodes(node.Left) + 1 + CountNodes(node.Right);
            }
        }
    }
}
