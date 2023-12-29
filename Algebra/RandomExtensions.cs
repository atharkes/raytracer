using Algebra.Numbers;

namespace Algebra;
public static class RandomExtensions {
    public static Number NextNumber(this Random random)
#if Double
        => (Number)random.NextDouble();
#else
        => (Number)random.NextSingle();
#endif

    public static PGA3D NextNumberPGA3d(this Random random) {
        var result = new PGA3D();
        for (var i = 0; i < PGA3D.BasisLength; i++) {
            result[i] = random.NextNumber();
        }
        return result;
    }

    public static VectorizedPga3d NextVectorizedPGA3d(this Random random) {
        var result = new VectorizedPga3d();
        for (var i = 0; i < VectorizedPga3d.BasisLength; i++) {
            result[i] = random.NextSingle();
        }
        return result;
    }
}
