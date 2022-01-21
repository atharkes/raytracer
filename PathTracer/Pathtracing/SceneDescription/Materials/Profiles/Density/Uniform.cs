using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density {
    public class Uniform : IDensityProfile {
        float Density { get; }

        public Uniform(float density) {
            Density = density;
        }

        public IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position, direction);
        }

        public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) {
            if (interval.Exit > 0 && interval.Entry < ray.Length) {
                return new ExponentialInterval(new Interval(Math.Max(0, interval.Entry), Math.Min(ray.Length, interval.Exit)), Density);
            } else {
                return null;
            }
        }

        public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) {
            return ray.Travel(distance);
        }
    }
}
