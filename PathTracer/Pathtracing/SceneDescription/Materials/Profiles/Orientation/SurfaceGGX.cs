using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class SurfaceGGX : IOrientationProfile {
        public float Roughness { get; }

        public SurfaceGGX(float roughness) {
            Roughness = roughness;
        }

        public IProbabilityDistribution<Normal3> GetOrientations(Position3 position, Normal3 direction, IShape shape) {
            return new SurfaceHalfEllipsoid(shape.OutwardsDirection(position), Roughness, direction);
        }
    }
}
