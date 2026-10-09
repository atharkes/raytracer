using BenchmarkDotNet.Attributes;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates.AccelerationStructures.BVH;

namespace PathTracer.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class BvhBuildBenchmark {
    private List<ISceneObject> primitives = null!;

    [GlobalSetup]
    public void Setup() => primitives = Bunny.Load();

    [Benchmark]
    public BoundingVolumeHierarchy BuildBunny() => new(primitives);
}
