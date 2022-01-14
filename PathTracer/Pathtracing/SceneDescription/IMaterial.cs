using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> The color <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        ISpectrum Albedo { get; }

        /// <summary> Get a distance distribution of a <paramref name="ray"/> through the <see cref="IMaterial"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IShapeInterval"/> of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance distribution of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
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
        IProbabilityDistribution<Normal3>? GetOrientationDistribution(IRay ray, IShape shape, Position3 position);

        /// <summary> Get a direction distribution of the <see cref="IMaterial"/> at a <paramref name="position"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="position"/></param>
        /// <param name="position">The <see cref="Position3"/> at which the scattering occurs</param>
        /// <param name="orientation">The orientation of the <see cref="IMaterial"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="position"/></param>
        /// <returns>A direction-medium PDF</returns>
        IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="orientation">The <see cref="IMaterial"/>s orientation at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay CreateRay(Position3 position, Normal3 orientation, Normal3 direction);
    }
}
