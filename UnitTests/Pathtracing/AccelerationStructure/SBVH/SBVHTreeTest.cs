using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;
using System.Diagnostics;

namespace UnitTests.Pathtracing.AccelerationStructure.SBVH;

[TestClass]
public class SBVHTreeTest {
    private static SpatialBVH RandomSBVH(int primitiveAmount) {
        var sceneObjects = new List<ISceneObject>(primitiveAmount);
        for (var i = 0; i < primitiveAmount; i++) {
            sceneObjects.Add(Utils.ThreadRandom.Primitive(100f, 100f));
        }
        return new SpatialBVH(sceneObjects);
    }

    [TestMethod]
    public void Constructor() => RandomSBVH(1_000);

    [TestMethod]
    public void NodeCount() {
        const int primitiveCount = 1_000;
        for (var i = 0; i < 10; i++) {
            var sbvh = RandomSBVH(primitiveCount);
            var nodeCount = CountNodes(sbvh);
            Debug.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
            Assert.IsLessThanOrEqualTo(primitiveCount * 4, nodeCount);
        }

        static int CountNodes(IBinaryTree? node) => node == null ? 0 : node.Leaf ? 1 : CountNodes(node.Left) + 1 + CountNodes(node.Right);
    }
}
