using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Pathtracing.AccelerationStructures;
using PathTracer.Pathtracing.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests.Pathtracing.AccelerationStructure.BVH {
    [TestClass]
    public class BVHTreeTest {
        static BVHTree RandomBVH(int primitiveAmount) {
            List<Shape> primitives = new List<Shape>(primitiveAmount);
            for (int i = 0; i < primitiveAmount; i++) {
                primitives.Add(Utils.Random.Primitive(100f, 100f));
            }
            return new BVHTree(primitives);
        }

        [TestMethod]
        public void Constructor() {
            RandomBVH(1_000);
        }

        [TestMethod]
        public void NodeCount() {
            const int primitiveCount = 1_000;
            for (int i = 0; i < 10; i++) {
                BVHTree bvh = RandomBVH(primitiveCount);
                int nodeCount = CountNodes(bvh.Root);
                Debug.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
                Assert.IsTrue(nodeCount <= primitiveCount * 2);
            }

            static int CountNodes(IBinaryTree? node) {
                if (node == null) return 0;
                else if (node.Leaf) return 1;
                else return CountNodes(node.Left) + 1 + CountNodes(node.Right);
            }
        }
    }
}
