using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;

namespace PathTracer.Tests.Quality;

/// <summary> Measures hierarchy quality on fixed scenes, printing the metrics so changes can be compared, and failing when quality regresses </summary>
public class HierarchyQualityTests {
    [Test]
    public async Task Bvh_Bunny() {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.Bunny, primitives => new BoundingVolumeHierarchy(primitives)));

        await Assert.That(metrics.References).IsEqualTo(metrics.Primitives);
    }

    [Test]
    public async Task Bvh_RandomTriangles() {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.RandomTriangles(10_000), primitives => new BoundingVolumeHierarchy(primitives)));

        await Assert.That(metrics.References).IsEqualTo(metrics.Primitives);
    }

    [Test]
    [Timeout(120_000)]
    public async Task Sbvh_RandomTriangles(CancellationToken cancellationToken) {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.RandomTriangles(2_000), primitives => new SpatialBVH(primitives)));

        await Assert.That(metrics.References).IsGreaterThanOrEqualTo(metrics.Primitives);
    }

    private static HierarchyMetrics Report(HierarchyMetrics metrics) {
        Console.WriteLine(metrics);
        return metrics;
    }
}
