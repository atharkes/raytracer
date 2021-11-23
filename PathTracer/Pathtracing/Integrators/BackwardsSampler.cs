using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Observers;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Integrators {
    /// <summary> An <see cref="Integrator"/> that samples from the <see cref="ICamera"/> </summary>
    public class BackwardsSampler : Integrator {
        /// <summary> The minimum amount of samples per integration cycle of the <see cref="BackwardsSampler"/> </summary>
        public static readonly int MinimumSampleCount = Program.Threadpool.MultithreadingTaskCount * 10;
        /// <summary> The maximum recursion depth for sampling </summary>
        public const int MaxRecursionDepth = 3;

        public override void Integrate(IScene scene, int sampleCount) {
            double taskSize = (double)sampleCount / Program.Threadpool.MultithreadingTaskCount;
            TraceRays(scene, (int)taskSize);
            return;
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                int lowerbound = (int)(i * taskSize);
                int higherbound = (int)((i + 1) * taskSize);
                tasks[i] = () => TraceRays(scene, higherbound - lowerbound);
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        void TraceRays(IScene scene, int count) {
            for (int i = 0; i < count; i++) {
                Position2 position = new((float)Utils.ThreadRandom.NextDouble(), (float)Utils.ThreadRandom.NextDouble());
                Direction2 direction = new((float)Utils.ThreadRandom.NextDouble(), (float)Utils.ThreadRandom.NextDouble());
                CameraRay ray = scene.Camera.GetCameraRay(position, direction);
                ISpectrum light = Sample(scene, ray, ISpectrum.White, 0);
                ISample sample = new Sample() { Position = position, Direction = direction, Light = light, Intersection = ray.Intersection, PrimaryBVHTraversals = ray.BVHTraversals };
                scene.Camera.Film.RegisterSample(sample);
            }
        }

        /// <summary> Sample the <paramref name="scene"/> with a <paramref name="sample"/> returning a color found </summary>
        /// <param name="scene">The <see cref="IScene"/> to sample </param>
        /// <param name="sample">The <see cref="ISample"/> to sample the <paramref name="scene"/> with </param>
        /// <returns>The color found for the <see cref="ISample"/></returns>
        public ISpectrum Sample(IScene scene, IRay ray, ISpectrum spectrum, int recursionDepth) {
            if (recursionDepth >= MaxRecursionDepth) return ISpectrum.Black;

            /// Sample Distance
            IDistanceDistribution? distances = scene.Trace(ray, spectrum);
            if (distances is null) return ISpectrum.Black;
            Position1 distance = distances.Sample(Utils.ThreadRandom);
            if (distance == Position1.PositiveInfinity) return ISpectrum.Black;
            float distanceImportance = (float)distances.InverseRelativeProbability(distance);

            /// Sample Material
            IProbabilityDistribution<IMaterial>? materials = distances.GetMaterials(distance);
            if (materials is null) throw new InvalidOperationException("Distance was sampled but no material was found");
            IMaterial material = materials.Sample(Utils.ThreadRandom);
            float materialImportance = (float)materials.InverseRelativeProbability(material);

            /// Sample Shape Interval
            IProbabilityDistribution<IShapeInterval>? intervals = distances.GetShapeIntervals(distance, material);
            if (intervals is null) throw new InvalidOperationException("Distance was sampled but no shape interval was found");
            IShapeInterval interval = intervals.Sample(Utils.ThreadRandom);
            float shapeIntervalImportance = (float)intervals.InverseRelativeProbability(interval);

            /// Compute Distance Sampling Throughput
            float distanceSampleImportance = shapeIntervalImportance * materialImportance * distanceImportance;
            float outScatteringAndDensity = (float)distances.ProbabilityDensity(distance);
            float distanceSampleThroughput = outScatteringAndDensity * distanceSampleImportance;

            /// Get Intersection Position
            Position3 position = material.GetPosition(ray, interval, distance);

            /// Sample Material Orientation
            IProbabilityDistribution<Normal3> orientations = material.GetOrientationDistribution(ray, interval.Shape, position);
            Normal3 orientation = orientations.Sample(Utils.ThreadRandom);
            float orientationImportance = (float)orientations.InverseRelativeProbability(orientation);

            /// Get Direct Illumination
            ISpectrum directIllumination = material.Emittance(position, orientation, -ray.Direction);

            /// Sample Direction
            IProbabilityDistribution<Normal3> directions = material.DirectionDistribution(ray.Direction, position, orientation, spectrum);
            Normal3 direction = directions.Sample(Utils.ThreadRandom);
            float directionImportance = (float)directions.InverseRelativeProbability(direction);

            /// Compute Direction Sampling Throughput
            float directionSampleImportance = orientationImportance * directionImportance;
            ISpectrum absorption = material.Albedo;
            ISpectrum directionSampleThroughput = absorption * directionSampleImportance;
            if (directionSampleThroughput.IsBlack) return directIllumination;

            /// Sample Indirect Illumination
            ISpectrum indirectIllumination = Sample(scene, new Ray(position, direction), directionSampleThroughput, recursionDepth + 1);

            /// Light Throughput Calculation
            return (indirectIllumination * directionSampleThroughput + directIllumination) * distanceSampleThroughput;
        }
    }
}
