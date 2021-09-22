using System;

namespace PathTracer.Pathtracing.PDFs {
    public interface IPDF {
        bool SingleSolution { get; }
        double DomainSize { get; }
    }

    public interface IPDF<T> : IPDF {
        T Sample(Random random);
        bool Contains(T sample);
        double Probability(T sample);
        double CumulativeDistribution(T sample);
    }

    public interface IPDF<T1, T2> : IPDF<T1>, IPDF<(T1, T2)> {
        T1 IPDF<T1>.Sample(Random random) => SampleSingle(random);
        T1 SampleSingle(Random random);

        (T1, T2) IPDF<(T1, T2)>.Sample(Random random) => SampleDouble(random);
        (T1, T2) SampleDouble(Random random);
    }
}
