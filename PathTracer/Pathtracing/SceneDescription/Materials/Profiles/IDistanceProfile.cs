using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The distance profile of an <see cref="IMaterial"/> </summary>
    public interface IDistanceProfile {
        /// <summary> Get the <see cref="IDistanceDistribution"/> along the specified <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> through the <see cref="IDistanceProfile"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IShapeInterval"/> along the <paramref name="ray"/></param>
        /// <returns>The <see cref="IDistanceDistribution"/> along the <paramref name="ray"/></returns>
        IDistanceDistribution GetDistances(IRay ray, ISpectrum spectrum, IShapeInterval interval);

        /// <summary> Get a <see cref="Position3"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to find a position at</param>
        /// <param name="interval">The <see cref="IShapeInterval"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The distance along the <paramref name="ray"/></param>
        /// <returns>The <see cref="Position3"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        Position3 GetPosition(IRay ray, IShapeInterval interval, Position1 distance);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="orientation">The <see cref="IMaterial"/>s orientation at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay CreateRay(Position3 position, Direction3 orientation, Direction3 direction);
    }
}
