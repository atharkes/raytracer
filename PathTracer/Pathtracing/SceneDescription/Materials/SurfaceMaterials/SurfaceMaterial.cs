using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="ISurfacePoint"/> on creation.
    /// </summary>
    public class SurfaceMaterial : Material, IMaterial {
        /// <summary> Create a new <see cref="SurfaceMaterial"/> using a specified <paramref name="albedo"/> </summary>
        /// <param name="albedo">The albedo <see cref="ISpectrum"/> of the new <see cref="SurfaceMaterial"/></param>
        public SurfaceMaterial(ISpectrum albedo) : base(albedo) { }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position + surfacePoint.Normal * 0.001f, direction);
        }

        public override IDistancePDF DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) {
            double distance = boundaryPoints.Min()?.Distance ?? throw new ArgumentException("There are no boundarypoints to supply intersection information.");
            return new SingleDistancePDF(distance);
        }

        public override IDistanceMaterialPDF DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) {
            double distance = boundaryPoints.Min()?.Distance ?? throw new ArgumentException("There are no boundarypoints to supply intersection information.");
            return new SingleDistanceMaterialPDF(new DistanceMaterial(distance, this));
        }

        public abstract IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance);

        public abstract IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
        public abstract IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
        public abstract IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection);
    }
}
