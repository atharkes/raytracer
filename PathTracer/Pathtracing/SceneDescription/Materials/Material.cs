using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; }
        public abstract bool IsEmitting { get; }
        public abstract bool IsSensing { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public abstract IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction);
        public abstract ISurfacePoint CreateSurfacePoint(IRay ray, IBoundaryInterval interval, float distance);

        public abstract IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary);
        public abstract IDirectionDistribution? DirectionDistribution(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
    }
}
