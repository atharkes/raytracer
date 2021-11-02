using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System.Diagnostics;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="Position3"/> on creation.
    /// </summary>
    public abstract class SurfaceMaterial : Material, IMaterial {
        /// <summary>
        /// Epsilon used to raise the exiting <see cref="IRay"/>s away from the scene object.
        /// Used to avoid the intersection falling behind the scene object due to rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.001f;

        /// <summary> Create a new <see cref="SurfaceMaterial"/> using a specified <paramref name="albedo"/> </summary>
        /// <param name="albedo">The albedo <see cref="ISpectrum"/> of the new <see cref="SurfaceMaterial"/></param>
        public SurfaceMaterial(ISpectrum albedo) : base(albedo) { }

        public override IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            return new SingleDistanceDistribution(interval.Entry, this, interval);
        }

        public override Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance) {
            Debug.Assert(interval.Entry == distance);
            return interval.Shape.IntersectPosition(ray, distance);
        }

        public override IPDF<Normal3> GetOrientationDistribution(IRay ray, IShape shape, Position3 position) {
            return new PMF<Normal3>(shape.OutwardsDirection(position));
        }

        public override IRay CreateRay(Position3 position, Normal3 normal, Normal3 direction) {
            return new Ray(position + normal * RaiseEpsilon, direction);
        }
    }
}
