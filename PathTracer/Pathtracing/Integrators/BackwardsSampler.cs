using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Multithreading;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.Integrators;

/// <summary> An <see cref="Integrator"/> that samples from the <see cref="ICamera"/> </summary>
public class BackwardsSampler : IIntegrator {
    /// <summary> The minimum amount of samples per integration cycle of the <see cref="BackwardsSampler"/> </summary>
    public int MinimumSampleCount => threadpool.MultithreadingTaskCount * 10;
    /// <summary> The maximum recursion depth for sampling </summary>
    public const int GauranteedRecursionDepth = 16;
    /// <summary> The chance of russian roulette when the gauranteed recursion depth is exceeded </summary>
    public const float RussianRouletteChance = 0.9f;

    private readonly Threadpool threadpool;

    /// <summary> Create a new <see cref="BackwardsSampler"/> </summary>
    /// <param name="threadpool">The <see cref="Threadpool"/> to trace the samples on</param>
    public BackwardsSampler(Threadpool threadpool) {
        this.threadpool = threadpool;
    }

    public void Integrate(IScene scene, int sampleCount) {
        var taskSize = (double)sampleCount / threadpool.MultithreadingTaskCount;
        var tasks = new Action[threadpool.MultithreadingTaskCount];
        for (var i = 0; i < threadpool.MultithreadingTaskCount; i++) {
            var lowerbound = (int)(i * taskSize);
            var higherbound = (int)((i + 1) * taskSize);
            tasks[i] = () => TraceRays(scene, higherbound - lowerbound, new Random());
        }
        threadpool.DoTasks(tasks);
        threadpool.WaitTillDone();
    }

    private static void TraceRays(IScene scene, int count, Random random) {
        for (var i = 0; i < count; i++) {
            Position2 position = new(random.NextSingle(), random.NextSingle());
            Direction2 direction = new(random.NextSingle(), random.NextSingle());
            var ray = scene.Camera.GetCameraRay(position, direction);
            try {
                var light = Sample(scene, ray, RGBSpectrum.White, random, 0);
                var sample = new Sample() { Position = position, Direction = direction, Light = light, PrimaryBVHTraversals = ray.BVHTraversals };
                scene.Camera.Film.RegisterSample(sample);
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    /// <summary> Sample the <paramref name="scene"/> with a <paramref name="sample"/> returning a color found </summary>
    /// <param name="scene">The <see cref="IScene"/> to sample </param>
    /// <param name="ray">The <see cref="Ir4"/> to trace through the <paramref name="scene"/> </param>
    /// <param name="spectrum">The throughput <see cref="ISpectrum"/></param>
    /// <param name="random">The <see cref="Random"/> to sample with</param>
    /// <param name="recursionDepth">The depth of recursion</param>
    /// <returns>The color found for the <see cref="ISample"/></returns>
    private static RGBSpectrum Sample(IScene scene, IRay ray, RGBSpectrum spectrum, Random random, int recursionDepth) {
        if (spectrum.IsBlack) return RGBSpectrum.Black;

        /// Russian Roulette
        var throughput = 1f;
        if (recursionDepth >= GauranteedRecursionDepth) {
            if (random.NextSingle() < RussianRouletteChance) {
                return RGBSpectrum.Black;
            } else {
                throughput = 1f / RussianRouletteChance;
            }
        }

        /// Sample Distance
        var distanceQuery = scene.Trace(ray, spectrum);
        if (distanceQuery is null) return RGBSpectrum.Black;
        var distance = distanceQuery.DistanceDistribution.Sample(random);
        if (distance == Position1.PositiveInfinity) return RGBSpectrum.Black;

        /// Sample Primitive
        var primitives = distanceQuery.TryGetPrimitives(distance) ?? throw new InvalidOperationException("Distance was sampled but no primitive was found");
        var primitive = primitives.Sample(random);

        /// Get Intersection Position
        var position = primitive.Material.DensityProfile.GetPosition(ray, distance, primitive.Shape);

        /// Sample Material Orientation
        var orientations = primitive.Material.OrientationProfile.GetOrientations(position, ray.Direction, primitive.Shape);
        if (orientations is null) return RGBSpectrum.Black;
        var orientation = orientations.Sample(random);

        /// Get Direct Illumination
        var directIllumination = RGBSpectrum.Black;
        if (primitive.Material.EmittanceProfile.IsEmitting) {
            directIllumination = primitive.Material.EmittanceProfile.GetEmittance(position, orientation, -ray.Direction);
        }

        /// Get Indirect Illumination
        var indirectIllumination = RGBSpectrum.Black;
        if (!primitive.Material.AbsorptionProfile.IsBlackBody) {
            /// Sample Direction
            var directions = primitive.Material.ReflectionProfile.GetDirections(ray.Direction, position, orientation, spectrum);
            var direction = directions.Sample(random);

            /// Get Albedo
            var albedo = primitive.Material.AbsorptionProfile.GetAlbedo(position, orientation, -direction);

            /// Get Ray
            var raySample = primitive.Material.DensityProfile.GetRay(position, orientation, direction);

            /// Sample Indirect Illumination
            indirectIllumination = albedo * Sample(scene, raySample, spectrum * albedo, random, recursionDepth + 1);
        }

        /// Light Throughput Calculation
        return (directIllumination + indirectIllumination) * throughput;
    }
}
