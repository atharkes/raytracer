using PathTracer.Pathtracing.SceneDescription;
using System;

namespace PathTracer.Pathtracing.PDFs.DistancePDFs {
    public interface IDistanceMaterialPDF : IDistancePDF, IPDF<IDistanceMaterial> {
        IDistanceMaterial IPDF<IDistanceMaterial>.Sample(Random random) => SampleWithMaterial(random);
        IDistanceMaterial SampleWithMaterial(Random random);

        public static IDistanceMaterialPDF? operator +(IDistanceMaterialPDF? left, IDistanceMaterialPDF? right) {
            return left is null ? right : (right is null ? left : new SumDistanceMaterialPDF(left, right));
        }
    }

    public interface IDistanceMaterial : IComparable<IDistanceMaterial>, IEquatable<IDistanceMaterial> {
        double Distance { get; }
        IMaterial Material { get; }
    }
}
