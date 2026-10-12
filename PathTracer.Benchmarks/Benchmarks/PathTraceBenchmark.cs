using BenchmarkDotNet.Attributes;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Integrators;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;

namespace PathTracer.Benchmarks.Benchmarks;

/// <summary> Samples whole paths through the scene the application renders, from the default camera, on a single thread </summary>
[MemoryDiagnoser]
public class PathTraceBenchmark {
    public const int PathCount = 1_000;

    private Scene scene = null!;
    private Random random = null!;

    [GlobalSetup]
    public void Setup() {
        var config = new Config();
        var camera = new PinholeCamera(config.Position, config.Rotation, config.AspectRatio, config.HorizontalFOV);
        scene = new Scene(camera, PathTracer.Program.RoughnessDensityComparison(2, 2));
        random = new Random(7);
    }

    [Benchmark(OperationsPerInvoke = PathCount)]
    public RGBSpectrum SamplePaths() {
        var light = RGBSpectrum.Black;
        for (var i = 0; i < PathCount; i++) {
            var position = new Position2(random.NextSingle(), random.NextSingle());
            var direction = new Direction2(random.NextSingle(), random.NextSingle());
            light += BackwardsSampler.Sample(scene, scene.Camera.GetCameraRay(position, direction), random);
        }
        return light;
    }
}
