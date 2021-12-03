using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Boundaries;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Pathtracing.SceneDescription;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Distance {
    /// <summary> A recursive <see cref="ICDF{T}"/> </summary>
    /// <typeparam name="Position1">The type of the samples of the <see cref="IRecursiveDistanceDistribution{T}"/> </typeparam>
    public interface IRecursiveDistanceDistribution : IDistanceDistribution {
        /// <summary> The left <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        IDistanceDistribution Left { get; }
        /// <summary> The right <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        IDistanceDistribution Right { get; }

        /// <summary> Whether the <see cref="IRecursiveDistanceDistribution{T}"/> contains a delta distribution </summary>
        bool IPDF.ContainsDelta => Left.ContainsDelta || Right.ContainsDelta;
        /// <summary> The minimum <see cref="Position1"/> in the domain of the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        Position1 ICDF<Position1>.Minimum => Utils.Min(Left.Minimum, Right.Minimum);
        /// <summary> The maximum <see cref="Position1"/> in the domain of the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        Position1 ICDF<Position1>.Maximum => Utils.Min(Left.Maximum, Right.Maximum);
        /// <summary> The size of the domain is 2; left and right </summary>
        double IProbabilityDistribution.DomainSize => throw new NotImplementedException("Union of intervals is not implemented");

        /// <summary> Sample the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="Position1"/></returns>
        Position1 IProbabilityDistribution<Position1>.Sample(Random random) {
            Position1 left = Left.Sample(random);
            if (left.CompareTo(Right.Minimum) <= 0) {
                return left;
            } else {
                return Utils.Min(left, Right.Sample(random));
            }
        }

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        /// <param name="sample">The sample to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        double IPDF<Position1>.ProbabilityDensity(Position1 sample) {
            if (!Contains(sample)) {
                return 0;
            } else {
                double pLeft = Left.ProbabilityDensity(sample);
                double pRight = Right.ProbabilityDensity(sample);
                if (pLeft <= 0 && pRight <= 0) {
                    return 0;
                } else if (pLeft <= 0) {
                    return (1 - Left.CumulativeProbabilityDensity(sample)) * pRight;
                } else if (pRight <= 0) {
                    return (1 - Right.CumulativeProbabilityDensity(sample)) * pLeft;
                } else {
                    return (1 - Left.CumulativeProbabilityDensity(sample)) * pRight + (1 - Right.CumulativeProbabilityDensity(sample)) * pLeft;
                }
            }
        }

        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double ICDF<Position1>.CumulativeProbabilityDensity(Position1 sample) {
            if (After(sample)) {
                return 0;
            } else {
                double l = Left.CumulativeProbabilityDensity(sample);
                double r = Right.CumulativeProbabilityDensity(sample);
                return l + r - l * r;
            }
        }
    }

    public class RecursiveDistanceDistribution : IRecursiveDistanceDistribution, IEquatable<RecursiveDistanceDistribution> {
        public IDistanceDistribution Left { get; }
        public IDistanceDistribution Right { get; }

        public RecursiveDistanceDistribution(IDistanceDistribution left, IDistanceDistribution right) {
            Left = left;
            Right = right;
        }

        public WeightedPMF<IMaterial>? GetMaterials(Position1 sample) {
            var left = Left.GetMaterials(sample);
            var right = Right.GetMaterials(sample);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IMaterial>((left, probabilityLeft), (right, probabilityRight));
            }
        }

        public WeightedPMF<IShapeInterval>? GetShapeIntervals(Position1 sample, IMaterial material) {
            var left = Left.GetShapeIntervals(sample, material);
            var right = Right.GetShapeIntervals(sample, material);
            if (left is null) {
                return right;
            } else if (right is null) {
                return left;
            } else {
                double probabilityLeft = (1 - Right.CumulativeProbabilityDensity(sample)) * Left.ProbabilityDensity(sample);
                double probabilityRight = (1 - Left.CumulativeProbabilityDensity(sample)) * Right.ProbabilityDensity(sample);
                return new WeightedPMF<IShapeInterval>((left, probabilityLeft), (right, probabilityRight));
            }
        }

        public override bool Equals(object? obj) => obj is RecursiveDistanceDistribution rdd && Equals(rdd);
        public bool Equals(IProbabilityDistribution<Position1>? other) => other is RecursiveDistanceDistribution rdd && Equals(rdd);
        public bool Equals(RecursiveDistanceDistribution? other) => other is not null && Left.Equals(other.Left) && Right.Equals(other.Right);
        public override int GetHashCode() => HashCode.Combine(648696017, Left, Right);

        public static bool operator ==(RecursiveDistanceDistribution left, RecursiveDistanceDistribution right) => left.Equals(right);
        public static bool operator !=(RecursiveDistanceDistribution left, RecursiveDistanceDistribution right) => !(left == right);
    }
}
