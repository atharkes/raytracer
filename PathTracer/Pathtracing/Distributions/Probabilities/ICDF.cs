using PathTracer.Pathtracing.Distributions.Boundaries;
using System;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A cummulative <see cref="IPDF{T}"/> </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="ICDF{T}"/></typeparam>
    public interface ICDF<T> : IPDF<T> where T : IComparable<T>, IEquatable<T> {
        /// <summary> The domain of the <see cref="ICDF{T}"/> </summary>
        IInterval<T> Domain { get; }
        /// <summary> Whether the <see cref="ICDF{T}"/> has a single solution </summary>
        bool IPDF.IsDelta => Domain.Planar;
        /// <summary> Whether the <see cref="ICDF{T}"/> contains a delta distribution </summary>
        bool IPDF.ContainsDelta => IsDelta;

        /// <summary> Check whether the <see cref="ICDF{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="ICDF{T}"/></returns>
        bool IProbabilityDistribution<T>.Contains(T sample) => Domain.Includes(sample);

        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="ICDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double CumulativeProbability(T sample);
    }
}
