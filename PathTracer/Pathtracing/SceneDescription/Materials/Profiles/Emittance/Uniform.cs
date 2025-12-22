using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Emittance;

public class Uniform : IEmittanceProfile {
    public ISpectrum Spectrum { get; }
    public bool IsEmitting => !Spectrum.IsBlack;

    public Uniform(ISpectrum spectrum) {
        Spectrum = spectrum;
    }

    public ISpectrum GetEmittance(Position3 position, Normal3 orientation, Normal3 direction) => Spectrum;
}
