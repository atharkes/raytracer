using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Tests.Quality;
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

    [Test]
    [Timeout(30_000)]
    public async Task SpatialConstructor_ShouldKeepAllPrimitives(CancellationToken cancellationToken) {
        var primitives = RandomPrimitives(500);

        var sut = new SpatialBVH(primitives);

        var originals = Leaves(sut).Select(leaf => leaf is PrimitiveFragment fragment ? fragment.Original : leaf).Distinct();
        await Assert.That(originals.Count()).IsEqualTo(primitives.Count);
    }

    [Test]
    [Arguments(1f / 1024)]
    [Arguments(1024f)]
    public async Task Constructor_ShouldBuildTheSameTree_AtAnyScale(float scale) {
        static string Structure(BoundingVolumeHierarchy node) => node.Leaf ? node.ItemCount.ToString() : $"({Structure(node.Left!)} {Structure(node.Right!)})";
        var reference = new BoundingVolumeHierarchy(QualityScenes.RandomTriangles(2_000));

        var sut = new BoundingVolumeHierarchy(QualityScenes.RandomTriangles(2_000, 100f * scale, 2f * scale));

        await Assert.That(Structure(sut)).IsEqualTo(Structure(reference));
    }

    [Test]
    [Timeout(60_000)]
    public async Task SpatialConstructor_ShouldStopSplitting_WhenSliversOverlap(CancellationToken cancellationToken) {
        List<ISceneObject> slivers = [.. Enumerable.Range(0, 64).Select(i => new Primitive(
            new Triangle(new Position3(0, 0, i * .01f), new Position3(100, 100, i * .01f), new Position3(100, 100.01f, i * .01f + .01f)),
            Material.Diffuse(RGBColors.Gray)))];

        var sut = new SpatialBVH(slivers);

        var originals = Leaves(sut).Select(leaf => leaf is PrimitiveFragment fragment ? fragment.Original : leaf).Distinct();
        await Assert.That(originals.Count()).IsEqualTo(slivers.Count);
        await Assert.That(Depth(sut)).IsLessThanOrEqualTo(SpatialBVH.MaxSpatialSplitDepth + 16);
    }

    private static IEnumerable<ISceneObject> Leaves(BoundingVolumeHierarchy node) => node.Leaf
        ? node
        : Leaves(node.Left!).Concat(Leaves(node.Right!));
}
