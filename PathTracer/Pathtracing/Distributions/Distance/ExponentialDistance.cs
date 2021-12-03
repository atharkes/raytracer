using MathNet.Numerics.Distributions;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    public struct ExponentialDistance : IDistanceDistribution, IEquatable<ExponentialDistance> {
        public double Density { get; }
        public Exponential Distribution { get; }
        public Position1 Entry { get; }
        public Position1 Exit { get; }
        public Direction1 ExponentialSize => Exit - Entry;
        public IMaterial Material { get; }
        public IShapeInterval Interval { get; }

        public Position1 Minimum => Entry;
        public Position1 Maximum => Position1.PositiveInfinity;
        public double DomainSize => ExponentialSize;
        public bool ContainsDelta => true;

        public ExponentialDistance(Position1 start, Position1 end, double density, IMaterial material, IShapeInterval interval) {
            Density = density;
            Distribution = new Exponential(density);
            Entry = start;
            Exit = end;
            Material = material;
            Interval = interval;
        }

        public Position1 Sample(Random random) {
            Position1 distance = Entry + (float)Distribution.InverseCumulativeDistribution(random.NextDouble());
            return distance <= Exit ? distance : Position1.PositiveInfinity;
        }

        public double ProbabilityDensity(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return 1 - Distribution.CumulativeDistribution(DomainSize);
            } else if (Entry <= sample && sample <= Exit) {
                return Distribution.Density(sample - Entry);
            } else {
                return 0;
            }
        }

        double IProbabilityDistribution<Position1>.RelativeProbability(Position1 sample) {
            if (sample == Position1.PositiveInfinity) {
                return ProbabilityDensity(sample) * DomainSize;
            } else if (Entry <= sample && sample <= Exit) {
                return ProbabilityDensity(sample) * DomainSize * ExponentialSize;
            } else {
                return 0;
            }
        }

        public double CumulativeProbabilityDensity(Position1 distance) {
            if (distance < Minimum) {
                return 0;
            } else if (distance < Exit) {
                return Distribution.CumulativeDistribution(distance - Entry);
            } else if (distance < double.PositiveInfinity) {
                return Distribution.CumulativeDistribution(Exit - Entry);
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

        public override bool Equals(object? obj) => obj is ExponentialDistance ed && Equals(ed);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is ExponentialDistance ed && Equals(ed);
        public bool Equals(ExponentialDistance other) => Density.Equals(other.Density) && Entry.Equals(other.Entry) && Exit.Equals(other.Exit) && Material.Equals(other.Material) && Interval.Equals(other.Interval);
        public override int GetHashCode() => HashCode.Combine(973102703, Density, Entry, Exit, Material, Interval);

        public static bool operator ==(ExponentialDistance left, ExponentialDistance right) => left.Equals(right);
        public static bool operator !=(ExponentialDistance left, ExponentialDistance right) => !(left == right);
    }
}
