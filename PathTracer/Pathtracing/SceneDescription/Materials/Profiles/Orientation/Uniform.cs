using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Orientation;

public class Uniform : IOrientationProfile {
    public IProbabilityDistribution<Normal3> GetOrientations(Position3 position, Normal3 direction, IShape shape) => new HemisphericalDiffuse(-direction);
}
