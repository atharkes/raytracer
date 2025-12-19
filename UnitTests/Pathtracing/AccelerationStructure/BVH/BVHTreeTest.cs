using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;
using System.Diagnostics;

namespace UnitTests.Pathtracing.AccelerationStructure.BVH;

[TestClass]
public class BVHTreeTest {
    private static BoundingVolumeHierarchy RandomBVH(int primitiveAmount) {
        List<ISceneObject> sceneObjects = new(primitiveAmount);
        for (var i = 0; i < primitiveAmount; i++) {
            sceneObjects.Add(Utils.ThreadRandom.Primitive(100f, 100f));
        }
        return new BoundingVolumeHierarchy(sceneObjects);
    }

    [TestMethod]
    public void Constructor() => RandomBVH(1_000);

    [TestMethod]
    public void NodeCount() {
        const int primitiveCount = 1_000;
        for (var i = 0; i < 10; i++) {
            var bvh = RandomBVH(primitiveCount);
            var nodeCount = CountNodes(bvh);
            Debug.WriteLine($"Node count in percentage of primitives: {(float)nodeCount / primitiveCount}");
            Assert.IsLessThanOrEqualTo(primitiveCount * 2, nodeCount);
        }

        static int CountNodes(IBinaryTree? node) => node == null ? 0 : node.Leaf ? 1 : CountNodes(node.Left) + 1 + CountNodes(node.Right);
    }
}
