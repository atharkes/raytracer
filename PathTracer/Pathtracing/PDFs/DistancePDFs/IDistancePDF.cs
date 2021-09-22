namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistancePDF : IPDF<double> {
        double DomainStart { get; }
        double DomainEnd { get; }

        bool IsBefore(double sample);
        bool IsAfter(double sample);

        public static IDistancePDF operator +(IDistancePDF left, IDistancePDF right) {
            return new SumDistancePDF(left, right);
        }

        public static IDistancePDF operator *(IDistancePDF left, IDistancePDF right) {
            return new ProductDistancePDF(left, right);
        }
    }

    public interface IRecursiveDistancePDF<T> : IPDF<T> {
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
