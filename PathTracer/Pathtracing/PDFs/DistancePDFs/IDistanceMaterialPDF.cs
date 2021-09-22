using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistanceMaterialPDF : IDistancePDF, IPDF<IDistanceMaterial> {
        IDistanceMaterial IPDF<IDistanceMaterial>.Sample(Random random) => SampleWithMaterial(random);
        public abstract IDistanceMaterial SampleWithMaterial(Random random);

        public static IDistanceMaterialPDF operator +(IDistanceMaterialPDF left, IDistanceMaterialPDF right) {
            return new SumDistanceMaterialPDF(left, right);
        }
    }

    public interface IDistanceMaterial {
        double Distance { get; }
        IMaterial Material { get; }
    }
}
