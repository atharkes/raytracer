using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Pathtracing.Distributions;

namespace UnitTests.Pathtracing.PDFs {
    [TestClass]
    public class DistancePDFTest {
        [TestMethod]
        public void Test() {
            ExponentialDistancePDF A = new(3, 14, 0.012);
            ExponentialDistancePDF B = new(6, 10, 0.001);
            ExponentialDistancePDF C = new(19, 25, 0.023);
            ExponentialDistancePDF D = new(21, 27, 0.042);
            double sample = 23;

            double cdfA = 1 - A.CumulativeDistribution(sample);
            double cdfB = 1 - B.CumulativeDistribution(sample);
            double cdfC = 1 - C.CumulativeDistribution(sample);
            double probC = C.Probability(sample);
            double cdfD = 1 - D.CumulativeDistribution(sample);
            double probD = D.Probability(sample);

            double prob1 = cdfA * cdfB * (cdfC * probD + cdfD * probC);

            ExponentialDistancePDF CbeforeD = new(C.DomainStart, D.DomainStart, C.Distribution.Rate);
            ExponentialDistancePDF CandD = new(D.DomainStart, C.DomainEnd, C.Distribution.Rate + D.Distribution.Rate);
            double cdfC2 = 1 - CbeforeD.CumulativeDistribution(sample);
            double probD2 = CandD.Probability(sample);

            double prob2 = cdfA * cdfB * cdfC2 * probD2;

            Assert.AreEqual(prob1, prob2, (prob1 + prob2) / 10000);
        }
    }
}
