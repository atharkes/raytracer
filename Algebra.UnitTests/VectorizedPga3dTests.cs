using Algebra.Numbers;
using FluentAssertions;

namespace Algebra.UnitTests;

public class VectorizedPga3dTests {
    [Fact]
    public void Addition_ShouldBeValid() {
        var left = Random.Shared.NextVectorizedPGA3d();
        var right = Random.Shared.NextVectorizedPGA3d();
        var expected = new PGA3D(left.ToArray()) + new PGA3D(right.ToArray());

        var result = left + right;

        new PGA3D(result.ToArray()).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Multiplication_ShouldBeValid() {
        var left = Random.Shared.NextVectorizedPGA3d();
        var right = Random.Shared.NextVectorizedPGA3d();
        var expected = new PGA3D(left.ToArray()) * new PGA3D(right.ToArray());

        var result = left * right;

        new PGA3D(result.ToArray()).Should().BeEquivalentTo(expected);
    }
}
