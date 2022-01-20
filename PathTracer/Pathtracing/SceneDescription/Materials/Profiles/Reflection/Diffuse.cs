using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Reflection {
    public class Diffuse : IReflectionProfile {
        public IProbabilityDistribution<Normal3> GetDirections(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            return new HemisphericalDiffuse(orientation);
        }
    }
}
