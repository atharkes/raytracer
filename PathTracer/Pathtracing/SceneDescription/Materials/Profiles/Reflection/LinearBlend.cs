using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Reflection;

public class LinearBlend : IReflectionProfile {
    private readonly (IReflectionProfile profile, double weight)[] pairs;

    public LinearBlend(params (IReflectionProfile profile, double weight)[] pairs) {
        this.pairs = pairs;
    }

    public IProbabilityDistribution<Normal3> GetDirections(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
        var blend = pairs.Select(p => (p.profile.GetDirections(incomingDirection, position, orientation, spectrum), p.weight)).ToArray();
        return new CombinedProbabilityDistribution<Normal3>(blend);
    }
}
