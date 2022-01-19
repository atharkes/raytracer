using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct DeltaDistance : IDistanceDistribution, IEquatable<DeltaDistance> {
        public Position1 Distance { get; }
        public IInterval Domain => new Interval(Distance, Distance);

        public DeltaDistance(Position1 distance) {
            Distance = distance;
        }

        public Position1 Sample(Random random) => Distance;

        public double ProbabilityDensity(Position1 sample) => sample.Equals(Distance) ? 1d / Domain.CoveredArea : 0;

        public double CumulativeProbabilityDensity(Position1 sample) => sample >= Distance ? 1 : 0;

        public override bool Equals(object? obj) => obj is DeltaDistance dd && Equals(dd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is DeltaDistance dd && Equals(dd);
        public bool Equals(DeltaDistance other) => Distance.Equals(other.Distance);
        public override int GetHashCode() => HashCode.Combine(818834969, Distance);

        public static bool operator ==(DeltaDistance left, DeltaDistance right) => left.Equals(right);
        public static bool operator !=(DeltaDistance left, DeltaDistance right) => !(left == right);
    }
}
