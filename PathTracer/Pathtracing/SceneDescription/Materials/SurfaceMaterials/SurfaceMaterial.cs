﻿using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;

namespace PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials {
    /// <summary>
    /// A surface <see cref="IMaterial"/>.
    /// To prevent self-intersection issues <see cref="IRay"/>s are raised from their <see cref="ISurfacePoint"/> on creation.
    /// </summary>
    public abstract class SurfaceMaterial : Material, IMaterial {
        /// <summary> Create a new <see cref="SurfaceMaterial"/> using a specified <paramref name="albedo"/> </summary>
        /// <param name="albedo">The albedo <see cref="ISpectrum"/> of the new <see cref="SurfaceMaterial"/></param>
        public SurfaceMaterial(ISpectrum albedo) : base(albedo) { }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position + surfacePoint.Normal * 0.001f, direction);
        }

        public override IDistanceMaterialPDF? DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IBoundary boundary) {
            IBoundaryPoint? entry = boundary.FirstEntry(0, ray.Length);
            if (entry is not null) {
                return new SingleDistanceMaterialPDF(new DistanceMaterial(entry.Distance, this));
            } else {
                return null;
            }
        }
    }
}
