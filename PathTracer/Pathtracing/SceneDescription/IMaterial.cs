using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> The color <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        ISpectrum Albedo { get; set; }
        /// <summary> Whether the <see cref="IMaterial"/> is emitting light or not </summary>
        bool IsEmitting { get; }
        /// <summary> Whether the <see cref="IMaterial"/> is sensing light or not </summary>
        bool IsSensing { get; }

        /// <summary> The emission <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        /// <param name="position">The position to get the emission at</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/> at the <paramref name="position"/></param>
        /// <param name="direction">The direction of the emission</param>
        /// <returns>The emission at the <paramref name="position"/> in the specified <paramref name="direction"/></returns>
        ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction);

        /// <summary> Get a distance-material PDF of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundary">The <see cref="IBoundary"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance-material PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary) {
            IDistanceDistribution? result = null;
            foreach (ShapeInterval interval in boundary) {
                IDistanceDistribution? distanceDistribution = DistanceDistribution(ray, spectrum, interval);
                if (distanceDistribution is not null) {
                    result += distanceDistribution;
                }
            }
            return result;
        }

        /// <summary> Get a distance-material PDF of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IShapeInterval"/> of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance-material PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval);

        /// <summary> Get a <see cref="Position3"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to find a position at</param>
        /// <param name="interval">The <see cref="IShapeInterval"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The distance along the <paramref name="ray"/></param>
        /// <returns>The <see cref="Position3"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance);

        /// <summary> Get an <see cref="IPDF{T}"/> of <see cref="Normal3"/> for the specified <paramref name="position"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> along which the <paramref name="position"/> is found</param>
        /// <param name="shape">The <see cref="IShape"/> in which the <paramref name="position"/> is found</param>
        /// <param name="position">The position at which to get the normal distribution</param>
        /// <returns>A <see cref="IPDF{T}"/> of <see cref="Normal3"/> at the specified <paramref name="position"/></returns>
        IPDF<Normal3> GetOrientationDistribution(IRay ray, IShape shape, Position3 position);

        /// <summary> Get a direction-medium PDF at a <paramref name="position"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="position"/></param>
        /// <param name="position">The <see cref="Position3"/> at which the scattering occurs</param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="position"/></param>
        /// <returns>A direction-medium PDF</returns>
        IDirectionDistribution? DirectionDistribution(Normal3 incomingDirection, Position3 position, ISpectrum spectrum);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position3"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="normal">The <see cref="Normal3"/> at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay CreateRay(Position3 position, Normal3 normal, Normal3 direction);
    }
}
