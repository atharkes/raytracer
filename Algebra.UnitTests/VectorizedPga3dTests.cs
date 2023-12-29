using Algebra.Numbers;
using FluentAssertions;

namespace Algebra.UnitTests;

public class VectorizedPga3dTests {
    [Fact]
    public void Addition_ShouldBeValid() {
        var left = Random.Shared.NextVectorizedPGA3d();
        var right = Random.Shared.NextVectorizedPGA3d();

        var result = left + right;

        for (var i = 0; i < VectorizedPga3d.BasisLength; i++) {
            result[i].Should().Be(left[i] + right[i]);
        }
    }
}