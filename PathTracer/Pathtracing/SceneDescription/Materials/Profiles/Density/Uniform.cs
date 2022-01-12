using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
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

        public IRay CreateRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position, direction);
        }

        public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            if (interval.Exit > 0 && interval.Entry < ray.Length) {
                throw new NotImplementedException("Reference to material has to be handled in the material");
                //return new ExponentialInterval(Math.Max(0, interval.Entry), Math.Min(ray.Length, interval.Exit), Density, this, interval);
            } else {
                return null;
            }
        }

        public Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance) {
            return interval.Shape.IntersectPosition(ray, distance);
        }
    }
}
