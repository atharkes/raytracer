using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.SBVH;

namespace PathTracer.Tests.Quality;

/// <summary> Measures hierarchy quality on fixed scenes, printing the metrics so changes can be compared, and failing when quality regresses.
/// The limits sit about 10% above the values measured when they were set; lower them when a change improves on them. </summary>
public class HierarchyQualityTests {
    [Test]
    public async Task Bvh_Bunny() {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.Bunny, primitives => new BoundingVolumeHierarchy(primitives)));

        await Assert.That(metrics.References).IsEqualTo(metrics.Primitives);
        await Assert.That(metrics.SahCost).IsLessThan(36);
        await Assert.That(metrics.PrimitiveTestsPerRay).IsLessThan(3);
        await Assert.That(metrics.BoxTestsPerRay).IsLessThan(41);
        await Assert.That(metrics.MaxLeafSize).IsLessThanOrEqualTo(8);
    }

    [Test]
    public async Task Bvh_RandomTriangles() {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.RandomTriangles(10_000), primitives => new BoundingVolumeHierarchy(primitives)));

        await Assert.That(metrics.References).IsEqualTo(metrics.Primitives);
        await Assert.That(metrics.SahCost).IsLessThan(66);
        await Assert.That(metrics.PrimitiveTestsPerRay).IsLessThan(2.5);
        await Assert.That(metrics.BoxTestsPerRay).IsLessThan(147);
    }

    [Test]
    [Timeout(120_000)]
    public async Task Sbvh_RandomTriangles(CancellationToken cancellationToken) {
        var metrics = Report(HierarchyMetrics.Measure(QualityScenes.RandomTriangles(2_000), primitives => new SpatialBVH(primitives)));

        await Assert.That(metrics.References).IsGreaterThanOrEqualTo(metrics.Primitives);
        await Assert.That(metrics.References).IsLessThan(metrics.Primitives * 11 / 10);
        await Assert.That(metrics.SahCost).IsLessThan(32);
        await Assert.That(metrics.BoxTestsPerRay).IsLessThan(83);
    }

    private static HierarchyMetrics Report(HierarchyMetrics metrics) {
        Console.WriteLine(metrics);
        return metrics;
    }
}
