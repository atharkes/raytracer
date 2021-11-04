using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
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
        /// <summary> The maximum recursion depth for sampling </summary>
        public int MaxRecursionDepth { get; } = 3;
        /// <summary> The amount of evaluated samples </summary>
        public override int SampleCount { get; } = 0;

        public override void Integrate(IScene scene, TimeSpan integrationTime) {
            int taskSize = 10;
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                tasks[i] = () => TraceRays(scene, taskSize);
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
            float distancePdf = (float)distances.Probability(distance);
            float distanceDomain = (float)distances.DomainSize;

            /// Sample Material
            IPDF<IMaterial>? materials = distances.GetMaterials(distance);
            if (materials is null) throw new InvalidOperationException("Distance was sampled but no material was found");
            IMaterial material = materials.Sample(Utils.ThreadRandom);
            float materialPdf = (float)materials.Probability(material);
            float materialDomain = (float)materials.DomainSize;

            /// Sample Shape Interval
            IPDF<IShapeInterval>? intervals = distances.GetShapeIntervals(distance, material);
            if (intervals is null) throw new InvalidOperationException("Distance was sampled but no shape interval was found");
            IShapeInterval interval = intervals.Sample(Utils.ThreadRandom);
            float intervalPdf = (float)intervals.Probability(interval);
            float intervalDomain = (float)intervals.DomainSize;

            /// Compute Distance Sampling Throughput
            float distanceThroughput = distancePdf * distanceDomain * materialPdf * materialDomain * intervalPdf * intervalDomain;

            /// Get Intersection Position
            Position3 position = material.GetPosition(ray, interval, distance);

            /// Sample Material Orientation
            IPDF<Normal3> orientations = material.GetOrientationDistribution(ray, interval.Shape, position);
            Normal3 orientation = orientations.Sample(Utils.ThreadRandom);
            float orientationPdf = (float)orientations.Probability(orientation);
            float orientationDomain = (float)orientations.DomainSize;

            /// Get Direct Illumination
            ISpectrum directIllumination = material.Emittance(position, orientation, -ray.Direction);

            /// Sample Direction
            IPDF<Normal3> directions = material.DirectionDistribution(ray.Direction, position, orientation, spectrum);
            Normal3 direction = directions.Sample(Utils.ThreadRandom);
            float directionPdf = (float)directions.Probability(direction);
            float directionDomain = (float)directions.DomainSize;

            /// Compute Direction Sampling Throughput
            ISpectrum absorption = material.Albedo;
            float areaConversion = Math.Abs(Vector3.Dot(orientation.Vector, direction.Vector));
            ISpectrum directionThroughput = absorption * areaConversion * directionPdf * directionDomain * orientationPdf * orientationDomain;
            if (directionThroughput.IsBlack) return directIllumination;

            /// Sample Indirect Illumination
            ISpectrum indirectIllumination = Sample(scene, new Ray(position, direction), directionThroughput, recursionDepth + 1);

            /// Light Throughput Calculation
            return (indirectIllumination * directionThroughput + directIllumination) * distanceThroughput;
        }
    }
}
