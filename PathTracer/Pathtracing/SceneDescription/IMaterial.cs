using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> The albedo <see cref="ISpectrum"/> of the <see cref="IMaterial"/> </summary>
        ISpectrum Albedo { get; }

        /// <summary> Get a distance PDF of a <paramref name="ray"/> traced through the <see cref="IMaterial"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="IMaterial"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A scattering distance PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IDistancePDF DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        /// <summary> Get a distance-material PDF of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance-material PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IDistanceMaterialPDF DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        /// <summary> Get a material PDF for a scattering <paramref name="distance"/> of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The scattering distance</param>
        /// <returns>A <see cref="IMaterial"/> PDF at the scattering distance</returns>
        IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance);


        /// <summary> Get an outgoing direction PDF for an <paramref name="incomingDirection"/> at a <paramref name="surfacePoint"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="surfacePoint"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="surfacePoint"/> </param>
        /// <param name="surfacePoint">The <see cref="ISurfacePoint"/> at which the scattering occurs</param>
        /// <returns>A scattering direction PDF</returns>
        IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);

        /// <summary> Get a direction-medium PDF at a <paramref name="surfacePoint"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="surfacePoint"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="surfacePoint"/></param>
        /// <param name="surfacePoint">The <see cref="ISurfacePoint"/> at which the scattering occurs</param>
        /// <returns>A direction-medium PDF</returns>
        IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);

        /// <summary> Get a medium PDF at a surfacePoint for an <paramref name="outgoingDirection"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="surfacePoint"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="surfacePoint"/></param>
        /// <param name="surfacePoint">The <see cref="ISurfacePoint"/> at which the scattering occurs</param>
        /// <param name="outgoingDirection">The outgoing direction</param>
        /// <returns>A medium PDF</returns>
        IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection);

        /// <summary> Create an outgoing <see cref="IRay"/> from a <paramref name="surfacePoint"/> along a specified <paramref name="direction"/> </summary>
        /// <param name="surfacePoint">The <see cref="ISurfacePoint"/> from which the <see cref="IRay"/> leaves</param>
        /// <param name="direction">The outgoing direction of the <see cref="IRay"/></param>
        /// <returns>An <see cref="IRay"/> from the <paramref name="surfacePoint"/> with the specified <paramref name="direction"/></returns>
        IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction);
    }
}
