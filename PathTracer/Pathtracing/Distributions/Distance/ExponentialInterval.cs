using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct ExponentialInterval : IDistanceDistribution, IEquatable<ExponentialInterval> {
        public IInterval Domain { get; }
        public double Density { get; }

        readonly Exponential distribution;

        public ExponentialInterval(IInterval interval, double density) {
            Domain = interval;
            Density = density;
            distribution = new Exponential(density);
        }

        public Position1 Sample(Random random) {
            Position1 distance = Domain.Entry + (float)distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= Domain.Exit ? distance : Position1.PositiveInfinity;
        }

        public double ProbabilityDensity(Position1 sample) {
            return Domain.Includes(sample) ? distribution.Density(sample - Domain.Entry) : 0;
        }

        public double CumulativeProbability(Position1 distance) {
            if (distance < Domain.Entry) {
                return 0;
            } else if (distance < Domain.Exit) {
                return distribution.CumulativeDistribution(distance - Domain.Entry);
            } else {
                return distribution.CumulativeDistribution(Domain.CoveredArea);
            }
        }

        public override bool Equals(object? obj) => obj is ExponentialInterval ed && Equals(ed);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is ExponentialInterval ed && Equals(ed);
        public bool Equals(ExponentialInterval other) => Domain.Equals(other.Domain) && Density.Equals(other.Density);
        public override int GetHashCode() => HashCode.Combine(973102703, Domain, Density);

        public static bool operator ==(ExponentialInterval left, ExponentialInterval right) => left.Equals(right);
        public static bool operator !=(ExponentialInterval left, ExponentialInterval right) => !(left == right);
    }
}
