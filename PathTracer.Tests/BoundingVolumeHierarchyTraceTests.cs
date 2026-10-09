using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.DistanceQuery;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using PathTracer.Pathtracing.SceneDescription.Shapes.Volumetrics;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Tests.Quality;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Tests;

public class BoundingVolumeHierarchyTraceTests {
    [Test]
    public async Task Trace_ShouldMatchTracingEveryPrimitive() {
        var random = new Random(1);
        List<ISceneObject> primitives = [
            .. Enumerable.Range(0, 400).Select(_ => random.Primitive(100f, 5f)),
            .. Enumerable.Range(0, 100).Select(_ => new Primitive(random.Shape(100f, 10f), Material.IsotropicVolumetric(RGBColors.Gray, .05f))),
        ];
        var flat = new Aggregate(primitives);
        var sut = new BoundingVolumeHierarchy(primitives);

        for (var i = 0; i < 1_000; i++) {
            var origin = new Position3(random.NextSingle() * 300 - 100, random.NextSingle() * 300 - 100, random.NextSingle() * 300 - 100);
            var target = new Position3(random.NextSingle() * 100, random.NextSingle() * 100, random.NextSingle() * 100);
            var ray = new Ray(origin, new Normal3((target - origin).Vector));

            var expected = flat.Trace(ray, RGBSpectrum.White);
            var actual = sut.Trace(ray, RGBSpectrum.White);

            await Assert.That(actual is null).IsEqualTo(expected is null);
            if (expected is null || actual is null) continue;
            var entry = (float)expected.DistanceDistribution.Domain.Entry;
            await Assert.That((float)actual.DistanceDistribution.Domain.Entry).IsEqualTo(entry);
            foreach (var offset in new[] { 0f, .5f, 2f, 10f, 50f, 500f }) {
                await Assert.That(actual.DistanceDistribution.CumulativeProbability(entry + offset))
                    .IsEqualTo(expected.DistanceDistribution.CumulativeProbability(entry + offset)).Within(1e-6);
            }
        }
    }

    [Test]
    public async Task Trace_ShouldSkipPrimitivesBehindAnOpaqueHit() {
        var boxes = Enumerable.Range(0, 64)
            .Select(i => new CountingSceneObject(new Primitive(new AxisAlignedBox(new Position3(i * 2, 0, 0), new Position3(i * 2 + 1, 1, 1)), Material.Diffuse(RGBColors.Gray))))
            .ToList();
        var sut = new BoundingVolumeHierarchy(boxes);
        var ray = new Ray(new Position3(-1, .5f, .5f), Normal3.UnitX);

        var result = sut.Trace(ray, RGBSpectrum.White);

        await Assert.That((float)result!.DistanceDistribution.Domain.Entry).IsEqualTo(1f).Within(0.001f);
        await Assert.That(boxes.Sum(b => b.Counter.Count)).IsLessThan(8);
    }
}
