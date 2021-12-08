using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests.Pathtracing.AccelerationStructure.SBVH {
    [TestClass]
    public class SBVHTreeTest {
        static SpatialBVH RandomSBVH(int primitiveAmount) {
            List<ISceneObject> sceneObjects = new(primitiveAmount);
            for (int i = 0; i < primitiveAmount; i++) {
                sceneObjects.Add(Utils.ThreadRandom.Primitive(100f, 100f));
            }
            return new SpatialBVH(sceneObjects);
        }

        [TestMethod]
        public void Constructor() {
            RandomSBVH(1_000);
        }

        [TestMethod]
        public void NodeCount() {
            const int primitiveCount = 1_000;
            for (int i = 0; i < 10; i++) {
                SpatialBVH sbvh = RandomSBVH(primitiveCount);
                int nodeCount = CountNodes(sbvh);
                Debug.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
                Assert.IsTrue(nodeCount <= primitiveCount * 4);
            }

            static int CountNodes(IBinaryTree? node) {
                if (node == null) return 0;
                else if (node.Leaf) return 1;
                else return CountNodes(node.Left) + 1 + CountNodes(node.Right);
            }
        }
    }
}
