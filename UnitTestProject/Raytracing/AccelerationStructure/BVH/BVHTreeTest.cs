using System.Collections.Generic;
using PathTracer.Raytracing.AccelerationStructures;
using PathTracer.Raytracing.AccelerationStructures.BVH;
using PathTracer.Raytracing.SceneObjects;
using PathTracer.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.Raytracing.AccelerationStructure.BVH {
    public class BVHTreeTest {
        readonly ITestOutputHelper output;

        public BVHTreeTest(ITestOutputHelper output) {
            this.output = output;
        }

        static BVHTree RandomBVH(int primitiveAmount) {
            List<Primitive> primitives = new List<Primitive>(primitiveAmount);
            for (int i = 0; i < primitiveAmount; i++) {
                primitives.Add(Utils.Random.Primitive(100f, 100f));
            }
            return new BVHTree(primitives);
        }

        [Fact]
        public void Constructor() {
            RandomBVH(1_000);
        }

        [Fact]
        public void NodeCount() {
            const int primitiveCount = 1_000;
            for (int i = 0; i < 10; i++) {
                BVHTree bvh = RandomBVH(primitiveCount);
                int nodeCount = CountNodes(bvh.Root);
                output.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
                Assert.True(nodeCount <= primitiveCount * 2);
            }

            int CountNodes(IBVHNode node) {
                if (node.Leaf) return 1;
                else return CountNodes(node.Left) + 1 + CountNodes(node.Right);
            }
        }
    }
}
