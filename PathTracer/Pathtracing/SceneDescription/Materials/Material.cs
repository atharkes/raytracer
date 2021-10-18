using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public abstract IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction);
        public abstract IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary);
        public abstract IDirectionDistribution? DirectionDistribution(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
    }
}
