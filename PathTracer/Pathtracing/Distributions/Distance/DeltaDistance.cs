using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct DeltaDistance : IDistanceDistribution, IEquatable<DeltaDistance> {
        public Position1 Distance { get; }
        public IInterval Domain => new Interval(Distance, Distance);

        public DeltaDistance(Position1 distance) {
            Distance = distance;
        }

        /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
        /// <param name="distance">The distance to get the material density at</param>
        /// <returns>The material density at the specified <paramref name="distance"/></returns>
        public double MaterialDensity(Position1 distance) => double.PositiveInfinity;

        public Position1 Sample(Random random) => Distance;

        public double ProbabilityDensity(Position1 sample) => sample.Equals(Distance) ? double.PositiveInfinity : 0d;

        public double CumulativeProbability(Position1 sample) => sample >= Distance ? 1d : 0d;

        public override bool Equals(object? obj) => obj is DeltaDistance dd && Equals(dd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is DeltaDistance dd && Equals(dd);
        public bool Equals(DeltaDistance other) => Distance.Equals(other.Distance);
        public override int GetHashCode() => HashCode.Combine(818834969, Distance);
        public override string ToString() => $"Delta{Distance}";

        public static bool operator ==(DeltaDistance left, DeltaDistance right) => left.Equals(right);
        public static bool operator !=(DeltaDistance left, DeltaDistance right) => !(left == right);
    }
}
