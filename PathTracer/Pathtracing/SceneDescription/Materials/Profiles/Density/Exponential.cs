using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density {
    internal class Exponential : IDensityProfile {
        public IRay CreateRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position, direction);
        }

        public IDistanceDistribution GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) {
            throw new NotImplementedException("Reference to material has to be handled in the material");
            //return ray.WithinBounds(interval.Entry) ? new UniformInterval(((float)interval.Entry).Decrement(64), interval.Entry, this, interval) : null;
        }

        public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) {
            return ray.Travel(distance);
        }
    }
}
