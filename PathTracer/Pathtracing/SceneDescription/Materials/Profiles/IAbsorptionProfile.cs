using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Absorption;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The absorption profile of an <see cref="IMaterial"/> </summary>
    public interface IAbsorptionProfile {
        public static readonly IAbsorptionProfile BlackBody = new Uniform(RGBColors.Black);
        public static IAbsorptionProfile Uniform(ISpectrum albedo) => new Uniform(albedo);

        /// <summary> Whether the <see cref="IAbsorptionProfile"/> absorbs all incoming light </summary>
        bool IsBlackBody { get; }

        /// <summary> Get the albedo <see cref="ISpectrum"/> along the specified incoming <paramref name="direction"/> </summary>
        /// <param name="position">The position to get the albedo at</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/> at the <paramref name="position"/></param>
        /// <param name="direction">The incoming direction of the light</param>
        /// <returns>The albedo at the <paramref name="position"/> in the specified <paramref name="direction"/></returns>
        ISpectrum GetAlbedo(Position3 position, Normal3 orientation, Normal3 direction);
    }
}
