using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct UniformInterval : IDistanceDistribution {
        public IInterval Domain { get; }

        public UniformInterval(IInterval interval) {
            Domain = interval;
        }

        public Position1 Sample(Random random) {
            return Domain.Entry + (float)(random.NextDouble() * Domain.CoveredArea);
        }

        public double ProbabilityDensity(Position1 sample) {
            return 1d / Domain.CoveredArea;
        }

        public double CumulativeProbability(Position1 sample) {
            return Math.Min(Math.Max(0d, sample - Domain.Entry), Domain.CoveredArea) / Domain.CoveredArea;
        }

        public override bool Equals(object? obj) => obj is UniformInterval ud && Equals(ud);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is UniformInterval ud && Equals(ud);
        public bool Equals(UniformInterval other) => Domain.Equals(other.Domain);
        public override int GetHashCode() => HashCode.Combine(963929819, Domain);

        public static bool operator ==(UniformInterval left, UniformInterval right) => left.Equals(right);
        public static bool operator !=(UniformInterval left, UniformInterval right) => !(left == right);
    }
}
