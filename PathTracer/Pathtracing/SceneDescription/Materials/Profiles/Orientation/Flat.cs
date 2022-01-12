using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation {
    public class Flat : IOrientationProfile {
        public IProbabilityDistribution<Normal3> GetOrientations(IRay ray, IShape shape, Position3 position) {
            return new UniformPMF<Normal3>(shape.OutwardsDirection(position));
        }
    }
}
