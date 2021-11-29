using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials {
    class DiffuseVolumetric : IVolumetricMaterial {
        public double Density { get; }
        public ISpectrum Albedo { get; }
        public bool IsEmitting => false;
        public bool IsSensing => false;

        public DiffuseVolumetric(ISpectrum albedo, double density) {
            Density = density;
            Albedo = albedo;
        }

        public IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            return new SphericalUniform(orientation);
        }

        public ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) => ISpectrum.Black;
    }
}
