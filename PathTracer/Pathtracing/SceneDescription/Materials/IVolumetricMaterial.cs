using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    /// <summary> A volumetric <see cref="IMaterial"/> like smoke or clouds. </summary>
    public interface IVolumetricMaterial : IMaterial {
        /// <summary> The density of the (reactive molecules in the) <see cref="IVolumetricMaterial"/> </summary>
        double Density { get; }

        /// <summary> Get a distance distribution of a <paramref name="ray"/> through the <see cref="IVolumetricMaterial"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="interval">The <see cref="IShapeInterval"/> of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance distribution of the <paramref name="ray"/> through the <see cref="IVolumetricMaterial"/></returns>
        IDistanceDistribution? IMaterial.DistanceDistribution(IRay ray, ISpectrum spectrum, IShapeInterval interval) {
            if (interval.Exit > 0 && interval.Entry < ray.Length) {
                return new ExponentialDistanceDistribution(Math.Max(0, interval.Entry), Math.Min(ray.Length, interval.Exit), Density, this, interval);
            } else {
                return null;
            }
        }

        /// <summary> Get a <see cref="Position3"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to find a position at</param>
        /// <param name="interval">The <see cref="IShapeInterval"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The distance along the <paramref name="ray"/></param>
        /// <returns>The <see cref="Position3"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        Position3 IMaterial.GetPosition(IRay ray, IShapeInterval interval, Position1 distance) => ray.Travel(distance);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="position"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="position">The <see cref="Position3"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="orientation">The <see cref="IVolumetricMaterial"/>s orientation at the specified <paramref name="position"/></param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="position"/> with the specified <paramref name="direction"/></returns>
        IRay IMaterial.CreateRay(Position3 position, Normal3 normal, Normal3 direction) => new Ray(position, direction);
    }
}
