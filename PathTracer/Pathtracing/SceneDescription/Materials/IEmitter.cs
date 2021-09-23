using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> A <see cref="IMaterial"/> that can emit light </summary>
    public interface IEmitter : IMaterial {
        /// <summary> The emission <see cref="ISpectrum"/> of the <see cref="IEmitter"/> </summary>
        ISpectrum Emission { get; }
    }
}
