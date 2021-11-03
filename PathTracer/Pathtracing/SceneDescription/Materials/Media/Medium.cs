using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Media {
    public class Medium : IMedium {
        public ISpectrum Albedo { get; }
        public double RefractiveIndex { get; }
        public double Priority { get; }
        public bool IsEmitting => false;
        public bool IsSensing => false;

        public Medium(ISpectrum albedo, double refractiveIndex, double priority) {
            Albedo = albedo;
            RefractiveIndex = refractiveIndex;
            Priority = priority;
        }

        public int CompareTo(IMedium? other) {
            return Priority.CompareTo(other?.Priority);
        }

        public bool Equals(IMedium? other) {
            return RefractiveIndex.Equals(other?.RefractiveIndex);
        }

        public IPDF<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            throw new NotImplementedException("Fresnel requires information of other media. Some overarching structure needs to accomodate for the other media.");
        }

        public ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) => ISpectrum.Black;
    }
}
