namespace PathTracer.Pathtracing.Distributions.Probabilities {
    /// <summary> A probability mass function </summary>
    public interface IPMF : IProbabilityDistribution {
        /// <summary> The domain size of the <see cref="IPMF"/> </summary>
        new int DomainSize { get; }
        /// <summary> The domain size of the <see cref="IPMF"/> </summary>
        double IProbabilityDistribution.DomainSize => DomainSize;
        /// <summary> Whether the <see cref="IPMF"/> has a single solution </summary>
        bool IProbabilityDistribution.SingleSolution => DomainSize.Equals(1);
        /// <summary> The <see cref="IPMF"/> is discreet </summary>
        bool IProbabilityDistribution.Discreet => true;
        /// <summary> The <see cref="IPMF"/> is not continuous </summary>
        bool IProbabilityDistribution.Continuous => false;
    }

    /// <summary> A typed <see cref="IPMF"/> </summary>
    /// <typeparam name="T">The sample type of the <see cref="IPMF{T}"/></typeparam>
    public interface IPMF<T> : IPMF, IProbabilityDistribution<T> {
        /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IPMF{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability for</param>
        /// <returns>The probability mass of the <paramref name="sample"/></returns>
        double IProbabilityDistribution<T>.Probability(T sample) => ProbabilityMass(sample);

        /// <summary> Get the probability mass of a <paramref name="sample"/> in the <see cref="IPMF{T}"/> </summary>
        /// <param name="sample">The <see cref="T"/> to get the probability mass for</param>
        /// <returns>Te probability mass of the <paramref name="sample"/></returns>
        double ProbabilityMass(T sample);
    }
}
