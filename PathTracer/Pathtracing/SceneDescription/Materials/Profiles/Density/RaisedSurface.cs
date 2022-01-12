using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density {
    internal class RaisedSurface : IDensityProfile {
        public IRay CreateRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position, direction);
        }

        public IDistanceDistribution GetDistances(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            return ray.WithinBounds(interval.Entry) ? new UniformInterval(((float)interval.Entry).Decrement(64), interval.Entry, this, interval) : null;
        }

        public Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance) {
            return interval.Shape.IntersectPosition(ray, distance);
        }
    }
}
