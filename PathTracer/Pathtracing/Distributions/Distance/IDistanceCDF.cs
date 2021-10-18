namespace PathTracer.Pathtracing.Distributions.Distance {
    public interface IDistanceCDF : ICDF<double> {
        double DomainStart { get; }
        double DomainEnd { get; }

        bool IsBefore(double sample);
        bool IsAfter(double sample);

        public static IDistanceCDF operator +(IDistanceCDF? left, IDistanceCDF right) {
            return left == null ? right : new SumDistanceCDF(left, right);
        }
    }

    public interface IRecursiveCDF<T> : ICDF<T> {
        ICDF<T> Left { get; }
        ICDF<T> Right { get; }
    }

    public interface ISumDistanceCDF<T> : IRecursiveCDF<T> {
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
