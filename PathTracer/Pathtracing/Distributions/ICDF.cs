using PathTracer.Geometry.Positions;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A cummulative <see cref="IPDF{T}"/> </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="ICDF{T}"/></typeparam>
    public interface ICDF<T> : IPDF<T> where T : IComparable<T>, IEquatable<T> {
        /// <summary> Whether the <see cref="ICDF{T}"/> has a single solution </summary>
        bool IPDF.SingleSolution => Minimum.Equals(Maximum);
        /// <summary> The minimum <see cref="T"/> in the domain of the <see cref="ICDF{T}"/> </summary>
        T Minimum { get; }
        /// <summary> The maximum <see cref="T"/> in the domain of the <see cref="ICDF{T}"/> </summary>
        T Maximum { get; }

        /// <summary> Check whether the <see cref="ICDF{T}"/> contains the <paramref name="sample"/> </summary>
        /// <param name="sample">The sample to check</param>
        /// <returns>Whether the <see cref="ICDF{T}"/> contains the <paramref name="sample"/></returns>
        bool IPDF<T>.Contains(T sample) => !(Before(sample) || After(sample));

        /// <summary> Check whether the <see cref="ICDF{T}"/> is entirely before the <paramref name="sample"/> </summary>
        /// <param name="sample">The sample to check</param>
        /// <returns>Whether the <see cref="ICDF{T}"/> is entirely before the <paramref name="sample"/></returns>
        bool Before(T sample) => Maximum.CompareTo(sample) < 0;

        /// <summary> Check whether the <see cref="ICDF{T}"/> is entirely after the <paramref name="sample"/> </summary>
        /// <param name="sample">The sample to check</param>
        /// <returns>Whether the <see cref="ICDF{T}"/> is entirely after the <paramref name="sample"/></returns>
        bool After(T sample) => Minimum.CompareTo(sample) > 0;

        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="ICDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double CumulativeProbability(T sample);
    }

    /// <summary> A recursive <see cref="ICDF{T}"/> </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="IRecursiveCDF{T}"/> </typeparam>
    public interface IRecursiveCDF<T> : ICDF<T> where T : IComparable<T>, IEquatable<T> {
        /// <summary> The left <see cref="IRecursiveCDF{T}"/> </summary>
        ICDF<T> Left { get; }
        /// <summary> The right <see cref="IRecursiveCDF{T}"/> </summary>
        ICDF<T> Right { get; }

        /// <summary> The minimum <see cref="T"/> in the domain of the <see cref="IRecursiveCDF{T}"/> </summary>
        T ICDF<T>.Minimum => Utils.Min(Left.Minimum, Right.Minimum);
        /// <summary> The maximum <see cref="T"/> in the domain of the <see cref="IRecursiveCDF{T}"/> </summary>
        T ICDF<T>.Maximum => Utils.Min(Left.Maximum, Right.Maximum);

        /// <summary> Sample the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        T IPDF<T>.Sample(Random random) => Utils.Min(Left.Sample(random), Right.Sample(random));

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="sample">The sample to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        double IPDF<T>.Probability(T sample) {
            if (!Contains(sample)) {
                return 0;
            } else {
                double pLeft = Left.Probability(sample);
                double pRight = Right.Probability(sample);
                if (pLeft <= 0 && pRight <= 0) {
                    return 0;
                } else if (pLeft <= 0) {
                    return (1 - Left.CumulativeProbability(sample)) * pRight;
                } else if (pRight <= 0) {
                    return (1 - Right.CumulativeProbability(sample)) * pLeft;
                } else {
                    return (1 - Left.CumulativeProbability(sample)) * pRight + (1 - Right.CumulativeProbability(sample)) * pLeft;
                }
            }
        }

        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double ICDF<T>.CumulativeProbability(T sample) {
            if (After(sample)) {
                return 0;
            } else {
                double l = Left.CumulativeProbability(sample);
                double r = Right.CumulativeProbability(sample);
                return l + r - l * r;
            }
        }
    }
}
