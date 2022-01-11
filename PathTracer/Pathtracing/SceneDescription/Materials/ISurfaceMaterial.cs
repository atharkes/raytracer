using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="Position3"/> on creation.
    /// </summary>
    public interface ISurfaceMaterial : IMaterial {
        /// <summary>
        /// Epsilon used to raise the exiting <see cref="IRay"/>s away from the scene object.
        /// Used to avoid the intersection falling behind the scene object due to rounding errors.
        /// </summary>
        public const float RaiseEpsilon = 0.000001f;

        /// <summary> The rougness of the <see cref="ISurfaceMaterial"/> </summary>
        public float Roughness { get; }

        /// <summary> Get a distance distribution of a <paramref name="ray"/> through the <see cref="ISurfaceMaterial"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IShapeInterval"/> of the <see cref="ISurfaceMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance distribution of the <paramref name="ray"/> through the <see cref="ISurfaceMaterial"/></returns>
        IDistanceDistribution? IMaterial.DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            return interval.Entry < 0 || interval.Entry > ray.Length ? null : new UniformInterval(((float)interval.Entry).Decrement(64), interval.Entry, this, interval);
        }

        /// <summary> Get a <see cref="Position3"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to find a position at</param>
        /// <param name="interval">The <see cref="IShapeInterval"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The distance along the <paramref name="ray"/></param>
        /// <returns>The <see cref="Position3"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        Position3 IMaterial.GetPosition(IRay ray, IShapeInterval interval, Position1 distance) {
            return interval.Shape.IntersectPosition(ray, distance);
        }

        /// <summary> Get an <see cref="IPDF{T}"/> of <see cref="Normal3"/> for the specified <paramref name="position"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> along which the <paramref name="position"/> is found</param>
        /// <param name="shape">The <see cref="IShape"/> in which the <paramref name="position"/> is found</param>
        /// <param name="position">The position at which to get the normal distribution</param>
        /// <returns>A <see cref="IPDF{T}"/> of <see cref="Normal3"/> at the specified <paramref name="position"/></returns>
        IProbabilityDistribution<Normal3>? IMaterial.GetOrientationDistribution(IRay ray, IShape shape, Position3 position) {
            Normal3 shapeOrientation = shape.OutwardsDirection(position);
            if (Roughness == 0f) {
                return new UniformPMF<Normal3>(shapeOrientation);
            } else {
                return new SurfaceGGX(shapeOrientation, Roughness, -ray.Direction);
            }
        }

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="orientation">The <see cref="ISurfaceMaterial"/>s orientation at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay IMaterial.CreateRay(Position3 position, Normal3 orientation, Normal3 direction) => new Ray(position, direction);
    }
}
