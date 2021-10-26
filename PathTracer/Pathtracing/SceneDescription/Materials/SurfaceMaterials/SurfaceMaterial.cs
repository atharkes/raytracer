using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System.Diagnostics;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="IMaterialPoint1"/> on creation.
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

        public override IRay CreateRay(IMaterialPoint1 surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position + surfacePoint.Normal * RaiseEpsilon, direction);
        }

        public override IMaterialPoint1 CreateSurfacePoint(IRay ray, IBoundaryInterval interval, float distance) {
            Debug.Assert(interval.Entry.Distance == distance);
            return new SurfacePoint(this, interval.Entry.Position, interval.Entry.Normal);
        }

        public override IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary) {
            float entry = boundary.BoundaryIntervals.First(i => i.Entry.Distance > 0).Entry.Distance;
            return new SingleDistanceDistribution(new DistanceMaterial(entry, this));
        }
    }
}
