using BenchmarkDotNet.Attributes;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Integrators;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Benchmarks.Benchmarks;

/// <summary> What the camera of the <see cref="PathTraceBenchmark"/> looks at </summary>
public enum PathView {
    /// <summary> Up into the sky, so most paths end at their first hit </summary>
    Sky,
    /// <summary> At the bunny and the floor, so most paths bounce between opaque surfaces </summary>
    Surfaces,
    /// <summary> Into the volumetric sphere, so most paths scatter inside it </summary>
    Volume,
}

/// <summary> Samples whole paths through the scene the application renders, on a single thread, for each <see cref="PathView"/> </summary>
[MemoryDiagnoser]
public class PathTraceBenchmark {
    public const int PathCount = 1_000;

    [ParamsAllValues]
    public PathView View { get; set; }

    private Scene scene = null!;
    private Random random = null!;

    /// <summary> Create the camera for a <paramref name="view"/> of <see cref="PathTracer.Program.RoughnessDensityComparison"/> </summary>
    /// <param name="view">The <see cref="PathView"/> to create the camera for</param>
    /// <returns>A <see cref="PinholeCamera"/> looking at the <paramref name="view"/></returns>
    public static PinholeCamera Camera(PathView view) {
        (Position3 position, Position3 target, float fov) = view switch {
            PathView.Sky => (new Position3(0, 1, 0), new Position3(0, 11, 5), 60f),
            PathView.Surfaces => (new Position3(-3, 1.6f, 4.5f), new Position3(-3, .6f, 6.5f), 60f),
            PathView.Volume => (new Position3(0, 1, 2.6f), new Position3(0, 1, 5), 40f),
            _ => throw new ArgumentOutOfRangeException(nameof(view)),
        };
        var camera = new PinholeCamera(position, OpenTK.Mathematics.Quaternion.Identity, 1.25f, fov);
        camera.SetViewDirection(new Normal3((target - position).Vector));
        return camera;
    }

    [GlobalSetup]
    public void Setup() {
        scene = new Scene(Camera(View), PathTracer.Program.RoughnessDensityComparison(2, 2));
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
