using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> An <see cref="IMaterial"/> that emits light </summary>
    public interface IEmitter {
        /// <summary> The color of light leaving the <see cref="IEmitter"/> </summary>
        ISpectrum Color { get; }
        /// <summary> The strength of the light leaving the <see cref="IEmitter"/> </summary>
        float Strength { get; }
    }
}
