using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Raytracing.AccelerationStructures;
using PathTracer.Raytracing.AccelerationStructures.BVH;
using PathTracer.Raytracing.SceneObjects;
using PathTracer.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests.Raytracing.AccelerationStructure.SBVH {
    [TestClass]
    public class SBVHTreeTest {
        static SBVHTree RandomSBVH(int primitiveAmount) {
            List<Primitive> primitives = new List<Primitive>(primitiveAmount);
            for (int i = 0; i < primitiveAmount; i++) {
                primitives.Add(Utils.Random.Primitive(100f, 100f));
            }
            return new SBVHTree(primitives);
        }

        [TestMethod]
        public void Constructor() {
            RandomSBVH(1_000);
        }

        [TestMethod]
        public void NodeCount() {
            const int primitiveCount = 1_000;
            for (int i = 0; i < 10; i++) {
                SBVHTree bvh = RandomSBVH(primitiveCount);
                int nodeCount = CountNodes(bvh.Root);
                Debug.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
                Assert.IsTrue(nodeCount <= primitiveCount * 4);
            }

            static int CountNodes(IBVHNode node) {
                if (node.Leaf) return 1;
                else return CountNodes(node.Left) + 1 + CountNodes(node.Right);
            }
        }
    }
}
