using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Absorption;

public class Uniform : IAbsorptionProfile {
    public RGBSpectrum Albedo { get; }
    public bool IsBlackBody => Albedo.IsBlack;

    public Uniform(RGBSpectrum albedo) {
        Albedo = albedo;
    }

    public RGBSpectrum GetAlbedo(Position3 position, Normal3 orientation, Normal3 direction) => Albedo;
}
