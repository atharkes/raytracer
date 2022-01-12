using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class SurfaceGGX : IOrientationProfile {
        public float Roughness { get; }

        public SurfaceGGX(float roughness) {
            Roughness = roughness;
        }

        public IProbabilityDistribution<Normal3> GetOrientations(IRay ray, IShape shape, Position3 position) {
            return new SurfaceHalfEllipsoid(shape.OutwardsDirection(position), Roughness, ray.Direction);
        }
    }
}
