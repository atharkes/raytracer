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
}
