using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density {
    public class Exponential : IDensityProfile {
        public IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position, direction);
        }

        public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) {
            return ray.WithinBounds(interval.Entry) ? new UniformInterval(new Interval(((float)interval.Entry).Decrement(64), interval.Entry)) : null;
        }

        public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) {
            return ray.Travel(distance);
        }
    }
}
