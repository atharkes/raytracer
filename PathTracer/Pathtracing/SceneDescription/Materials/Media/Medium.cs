using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.Spectra;
using System;

namespace PathTracer.Pathtracing.SceneDescription.Materials.Media {
    public class Medium : IMedium {
        public ISpectrum Albedo { get; }
        public double Roughness { get; }
        /// <summary> The refractive index of this primitive if it is a dielectric. This is typically a value between 1 and 3.
        /// <para> Vacuum 1 </para>
        /// <para> Gases at 0 °C: Air 1.000293, Helium 1.000036, Hydrogen 1.000132, Carbon dioxide 1.00045 </para>
        /// <para> Liquids at 20 °C: Water 1.333, Ethanol 1.36, Olive oil 1.47 </para>
        /// <para> Solids: Ice 1.31, Fused silica(quartz) 1.46, Plexiglas 1.49, Window glass 1.52, 
        /// Flint glass 1.62, Sapphire 1.77, Cubic zirconia 2.15, Diamond 2.42, Moissanite 2.65 </para>
        /// </summary>
        public double RefractiveIndex { get; } = 1f;
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
            return other is not null && RefractiveIndex.Equals(other.RefractiveIndex);
        }

        public IProbabilityDistribution<Normal3> DirectionDistribution(Normal3 incomingDirection, Position3 position, Normal3 orientation, ISpectrum spectrum) {
            throw new NotImplementedException("Fresnel requires information of other media. Some overarching structure needs to accomodate for the other media.");
        }

        public ISpectrum Emittance(Position3 position, Normal3 orientation, Normal3 direction) => ISpectrum.Black;
    }
}
