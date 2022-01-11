using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    public class Diffuse : ISurfaceMaterial {
        public ISpectrum Albedo { get; }
        public float Roughness { get; } 

        public Diffuse(ISpectrum albedo, float roughness) {
            Albedo = albedo;
            Roughness = roughness;
        }

        public IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            return new HemisphericalDiffuse(orientation);
        }
    }
}
