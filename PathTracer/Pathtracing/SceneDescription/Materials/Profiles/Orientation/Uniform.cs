using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class Uniform : IOrientationProfile {
        public IProbabilityDistribution<Normal3> GetOrientations(IRay ray, IShape shape, Position3 position) {
            return new HemisphericalDiffuse(-ray.Direction);
        }
    }
}
