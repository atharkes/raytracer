using System;

namespace PathTracer.Pathtracing.Distributions {
    /// <summary> A generic probability distribution/density function </summary>
    public interface IPDF {
        bool SingleSolution { get; }
        double DomainSize { get; }
    }

    /// <summary> A probability distribution/density function </summary>
    /// <typeparam name="T">The variable of the distribution function</typeparam>
    public interface IPDF<T> : IPDF {
        T Sample(Random random);
        bool Contains(T sample);
        double Probability(T sample);
    }

    /// <summary> A cummulative distribution/density function </summary>
    /// <typeparam name="T">The variable of the distribution function</typeparam>
    public interface ICDF<T> : IPDF<T> {
        double CumulativeDistribution(T sample);
    }

    /// <summary> A joint probability function </summary>
    /// <typeparam name="T1">The first variable of the joint probability function</typeparam>
    /// <typeparam name="T2">The second variable of the joint probability function</typeparam>
    public interface IJPF<T1, T2> : IPDF<(T1, T2)> { }
}
