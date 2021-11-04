using System;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A generic probability distribution function </summary>
    public interface IPDF {
        /// <summary> Whether the <see cref="IPDF"/> has a single solution </summary>
        bool SingleSolution { get; }
        /// <summary> The size of the domain of the <see cref="IPDF"/> </summary>
        double DomainSize { get; }
    }

    /// <summary> A probability distribution function </summary>
    /// <typeparam name="T">The type of the samples of the <see cref="IPDF{T}"/></typeparam>
    public interface IPDF<T> : IPDF {
        /// <summary> Sample the <see cref="IPDF{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        T Sample(Random random);

        /// <summary> Check whether the <see cref="IPDF{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="IPDF{T}"/></returns>
        bool Contains(T sample);

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        double Probability(T sample);

        /// <summary> Get the relative probability of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the relative probability for</param>
        /// <returns>The relative probability of the <paramref name="sample"/></returns>
        double RelativeProbability(T sample) => Probability(sample) * DomainSize;
    }
}
