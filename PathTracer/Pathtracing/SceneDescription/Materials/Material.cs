using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials {
    public abstract class Material : IMaterial {
        public ISpectrum Albedo { get; }

        public Material(ISpectrum albedo) {
            Albedo = albedo;
        }

        public abstract IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction);
        public abstract IDistanceMaterialPDF? DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary);
        public abstract IPDF<Vector3, IMedium>? DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint);
    }
}
