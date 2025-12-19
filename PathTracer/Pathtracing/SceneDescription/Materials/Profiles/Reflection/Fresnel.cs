using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Reflection;

public class Fresnel : IReflectionProfile {
    /// <summary> The refractive index of the <see cref="Fresnel"/> <see cref="IReflectionProfile"/> This is typically a value between 1 and 3.
    /// <para> Vacuum 1 </para>
    /// <para> Gases at 0 °C: Air 1.000293, Helium 1.000036, Hydrogen 1.000132, Carbon dioxide 1.00045 </para>
    /// <para> Liquids at 20 °C: Water 1.333, Ethanol 1.36, Olive oil 1.47 </para>
    /// <para> Solids: Ice 1.31, Fused silica(quartz) 1.46, Plexiglas 1.49, Window glass 1.52, 
    /// Flint glass 1.62, Sapphire 1.77, Cubic zirconia 2.15, Diamond 2.42, Moissanite 2.65 </para> </summary>
    public float RefractiveIndex { get; }
    /// <summary> The priority of this <see cref="Fresnel"/> <see cref="IReflectionProfile"/> </summary>
    public float Priority { get; }

    public Fresnel(float refractiveIndex, float priority) {
        RefractiveIndex = refractiveIndex;
        Priority = priority;
    }

    public IProbabilityDistribution<Normal3> GetDirections(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) => throw new NotImplementedException("Requires structure to remember indices of refraction");
}
