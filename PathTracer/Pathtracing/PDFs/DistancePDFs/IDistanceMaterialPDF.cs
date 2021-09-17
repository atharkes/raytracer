using PathTracer.Pathtracing.PDFs.MaterialPDFs;
using PathTracer.Pathtracing.SceneDescription;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistanceMaterialPDF : IDistancePDF, IPDF<double, IMaterial> {
        IPDF<IMaterial> IPDF<double, IMaterial>.ExtractPDF(double sample) => ExtractPDF(sample);
        new IMaterialPDF ExtractPDF(double sample);

        public static IDistanceMaterialPDF operator +(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            return new SumDistanceMaterialPDF(left, right);
        }

        public static IDistanceMaterialPDF operator *(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            return new ProductDistanceMaterialPDF(left, right);
        }
    }
}
