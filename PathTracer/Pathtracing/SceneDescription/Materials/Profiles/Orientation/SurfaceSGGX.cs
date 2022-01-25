using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class SurfaceSGGX : IOrientationProfile {
        public float Roughness { get; }

        public SurfaceSGGX(float roughness) {
            Roughness = roughness;
        }

        public IProbabilityDistribution<Normal3> GetOrientations(Position3 position, Normal3 direction, IShape shape) {
            Normal3 orientation = shape.OutwardsDirection(position);
            return Roughness > 0f ? new SurfaceEllipsoid(orientation, Roughness, direction) : new UniformPMF<Normal3>(orientation);
        }
    }
}
