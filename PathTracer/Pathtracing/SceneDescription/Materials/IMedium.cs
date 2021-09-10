using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public interface IMedium : IMaterial {
        ISpectrum Absorb(IRay ray, ISpectrum spectrum);
        ISpectrum Emit(IRay ray);
    }
}
