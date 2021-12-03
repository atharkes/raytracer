using System;

namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A probability distribution </summary>
    public interface IProbabilityDistribution {
        /// <summary> The domain size of the <see cref="IProbabilityDistribution"/> </summary>
        double DomainSize { get; }
        /// <summary> Whether the <see cref="IProbabilityDistribution"/> has a single solution </summary>
        bool SingleSolution { get; }
        /// <summary> Whether the <see cref="IProbabilityDistribution"/> is discreet </summary>
        bool Discreet { get; }
        /// <summary> Whether the <see cref="IProbabilityDistribution"/> is continuous </summary>
        bool Continuous { get; }
    }

    /// <summary> A typed <see cref="IProbabilityDistribution"/> </summary>
    /// <typeparam name="T">The type of samples in the <see cref="IProbabilityDistribution{T}"/></typeparam>
    public interface IProbabilityDistribution<T> : IProbabilityDistribution, IEquatable<IProbabilityDistribution<T>> {
        /// <summary> Sample the <see cref="IProbabilityDistribution{T}"/> </summary>
        /// <param name="random">The <see cref="Random"/> to use for sampling</param>
        /// <returns>A <paramref name="random"/> <see cref="T"/></returns>
        T Sample(Random random);

        /// <summary> Check whether the <see cref="IProbabilityDistribution{T}"/> contains a <paramref name="sample"/> in its domain </summary>
        /// <param name="sample">The <see cref="T"/> to check</param>
        /// <returns>Whether the <paramref name="sample"/> is in the domain of the <see cref="IProbabilityDistribution{T}"/></returns>
        bool Contains(T sample);

        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IProbabilityDistribution{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability for</param>
        /// <returns>The probability of the <paramref name="sample"/></returns>
        double Probability(T sample);

        /// <summary> Get the probability of a <paramref name="sample"/> relative to a uniform distribution </summary>
        /// <param name="sample">The <see cref="T"/> to get the relative probability for</param>
        /// <returns>The relative probability of the <paramref name="sample"/></returns>
        double RelativeProbability(T sample) => Probability(sample) * DomainSize;

        /// <summary> Get the probability of a <paramref name="sample"/> inversely relative to a uniform distribution </summary>
        /// <param name="sample">The <see cref="T"/> to get the inverse relative probability for</param>
        /// <returns>The inverse relative probability of the <paramref name="sample"/></returns>
        double InverseRelativeProbability(T sample) => 1 / RelativeProbability(sample);
    }
}
