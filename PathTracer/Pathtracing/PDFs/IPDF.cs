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

    public interface IRecursivePDF<T> : IPDF<T> {
        IPDF<T> Left { get; }
        IPDF<T> Right { get; }

        new double Probability(T sample) {
            if (!Contains(sample)) {
                return 0;
            } else {
                double pLeft = Left.Probability(sample);
                double pRight = Right.Probability(sample);
                // If there would be an early out for multiplication with 0 (without having to evaluate the recursive calls), the final else statement would be suffice
                if (pLeft <= 0 && pRight <= 0) {
                    return 0;
                } else if (pLeft <= 0) {
                    return (1 - Left.CumulativeDistribution(sample)) * pRight;
                } else if (pRight <= 0) {
                    return (1 - Right.CumulativeDistribution(sample)) * pLeft;
                } else {
                    return (1 - Left.CumulativeDistribution(sample)) * pRight + (1 - Right.CumulativeDistribution(sample)) * pLeft;
                }
            }
        }

        new double CumulativeDistribution(T sample) {
            double l = Left.CumulativeDistribution(sample);
            double r = Right.CumulativeDistribution(sample);
            return l + r - l * r;
        }
    }
}
