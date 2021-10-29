using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Integrators {
    public class BackwardsSampler : Integrator {
        /// <summary> The maximum recursion depth for sampling </summary>
        public int MaxRecursionDepth { get; } = 5;
        /// <summary> The amount of evaluated samples </summary>
        public override int SampleCount { get; } = 0;

        public override void Integrate(IScene scene, TimeSpan integrationTime) {
            int rayCount = 100;
            Action[] tasks = new Action[Program.Threadpool.MultithreadingTaskCount];
            for (int i = 0; i < Program.Threadpool.MultithreadingTaskCount; i++) {
                tasks[i] = () => TraceRays(scene, rayCount);
            }
            Program.Threadpool.DoTasks(tasks);
            Program.Threadpool.WaitTillDone();
        }

        void TraceRays(IScene scene, int count) {
            for (int i = 0; i < count; i++) {
                Position2 position = new((float)Utils.Random.NextDouble(), (float)Utils.Random.NextDouble());
                Direction2 direction = new((float)Utils.Random.NextDouble(), (float)Utils.Random.NextDouble());
                CameraRay ray = scene.Camera.GetCameraRay(position, direction);
                ISpectrum light = Sample(scene, ray, ISpectrum.White, 0);
                scene.Camera.Film.RegisterSample();
            }
        }

        /// <summary> Sample the <paramref name="scene"/> with a <paramref name="sample"/> returning a color found </summary>
        /// <param name="scene">The <see cref="IScene"/> to sample </param>
        /// <param name="sample">The <see cref="ISample"/> to sample the <paramref name="scene"/> with </param>
        /// <returns>The color found for the <see cref="ISample"/></returns>
        public ISpectrum Sample(IScene scene, IRay ray, ISpectrum spectrum, int recursionDepth) {
            if (recursionDepth > MaxRecursionDepth) return ISpectrum.Black;

            /// Distance Sampling
            IDistanceDistribution? distances = scene.Trace(ray, spectrum);
            if (distances is null) return ISpectrum.Black;
            Position1 distance = distances.Sample(Utils.Random);
            if (distance == Position1.PositiveInfinity) return ISpectrum.Black;

            IPDF<IMaterial>? materials = distances.GetMaterials(distance);
            if (materials is null) throw new InvalidOperationException("Distance was sampled but no material was found");
            IMaterial material = materials.Sample(Utils.Random);

            IPDF<IShapeInterval>? intervals = distances.GetShapeIntervals(distance, material);
            if (intervals is null) throw new InvalidOperationException("Distance was sampled but no shape interval was found");
            IShapeInterval interval = intervals.Sample(Utils.Random);

            Position3 position = material.GetPosition(ray, interval, distance);
            IPDF<Normal3> normals = material.GetOrientationDistribution(ray, interval.Shape, position);
            Normal3 normal = normals.Sample(Utils.Random);

            /// Direction Sampling
            IDirectionDistribution? directions = material.DirectionDistribution(ray.Direction, position, spectrum);
            if (directions is null) return ISpectrum.Black;
            Direction3 direction = directions.Sample(Utils.Random);

            /// Direct Illumination
            ISpectrum outgoingLight = distances.Emittance(sample.Ray);
            /// Indirect Illumination
            Normal3 incSample = distances.BSDF().Sample(sample, Utils.Random);
            ISpectrum incLight = Sample(scene, incSample);
            ISpectrum scatteredLight = incLight * distances.Absorption(incSample.Ray);
            ISpectrum relevantLight = scatteredLight * Vector3.Dot(incSample.Ray.Direction, distances.Normal);
            outgoingLight += relevantLight;

            return outgoingLight;

            //Vector3 radianceOut;
            //if (surfacePoint.Primitive.Material.Specularity > 0) {
            //    // Specular
            //    Vector3 reflectedIn = Sample(intersection.Reflect());
            //    Vector3 reflectedOut = reflectedIn * intersection.SurfacePoint.Primitive.Material.Color;
            //    radianceOut = irradianceIn * (1 - intersection.SurfacePoint.Primitive.Material.Specularity) + reflectedOut * surfacePoint.Primitive.Material.Specularity;
            //} else if (surfacePoint.Primitive.Material.Dielectric > 0) {
            //    // Dielectric
            //    float reflected = intersection.Reflectivity();
            //    float refracted = 1 - reflected;
            //    Ray? refractedRay = intersection.Refract();
            //    Vector3 incRefractedLight = refractedRay != null ? Sample(refractedRay) : Vector3.Zero;
            //    Vector3 incReflectedLight = Sample(intersection.Reflect());
            //    radianceOut = irradianceIn * (1f - surfacePoint.Primitive.Material.Dielectric) + (incRefractedLight * refracted + incReflectedLight * reflected) * surfacePoint.Primitive.Material.Dielectric * surfacePoint.Primitive.Material.Color;
            //} else {
            //    // Diffuse
            //    radianceOut = irradianceIn;
            //}
        }
    }
}
