using BenchmarkDotNet.Attributes;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BvhTraceBenchmark {
    public const int RayCount = 1_000;

    private BoundingVolumeHierarchy bvh = null!;
    private Ray[] rays = null!;

    [GlobalSetup]
    public void Setup() {
        bvh = new BoundingVolumeHierarchy(Bunny.Load());
        rays = Bunny.Rays(bvh.Shape.BoundingBox, RayCount);
    }

    [Benchmark(OperationsPerInvoke = RayCount)]
    public int TraceBunny() {
        var hits = 0;
        foreach (var ray in rays) {
            if (bvh.Trace(ray, RGBSpectrum.White) is not null) hits++;
        }
        return hits;
    }
}
