using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Spectra;
using System.Collections.Generic;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public ISpectrum Absorb(Vector3 direction, ISurfacePoint surfacePoint, ISpectrum spectrum) {
            return spectrum * Albedo;
        }

        public abstract ISpectrum Emit(ISurfacePoint surfacePoint, Vector3 direction);

        public (IPDF<float>, IPDF<float, IMaterial>) DistancePDFs(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) {
            return (DistancePDF(ray, spectrum, boundaryPoints), DistanceMaterialPDF(ray, spectrum, boundaryPoints));
        }

        public abstract IPDF<float> DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);
        public abstract IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance);
        public abstract IPDF<float, IMaterial> DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints);

        public (IPDF<Vector3>, IPDF<Vector3, IMedium>) DirectionalPDFs(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            return (DirectionPDF(incomingDirection, spectrum, surfacePoint), DirectionMediumPDF(incomingDirection, spectrum, surfacePoint));
        }

        public abstract IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
        public abstract IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection);
        public abstract IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
    }
}
