using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Reflection {
    public class Fresnel : IReflectionProfile {
        public IProbabilityDistribution<Normal3> GetDirections(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            throw new NotImplementedException("Requires structure to remember indices of refraction");
        }
    }
}
