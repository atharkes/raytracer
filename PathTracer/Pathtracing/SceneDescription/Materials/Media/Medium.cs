using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Direction;
using PathTracer.Pathtracing.SceneDescription.Materials.SurfaceMaterials;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Media {
    public class Medium : SurfaceMaterial, IMedium {
        public double RefractiveIndex { get; }
        public double Priority { get; }

        public override bool IsEmitting => false;

        public override bool IsSensing => false;

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

        public override IDirectionDistribution? DirectionDistribution(Normal3 incomingDirection, Position3 position, ISpectrum spectrum) {
            throw new NotImplementedException("Fresnel requires information of other media. Some overarching structure needs to accomodate for the other media.");
        }

        public override ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) => ISpectrum.Black;
    }
}
