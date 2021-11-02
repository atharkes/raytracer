using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; set; }
        public abstract bool IsEmitting { get; }
        public abstract bool IsSensing { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public abstract IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval);
        public abstract Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance);
        public abstract IPDF<Normal3> GetOrientationDistribution(IRay ray, IShape shape, Position3 position);

        public abstract ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction);
        public abstract IDirectionDistribution? DirectionDistribution(Normal3 incomingDirection, Position3 position, ISpectrum spectrum);
        public abstract IRay CreateRay(Position3 position, Normal3 normal, Normal3 direction);
    }
}
