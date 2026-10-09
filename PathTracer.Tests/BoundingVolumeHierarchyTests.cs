using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Tests;

public class BoundingVolumeHierarchyTests {
    private static List<ISceneObject> RandomPrimitives(int amount) => [.. Enumerable.Range(0, amount).Select(_ => Random.Shared.Primitive(100f, 1f))];

    private static int Depth(IBinaryTree? node) => node is null || node.Leaf ? 1 : 1 + Math.Max(Depth(node.Left), Depth(node.Right));

    [Test]
    public async Task Constructor_ShouldSplitRecursively() {
        var sut = new BoundingVolumeHierarchy(RandomPrimitives(1_000));

        await Assert.That(Depth(sut)).IsGreaterThan(2);
    }

    [Test]
    public async Task Constructor_ShouldKeepAllPrimitives() {
        var primitives = RandomPrimitives(1_000);

        var sut = new BoundingVolumeHierarchy(primitives);

        await Assert.That(Leaves(sut).Distinct().Count()).IsEqualTo(primitives.Count);
    }

    [Test]
    [Timeout(30_000)]
    public async Task SpatialConstructor_ShouldSplitRecursively(CancellationToken cancellationToken) {
        var sut = new SpatialBVH(RandomPrimitives(1_000));

        await Assert.That(Depth(sut)).IsGreaterThan(2);
    }

    private static IEnumerable<ISceneObject> Leaves(BoundingVolumeHierarchy node) => node.Leaf
        ? node
        : Leaves(node.Left!).Concat(Leaves(node.Right!));
}
