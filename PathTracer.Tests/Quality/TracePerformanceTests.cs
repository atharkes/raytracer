using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;

namespace PathTracer.Tests.Quality;

/// <summary> Reports the wall clock time and spread of tracing rays through fixed scenes. Unlike <see cref="HierarchyQualityTests"/> this includes
/// the intersection code and allocations, and depends on the machine and build configuration, so it reports without failing.
/// Compare runs on the same machine, preferably in Release: dotnet run --project PathTracer.Tests -c Release -- --treenode-filter "/*/*/TracePerformanceTests/*" --output Detailed </summary>
public class TracePerformanceTests {
    [Test]
    public async Task Bvh_Bunny() => await Measure(new BoundingVolumeHierarchy(QualityScenes.Bunny));

    [Test]
    public async Task Bvh_RandomTriangles() => await Measure(new BoundingVolumeHierarchy(QualityScenes.RandomTriangles(10_000)));

    [Test]
    [Timeout(120_000)]
    public async Task Sbvh_RandomTriangles(CancellationToken cancellationToken) => await Measure(new SpatialBVH(QualityScenes.RandomTriangles(2_000)));

    private static async Task Measure(BoundingVolumeHierarchy root) {
        var rays = HierarchyMetrics.Rays(root.Shape.BoundingBox, 2_000).ToList();

        var timing = TraceTiming.Measure(root, rays);

        Console.WriteLine(timing);
        await Assert.That(timing.MeanMicroseconds).IsGreaterThan(0);
    }
}
