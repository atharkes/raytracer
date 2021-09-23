using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public abstract IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction);

        public abstract IDistancePDF DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);
        public abstract IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance);
        public abstract IDistanceMaterialPDF DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        public abstract IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
        public abstract IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection);
        public abstract IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
        
    }
}
