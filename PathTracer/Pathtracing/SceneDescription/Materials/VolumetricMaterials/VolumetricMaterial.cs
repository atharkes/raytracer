using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials {
    public abstract class VolumetricMaterial : Material, IVolumetricMaterial {
        public double Density { get; }

        public VolumetricMaterial(ISpectrum albedo, double density) : base(albedo) {
            Density = density;
        }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position, direction);
        }

        public override IDistanceMaterialPDF? DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IBoundary boundary) {
            IDistanceMaterialPDF? result = null;
            foreach (var (Entry, Exit) in boundary.PassthroughIntervals()) {
                if (Exit.Distance > 0 && Entry.Distance < ray.Length) {
                    result += new ExponentialDistanceMaterialPDF(Math.Max(0, Entry.Distance), Math.Min(ray.Length, Exit.Distance), Density, this);
                }
            }
            return result;
        }
    }
}
