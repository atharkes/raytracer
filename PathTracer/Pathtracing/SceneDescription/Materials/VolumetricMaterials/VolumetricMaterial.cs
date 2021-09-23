using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.PDFs.DistancePDFs;
using PathTracer.Spectra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathTracer.Pathtracing.SceneDescription.Materials.VolumetricMaterials {
    public class VolumetricMaterial : Material, IVolumetricMaterial {
        public double Density { get; }

        public VolumetricMaterial(ISpectrum albedo, double density) : base(albedo) {
            Density = density;
        }

        public override IRay CreateRay(ISurfacePoint surfacePoint, Vector3 direction) {
            return new Ray(surfacePoint.Position, direction);
        }


        public override IDistancePDF DistancePDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) {
            double entryDistance = 0;
            IDistancePDF? result = null;
            foreach (IBoundaryPoint boundaryPoint in boundaryPoints.OrderBy(b => b.Distance)) {
                if (boundaryPoint.IsEntered(ray)) {
                    entryDistance = boundaryPoint.Distance;
                } else if (boundaryPoint.IsExited(ray)) {
                    result += new ExponentialDistancePDF(entryDistance, boundaryPoint.Distance, Density);
                }
            }
            if (result == null) throw new ArgumentException("No intersection interval could be found");
            return result;
        }

        public override IDistanceMaterialPDF DistanceMaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints) {
            throw new NotImplementedException();
        }

        public override IPDF<IMaterial> MaterialPDF(IRay ray, ISpectrum spectrum, IEnumerable<IBoundaryPoint> boundaryPoints, float distance) {
            throw new NotImplementedException();
        }
        

        public override IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            throw new NotImplementedException();
        }

        public override IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection) {
            throw new NotImplementedException();
        }

        public override IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            throw new NotImplementedException();
        }
    }
}
