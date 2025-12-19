using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.SceneObjects;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;

namespace PathTracer.Pathtracing.Integrators;

/// <summary> An <see cref="Integrator"/> that samples from the <see cref="ICamera"/> </summary>
public class BackwardsSampler : IIntegrator {
    /// <summary> The minimum amount of samples per integration cycle of the <see cref="BackwardsSampler"/> </summary>
    public static readonly int MinimumSampleCount = Program.Threadpool.MultithreadingTaskCount * 10;
    /// <summary> The maximum recursion depth for sampling </summary>
    public const int GauranteedRecursionDepth = 16;
    /// <summary> The chance of russian roulette when the gauranteed recursion depth is exceeded </summary>
    public const float RussianRouletteChance = 0.9f;

    public void Integrate(IScene scene, int sampleCount) {
        var taskSize = (double)sampleCount / Program.Threadpool.MultithreadingTaskCount;
        var tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
        for (var i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
            var lowerbound = (int)(i * taskSize);
            var higherbound = (int)((i + 1) * taskSize);
            tasks[i] = () => TraceRays(scene, higherbound - lowerbound);
        }
        Program.Threadpool.DoTasks(tasks);
        Program.Threadpool.WaitTillDone();
    }

    private void TraceRays(IScene scene, int count) {
        for (var i = 0; i < count; i++) {
            Position2 position = new((float)Utils.ThreadRandom.NextDouble(), (float)Utils.ThreadRandom.NextDouble());
            Direction2 direction = new((float)Utils.ThreadRandom.NextDouble(), (float)Utils.ThreadRandom.NextDouble());
            var ray = scene.Camera.GetCameraRay(position, direction);
            try {
                var light = Sample(scene, ray, ISpectrum.White, 0);
                ISample sample = new Sample() { Position = position, Direction = direction, Light = light, Intersection = ray.Intersection, PrimaryBVHTraversals = ray.BVHTraversals };
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
    /// <param name="recursionDepth">The depth of recursion</param>
    /// <returns>The color found for the <see cref="ISample"/></returns>
    public ISpectrum Sample(IScene scene, IRay ray, ISpectrum spectrum, int recursionDepth) {
        if (spectrum.Equals(ISpectrum.Black)) return ISpectrum.Black;

        /// Russian Roulette
        var throughput = 1f;
        if (recursionDepth >= GauranteedRecursionDepth) {
            if (Utils.ThreadRandom.NextSingle() < RussianRouletteChance) {
                return ISpectrum.Black;
            } else {
                throughput = 1f / RussianRouletteChance;
            }
        }

        /// Sample Distance
        var distanceQuery = scene.Trace(ray, spectrum);
        if (distanceQuery is null) return ISpectrum.Black;
        var distance = distanceQuery.DistanceDistribution.Sample(Utils.ThreadRandom);
        if (distance == Position1.PositiveInfinity) return ISpectrum.Black;

        /// Sample Primitive
        IProbabilityDistribution<IPrimitive>? primitives = distanceQuery.TryGetPrimitives(distance);
        if (primitives is null) throw new InvalidOperationException("Distance was sampled but no primitive was found");
        var primitive = primitives.Sample(Utils.ThreadRandom);

        /// Get Intersection Position
        var position = primitive.Material.DensityProfile.GetPosition(ray, distance, primitive.Shape);

        /// Sample Material Orientation
        var orientations = primitive.Material.OrientationProfile.GetOrientations(position, ray.Direction, primitive.Shape);
        if (orientations is null) return ISpectrum.Black;
        var orientation = orientations.Sample(Utils.ThreadRandom);

        /// Get Direct Illumination
        ISpectrum directIllumination = RGBColors.Black;
        if (primitive.Material.EmittanceProfile.IsEmitting) {
            directIllumination = primitive.Material.EmittanceProfile.GetEmittance(position, orientation, -ray.Direction);
        }

        /// Get Indirect Illumination
        ISpectrum indirectIllumination = RGBColors.Black;
        if (!primitive.Material.AbsorptionProfile.IsBlackBody) {
            /// Sample Direction
            var directions = primitive.Material.ReflectionProfile.GetDirections(ray.Direction, position, orientation, spectrum);
            var direction = directions.Sample(Utils.ThreadRandom);

            /// Get Albedo
            var albedo = primitive.Material.AbsorptionProfile.GetAlbedo(position, orientation, -direction);

            /// Get Ray
            var raySample = primitive.Material.DensityProfile.GetRay(position, orientation, direction);

            /// Sample Indirect Illumination
            indirectIllumination = albedo * Sample(scene, raySample, spectrum * albedo, recursionDepth + 1);
        }

        /// Light Throughput Calculation
        return (directIllumination + indirectIllumination) * throughput;
    }
}
