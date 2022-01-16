using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct ExponentialInterval : IDistanceDistribution, IEquatable<ExponentialInterval> {
        public IInterval Interval { get; }
        public double Density { get; }
        public Exponential Distribution { get; }
        
        public Position1 Minimum => Interval.Entry;
        public Position1 Maximum => Position1.PositiveInfinity;
        public double DomainSize => throw new NotImplementedException("Domains is partly continuous and partly discreet");
        public bool ContainsDelta => true;

        readonly IMaterial material;
        readonly IShapeInterval shapeInterval;

        public ExponentialInterval(IInterval interval, double density, IMaterial material, IShapeInterval shapeInterval) {
            Density = density;
            Distribution = new Exponential(density);
            Interval = interval;
            this.material = material;
            this.shapeInterval = shapeInterval;
        }

        public Position1 Sample(Random random) {
            Position1 distance = Interval.Entry + (float)Distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= Interval.Exit ? distance : Position1.PositiveInfinity;
        }

        public double ProbabilityDensity(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return 1 - Distribution.CumulativeDistribution(DomainSize);
            } else if (Interval.Includes(sample)) {
                return Distribution.Density(sample - Interval.Entry);
            } else {
                return 0;
            }
        }

        double IProbabilityDistribution<Position1>.RelativeProbability(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return ProbabilityDensity(sample) * 2;
            } else if (Interval.Includes(sample)) {
                return ProbabilityDensity(sample) * 2 * Interval.Size;
            } else {
                return 0;
            }
        }

        public double CumulativeProbabilityDensity(Position1 distance) {
            if (distance < Minimum) {
                return 0;
            } else if (distance < Interval.Exit) {
                return Distribution.CumulativeDistribution(distance - Interval.Entry);
            } else if (distance < double.PositiveInfinity) {
                return Distribution.CumulativeDistribution(Interval.Size);
            } else {
                return 1;
            }
        }

        public bool Contains(Position1 sample) => Interval.Includes(sample) || sample.Equals(Position1.PositiveInfinity);

        public bool Contains(IMaterial material) => material.Equals(this.material);

        public bool Contains(IShapeInterval interval) => interval.Equals(Interval);

        public WeightedPMF<IMaterial> GetMaterials(Position1 sample) => new((material, 1));

        public WeightedPMF<IShapeInterval> GetShapeIntervals(Position1 sample, IMaterial material) => new((shapeInterval, 1));

        public override bool Equals(object? obj) => obj is ExponentialInterval ed && Equals(ed);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is ExponentialInterval ed && Equals(ed);
        public bool Equals(ExponentialInterval other) => Density.Equals(other.Density) && Interval.Equals(other.Interval) && material.Equals(other.material) && Interval.Equals(other.Interval);
        public override int GetHashCode() => HashCode.Combine(973102703, Density, Interval, material, Interval);

        public static bool operator ==(ExponentialInterval left, ExponentialInterval right) => left.Equals(right);
        public static bool operator !=(ExponentialInterval left, ExponentialInterval right) => !(left == right);
    }
}
