using OpenTK.Mathematics;
using PathTracer.Pathtracing.PDFs;
using PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials;
using PathTracer.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Mediums {
    public class Medium : SurfaceMaterial, IMedium {
        public double RefractiveIndex { get; }
        public double Priority { get; }

        public Medium(ISpectrum albedo, double refractiveIndex, double priority) : base(albedo) {
            RefractiveIndex = refractiveIndex;
            Priority = priority;
        }

        public int CompareTo(IMedium? other) {
            return Priority.CompareTo(other?.Priority);
        }

        public bool Equals(IMedium? other) {
            return RefractiveIndex.Equals(other?.RefractiveIndex);
        }

        public override IPDF<Vector3> DirectionPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            throw new NotImplementedException();
        }

        public override IPDF<Vector3, IMedium> DirectionMediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            throw new NotImplementedException();
        }

        public override IPDF<IMedium> MediumPDF(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint, Vector3 outgoingDirection) {
            throw new NotImplementedException();
        }

        
    }
}
