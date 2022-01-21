using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density {
    public class Delta : IDensityProfile {
        /// <summary>
        /// Epsilon used to raise the exiting <see cref="IRay"/>s away from the scene object.
        /// Used to avoid the intersection falling behind the scene object due to rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.00001f;

        public IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction) {
            return new Ray(position + orientation * RaiseEpsilon, direction);
        }

        public IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval) {
            return ray.WithinBounds(interval.Entry) ? new DeltaDistance(interval.Entry) : null;
        }

        public Position3 GetPosition(IRay ray, Position1 distance, IShape shape) {
            return shape.IntersectPosition(ray, distance);
        }
    }
}
