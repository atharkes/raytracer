using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct UniformInterval : IDistanceDistribution {
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }
        public Position1 Minimum { get; }
        public Position1 Maximum { get; }
        public double DomainSize => Maximum - Minimum;
        public bool ContainsDelta => true;

        public UniformInterval(Position1 start, Position1 end, IMaterial material, IShapeInterval interval) {
            Minimum = start;
            Maximum = end;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            return Minimum + (float)(random.NextDouble() * DomainSize);
        }

        public double ProbabilityDensity(Position1 sample) {
            return 1 / DomainSize;
        }

        public double CumulativeProbabilityDensity(Position1 sample) {
            if (sample < Minimum) {
                return 0;
            } else if (sample < Maximum) {
                return (sample - Minimum) / DomainSize;
            } else {
                return 1;
            }
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            return (this as IPDF<Position1>).Contains(sample) ? new WeightedPMF<IMaterial>((Material, 1)) : null;
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            return (this as IPDF<Position1>).Contains(sample) && material.Equals(Material) ? new WeightedPMF<IShapeInterval>((Interval, 1)) : null;
        }

        public override bool Equals(object? obj) => obj is UniformInterval ud && Equals(ud);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is UniformInterval ud && Equals(ud);
        public bool Equals(UniformInterval other) => Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum) && Material.Equals(other.Material) && Interval.Equals(other.Interval);
        public override int GetHashCode() => HashCode.Combine(963929819, Minimum, Maximum, Material, Interval);

        public static bool operator ==(UniformInterval left, UniformInterval right) => left.Equals(right);
        public static bool operator !=(UniformInterval left, UniformInterval right) => !(left == right);
    }
}
