using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The absorption profile of an <see cref="IMaterial"/> </summary>
    public interface IAbsorptionProfile {
        /// <summary> The color <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        ISpectrum Albedo { get; }
        /// <summary> The absoprtion <see cref="ISpectrum"/> of the <see cref="IAbsorptionProfile"/> </summary>
        ISpectrum Absorption => ISpectrum.White - Albedo;
        /// <summary> Whether the <see cref="IAbsorptionProfile"/> is sensing light or not </summary>
        bool IsSensing { get; }
    }
}
