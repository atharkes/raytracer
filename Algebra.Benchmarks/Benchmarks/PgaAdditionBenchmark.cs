using Algebra.Numbers;
using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace Algebra.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class PgaAdditionBenchmark {
    public PGA3D Left { get; set; } = null!;
    public PGA3D Right { get; set; } = null!;

    [GlobalSetup]
    public void Setup() {
        Left = Random.Shared.NextNumberPGA3d();
        Right = Random.Shared.NextNumberPGA3d();

        LeftVectorized = Random.Shared.NextVectorizedPGA3d();
        RightVectorized = Random.Shared.NextVectorizedPGA3d();
    }

    [Benchmark(Baseline = true)]
    public PGA3D Default() => Left + Right;

    [Benchmark]
    public PGA3D VectorizedCalculation() {
        var result = new Number[PGA3D.BasisLength];

        for (var i = 0; i < PGA3D.BasisLength; i += Vector<Number>.Count) {
            var v1 = new Vector<Number>(Left._mVec, i);
            var v2 = new Vector<Number>(Right._mVec, i);
            (v1 + v2).CopyTo(result, i);
        }

        return new PGA3D(result);
    }

    public VectorizedPga3d LeftVectorized { get; set; } = null!;
    public VectorizedPga3d RightVectorized { get; set; } = null!;

    [Benchmark]
    public VectorizedPga3d Vectorized() => LeftVectorized + RightVectorized;
}
