using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A cummulative <see cref="IPDF{T}"/> </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="ICDF{T}"/></typeparam>
    public interface ICDF<T> : IPDF<T> where T : IComparable<T>, IEquatable<T> {
        /// <summary> Whether the <see cref="ICDF{T}"/> has a single solution </summary>
        bool IPDF.IsDelta => Minimum.Equals(Maximum);
        /// <summary> The minimum <see cref="T"/> in the domain of the <see cref="ICDF{T}"/> </summary>
        T Minimum { get; }
        /// <summary> The maximum <see cref="T"/> in the domain of the <see cref="ICDF{T}"/> </summary>
        T Maximum { get; }

        /// <summary> Check whether the <see cref="ICDF{T}"/> contains the <paramref name="sample"/> </summary>
        /// <param name="sample">The sample to check</param>
        /// <returns>Whether the <see cref="ICDF{T}"/> contains the <paramref name="sample"/></returns>
        bool IProbabilityDistribution<T>.Contains(T sample) => !(Before(sample) || After(sample));

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
        double CumulativeProbabilityDensity(T sample);
    }

    /// <summary> A recursive <see cref="ICDF{T}"/> </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="IRecursiveCDF{T}"/> </typeparam>
    public interface IRecursiveCDF<T> : ICDF<T> where T : IComparable<T>, IEquatable<T> {
        /// <summary> The left <see cref="IRecursiveCDF{T}"/> </summary>
        ICDF<T> Left { get; }
        /// <summary> The right <see cref="IRecursiveCDF{T}"/> </summary>
        ICDF<T> Right { get; }

        /// <summary> Whether the <see cref="IRecursiveCDF{T}"/> contains a delta distribution </summary>
        bool IPDF.ContainsDelta => Left.ContainsDelta || Right.ContainsDelta;
        /// <summary> The minimum <see cref="T"/> in the domain of the <see cref="IRecursiveCDF{T}"/> </summary>
        T ICDF<T>.Minimum => Utils.Min(Left.Minimum, Right.Minimum);
        /// <summary> The maximum <see cref="T"/> in the domain of the <see cref="IRecursiveCDF{T}"/> </summary>
        T ICDF<T>.Maximum => Utils.Min(Left.Maximum, Right.Maximum);
        /// <summary> The size of the domain is 2; left and right </summary>
        double IProbabilityDistribution.DomainSize => 2;

        /// <summary> Sample the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        T IProbabilityDistribution<T>.Sample(Random random) {
            T left = Left.Sample(random);
            if (left.CompareTo(Right.Minimum) <= 0) {
                return left;
            } else {
                return Utils.Min(left, Right.Sample(random));
            }
        }

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="sample">The sample to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        double IPDF<T>.ProbabilityDensity(T sample) {
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

        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="IRecursiveCDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double ICDF<T>.CumulativeProbabilityDensity(T sample) {
            if (After(sample)) {
                return 0;
            } else {
                double l = Left.CumulativeProbabilityDensity(sample);
                double r = Right.CumulativeProbabilityDensity(sample);
                return l + r - l * r;
            }
        }
    }
}
