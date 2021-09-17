using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.SceneDescription.Materials;
using PathTracer.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription {
    /// <summary> An interface for a material of a <see cref="ISceneObject"/> </summary>
    public interface IMaterial {
        /// <summary> Get a distance and distance-material PDF of a <paramref name="ray"/> traced through the <see cref="IMaterial"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="IMaterial"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A scattering distance PDF, and a distance-material PDF</returns>
        (IPDF<float>, IPDF<float, IMaterial>) DistancePDFs(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        /// <summary> Get a distance PDF of a <paramref name="ray"/> traced through the <see cref="IMaterial"/> </summary>
        /// <param name="ray">The <see cref="IRay"/> to trace through the <see cref="IMaterial"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A scattering distance PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IPDF<float> DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        /// <summary> Get a distance-material PDF of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <returns>A distance-material PDF of the <paramref name="ray"/> through the <see cref="IMaterial"/></returns>
        IPDF<float, IMaterial> DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        /// <summary> Get a material PDF for a scattering <paramref name="distance"/> of a <paramref name="ray"/> </summary>
        /// <param name="ray">The scattering <see cref="IRay"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of the <paramref name="ray"/></param>
        /// <param name="boundaryPoints">The <see cref="IBoundaryPoint"/>s of the <see cref="IMaterial"/> along the <paramref name="ray"/></param>
        /// <param name="distance">The scattering distance</param>
        /// <returns>A <see cref="IMaterial"/> PDF at the scattering distance</returns>
        IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance);


        /// <summary> Get an outgoing direction and direction-medium PDF at a <paramref name="surfacePoint"/> </summary>
        /// <param name="incomingDirection">The incoming direction at the <paramref name="surfacePoint"/></param>
        /// <param name="spectrum">The <see cref="ISpectrum"/> of light scattering at the <paramref name="surfacePoint"/></param>
        /// <param name="surfacePoint">The <see cref="ISurfacePoint"/> at which the scattering occurs</param>
        /// <returns>An outgoing direction and direction-medium PDF</returns>
        (IPDF<Vector3>, IPDF<Vector3, IMedium>) DirectionalPDFs(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);

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


        ISpectrum Absorb(Vector3 direction, ISurfacePoint surfacePoint, ISpectrum spectrum);
        ISpectrum Emit(ISurfacePoint surfacePoint, Vector3 direction);
    }
}
