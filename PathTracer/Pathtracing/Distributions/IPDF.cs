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
        /// <param name="sample">The sample to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="IPDF{T}"/></returns>
        bool Contains(T sample);

        /// <summary> Get the probability (density) of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
        /// <param name="sample">The sample to get the probability for</param>
        /// <returns>The probability (density) of the <paramref name="sample"/></returns>
        double Probability(T sample);
    }

    /// <summary> A cummulative distribution/density function </summary>
    /// <typeparam name="T">The variable of the distribution function</typeparam>
    public interface ICDF<T> : IPDF<T> {
        /// <summary> Get the cummulative probability of a <paramref name="sample"/> in the <see cref="ICDF{T}"/> </summary>
        /// <param name="sample">The sample to get the cummulative probability for</param>
        /// <returns>The cummulative probability of the <paramref name="sample"/></returns>
        double CumulativeDistribution(T sample);
    }

    /// <summary> A joint probability function </summary>
    /// <typeparam name="T1">The first variable of the joint probability function</typeparam>
    /// <typeparam name="T2">The second variable of the joint probability function</typeparam>
    public interface IJPF<T1, T2> : IPDF<(T1, T2)> { }
}
