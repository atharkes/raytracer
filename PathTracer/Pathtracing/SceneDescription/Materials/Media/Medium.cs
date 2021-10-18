using OpenTK.Mathematics;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials;
using PathTracer.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Media {
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

        public override IPDF<Vector3, IMedium> DirectionDistribution(Vector3 incomingDirection, ISpectrum spectrum, ISurfacePoint surfacePoint) {
            throw new NotImplementedException("Fresnel requires information of other media. Some overarching structure needs to accomodate for the other media.");
        }
    }
}
