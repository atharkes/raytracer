using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.PDFs.MaterialPDFs {
    public interface IMaterialPDF : IPDF<IMaterial> {
        public static IMaterialPDF operator +(IMaterialPDF left, IMaterialPDF right) {
            throw new NotImplementedException();
        }

        public static IMaterialPDF operator *(IMaterialPDF left, IMaterialPDF right) {
            throw new NotImplementedException();
        }
    }
}
