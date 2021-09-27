using OpenTK.Mathematics;
using PathTracer.Pathtracing.Boundaries;
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

        public override IDistanceMaterialPDF? DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IBoundaryCollection boundary) {
            IDistanceMaterialPDF? result = null;
            foreach (IBoundaryInterval interval in boundary.BoundaryIntervals) {
                if (interval.Exit.Distance > 0 && interval.Entry.Distance < ray.Length) {
                    result += new ExponentialDistanceMaterialPDF(Math.Max(0, interval.Entry.Distance), Math.Min(ray.Length, interval.Exit.Distance), Density, this);
                }
            }
            return result;
        }
    }
}
