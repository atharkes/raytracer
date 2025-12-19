using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Distributions.Intervals;
using PathTracer.Pathtracing.Distributions.Probabilities;
using PathTracer.Utilities;

namespace PathTracer.Pathtracing.Distributions.Distance;

/// <summary> A recursive <see cref="IDistanceDistribution"/> </summary>
public class RecursiveDistanceDistribution : IDistanceDistribution, IEquatable<RecursiveDistanceDistribution> {
    /// <summary> The left <see cref="IDistanceDistribution"/> </summary>
    public IDistanceDistribution Left { get; }
    /// <summary> The right <see cref="IDistanceDistribution"/> </summary>
    public IDistanceDistribution Right { get; }

    /// <summary> The interval the <see cref="RecursiveDistanceDistribution"/> covers </summary>
    public IInterval Domain => new IntervalCollection(Left.Domain, Right.Domain);
    /// <summary> Whether the <see cref="IRecursiveDistanceDistribution{T}"/> contains a delta distribution </summary>
    bool IPDF.ContainsDelta => Left.ContainsDelta || Right.ContainsDelta;

    public RecursiveDistanceDistribution(IDistanceDistribution left, IDistanceDistribution right) {
        Left = left;
        Right = right;
    }

    /// <summary> Get the material density at the specified <paramref name="distance"/> </summary>
    /// <param name="distance">The distance to get the material density at</param>
    /// <returns>The material density at the specified <paramref name="distance"/></returns>
    public double MaterialDensity(Position1 distance) => Left.MaterialDensity(distance) + Right.MaterialDensity(distance);

    /// <summary> Sample the <see cref="IRecursiveDistanceDistribution{T}"/> </summary>
    /// <param name="random">The <see cref="Random"/> to use for sampling</param>
    /// <returns>A <paramref name="random"/> <see cref="Position1"/></returns>
    public Position1 Sample(Random random) {
        var left = Left.Sample(random);
        return left.CompareTo(Right.Domain.Entry) <= 0 ? left : Utils.Min(left, Right.Sample(random));
    }

    /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="RecursiveDistanceDistribution"/> </summary>
    /// <param name="sample">The sample to get the probability for</param>
    /// <returns>The probability of the <paramref name="sample"/></returns>
    public double ProbabilityDensity(Position1 sample) {
        if (!(this as IDistanceDistribution).Contains(sample)) {
            return 0;
        } else {
            var pLeft = Left.ProbabilityDensity(sample);
            var pRight = Right.ProbabilityDensity(sample);
            return pLeft <= 0 && pRight <= 0
                ? 0
                : pLeft <= 0
                    ? (1 - Left.CumulativeProbability(sample)) * pRight
                    : pRight <= 0
                                    ? (1 - Right.CumulativeProbability(sample)) * pLeft
                                    : (1 - Left.CumulativeProbability(sample)) * pRight + (1 - Right.CumulativeProbability(sample)) * pLeft;
        }
    }

    /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="RecursiveDistanceDistribution"/> </summary>
    /// <param name="sample">The sample to get the cummulative probability for</param>
    /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
    public double CumulativeProbability(Position1 sample) {
        if (sample < Domain.Entry) {
            return 0;
        } else {
            var l = Left.CumulativeProbability(sample);
            var r = Right.CumulativeProbability(sample);
            return l + r - l * r;
        }
    }

    public override bool Equals(object? obj) => obj is RecursiveDistanceDistribution rdd && Equals(rdd);
    public bool Equals(IProbabilityDistribution<Position1>? other) => other is RecursiveDistanceDistribution rdd && Equals(rdd);
    public bool Equals(RecursiveDistanceDistribution? other) => other is not null && Left.Equals(other.Left) && Right.Equals(other.Right);
    public override int GetHashCode() => HashCode.Combine(648696017, Left, Right);
    public override string ToString() => $"Recursive[{string.Join(',', Left, Right)}]";

    public static bool operator ==(RecursiveDistanceDistribution left, RecursiveDistanceDistribution right) => left.Equals(right);
    public static bool operator !=(RecursiveDistanceDistribution left, RecursiveDistanceDistribution right) => !(left == right);
}
