using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Emittance;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The emittance profile of an <see cref="IMaterial"/> </summary>
    public interface IEmittanceProfile {
        public static readonly IEmittanceProfile None = new Uniform(RGBSpectrum.Black);
        public static IEmittanceProfile Uniform(ISpectrum spectrum) => new Uniform(spectrum);

        /// <summary> Whether the <see cref="IEmittanceProfile"/> is emitting light </summary>
        bool IsEmitting { get; }

        /// <summary> Get the emission <see cref="ISpectrum"/> along the specified <paramref name="direction"/> </summary>
        /// <param name="position">The position to get the emission at</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/> at the <paramref name="position"/></param>
        /// <param name="direction">The direction of the emission</param>
        /// <returns>The emission at the <paramref name="position"/> in the specified <paramref name="direction"/></returns>
        ISpectrum GetEmittance(Position3 position, Normal3 orientation, Normal3 direction);
    }
}
