using System;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A generic probability distribution/density function </summary>
    public interface IPDF {
        /// <summary> Whether the <see cref="IPDF"/> has a single solution </summary>
        bool SingleSolution { get; }
        /// <summary> The size of the domain of the <see cref="IPDF"/> </summary>
        double DomainSize { get; }
    }

    /// <summary> A probability distribution/density function </summary>
    /// <typeparam name="T">The variable of the distribution function</typeparam>
    public interface IPDF<T> : IPDF {
        /// <summary> Sample the <see cref="IPDF{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        T Sample(Random random);

        /// <summary> Check whether the <see cref="IPDF{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="IPDF{T}"/></returns>
        bool Contains(T sample);

        /// <summary> Get the probability (density) of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
        /// <param name="sample">The sample to get the probability for</param>
        /// <returns>The probability (density) of the <paramref name="sample"/></returns>
        double Probability(T sample);
    }
}
