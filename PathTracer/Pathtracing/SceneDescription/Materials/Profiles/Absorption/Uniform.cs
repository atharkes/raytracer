using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Absorption {
    public class Uniform : IAbsorptionProfile {
        public ISpectrum Albedo { get; }
        public bool IsBlackBody => Albedo.IsBlack;

        public Uniform(ISpectrum albedo) {
            Albedo = albedo;
        }

        public ISpectrum GetAlbedo(Position3 position, Normal3 orientation, Normal3 direction) {
            return Albedo;
        }
    }
}
