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
}
