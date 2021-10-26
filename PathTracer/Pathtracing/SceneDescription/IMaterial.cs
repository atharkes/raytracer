using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Distributions.Distance;
using PathTracer.Pathtracing.Points;
using PathTracer.Pathtracing.Points.Boundaries;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.Spectra;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> The albedo <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        ISpectrum Albedo { get; }

        /// <summary> Whether the <see cref="IMaterial"/> is emitting light or not </summary>
        bool IsEmitting { get; }
        /// <summary> Whether the <see cref="IMaterial"/> is sensing light or not </summary>
        bool IsSensing { get; }

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="surfacePoint"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="surfacePoint">The <see cref="IMaterialPoint1"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="surfacePoint"/> with the specified <paramref name="direction"/></returns>
        IRay CreateRay(IMaterialPoint1 surfacePoint, Vector3 direction);

        /// <summary> Create a <see cref="IMaterialPoint1"/> at a specified <paramref name="distance"/> along a <paramref name="ray"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> that finds a <see cref="IMaterialPoint1"/></param>
        /// <param name="interval">The <see cref="IBoundaryInterval"/> in which the <see cref="IMaterialPoint1"/> is found</param>
        /// <param name="distance">The specified distance at which the <see cref="IMaterialPoint1"/> is found</param>
        /// <returns>An <see cref="IMaterialPoint1"/> at the specified <paramref name="distance"/> along the <paramref name="ray"/></returns>
        IMaterialPoint1 CreateSurfacePoint(IRay ray, IBoundaryInterval interval, float distance);

        /// <summary> Get a distance-material PDF of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundary">The <see cref="IBoundary"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance-material PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IDistanceDistribution? DistanceDistribution(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary);

        /// <summary> Get a direction-medium PDF at a <paramref name="surfacePoint"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="surfacePoint"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="surfacePoint"/></param>
        /// <param name="surfacePoint">The <see cref="IMaterialPoint1"/> at which the scattering occurs</param>
        /// <returns>A direction-medium PDF</returns>
        IDirectionDistribution? DirectionDistribution(Vector3 incomingDirection, ISpectrum spectrum, IMaterialPoint1 surfacePoint);
    }
}
