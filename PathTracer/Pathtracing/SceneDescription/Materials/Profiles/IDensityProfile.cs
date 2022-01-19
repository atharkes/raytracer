using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneDescription.Materials.Profiles.Density;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Profiles {
    /// <summary> The density profile of an <see cref="IMaterial"/> </summary>
    public interface IDensityProfile {
        public static readonly IDensityProfile Surface = new Delta();
        public static IDensityProfile Volumetric(float density) => new Uniform(density);

        /// <summary> Get the <see cref="IDistanceDistribution"/> along the specified <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> through the <see cref="IDensityProfile"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IInterval"/> along which the <see cref="IMaterial"/> is intersected</param>
        /// <returns>The <see cref="IDistanceDistribution"/> along the <paramref name="ray"/></returns>
        IDistanceDistribution? GetDistances(IRay ray, ISpectrum spectrum, IInterval interval);

        /// <summary> Get a <see cref="Position3"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to find a position at</param>
        /// <param name="distance">The distance along the <paramref name="ray"/></param>
        /// <param name="shape">The <see cref="IShape"/> that is traversed by the <paramref name="ray"/></param>
        /// <returns>The <see cref="Position3"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        Position3 GetPosition(IRay ray, Position1 distance, IShape shape);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="orientation">The <see cref="IMaterial"/>s orientation at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay GetRay(Position3 position, Normal3 orientation, Normal3 direction);
    }
}
