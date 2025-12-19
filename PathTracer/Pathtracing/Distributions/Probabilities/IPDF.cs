namespace PathTracer.Pathtracing.Distributions.Probabilities;

/// <summary> A probability density function </summary>
public interface IPDF : IProbabilityDistribution {
    /// <summary> Whether the <see cref="IPDF"/> is a delta distribution </summary>
    bool IsDelta => DomainSize.Equals(0);
    /// <summary> Whether the <see cref="IPDF"/> contains a delta distribution </summary>
    bool ContainsDelta { get; }
    /// <summary> Whether the <see cref="IPDF"/> has a single solution </summary>
    bool IProbabilityDistribution.SingleSolution => IsDelta;
    /// <summary> The <see cref="IPMF"/> is discreet </summary>
    bool IProbabilityDistribution.Discreet => false;
    /// <summary> The <see cref="IPMF"/> is not continuous </summary>
    bool IProbabilityDistribution.Continuous => true;
}

/// <summary> A typed <see cref="IPDF"/> </summary>
/// <typeparam name="T">The type of the samples of the <see cref="IPDF{T}"/></typeparam>
public interface IPDF<T> : IPDF, IProbabilityDistribution<T> {
    /// <summary> Get the probability of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
    /// <param name="sample">The <see cref="T"/> to get the probability for</param>
    /// <returns>The probability density of the <paramref name="sample"/></returns>
    double IProbabilityDistribution<T>.Probability(T sample) => ProbabilityDensity(sample);

    /// <summary> Get the probability density of a <paramref name="sample"/> in the <see cref="IPDF{T}"/> </summary>
    /// <param name="sample">The <see cref="T"/> to get the probability density for</param>
    /// <returns>The probability density of the <paramref name="sample"/></returns>
    double ProbabilityDensity(T sample);
}
