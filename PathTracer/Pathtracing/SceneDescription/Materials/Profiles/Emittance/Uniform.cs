using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Emittance;

public class Uniform : IEmittanceProfile {
    public RGBSpectrum Spectrum { get; }
    public bool IsEmitting => !Spectrum.IsBlack;

    public Uniform(RGBSpectrum spectrum) {
        Spectrum = spectrum;
    }

    public RGBSpectrum GetEmittance(Position3 position, Normal3 orientation, Normal3 direction) => Spectrum;
}
