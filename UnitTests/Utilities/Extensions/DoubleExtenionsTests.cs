using PathTracer.Utilities.Extensions;
using System.Numerics;

namespace UnitTests.Utilities.Extensions;

[TestClass]
public class DoubleExtenionsTests {
    #region ToIndex
    [TestMethod]
    public void ToIndexNaNGivesException() => Assert.Throws<ArgumentException>(() => double.NaN.ToIndex());

    [TestMethod]
    public void ToIndexNegativeInfinity() => double.NegativeInfinity.ToIndex();

    [TestMethod]
    public void ToIndexMinValue() => double.MinValue.ToIndex();

    [TestMethod]
    public void ToIndexNegativeEpsilonIsNegativeOne() => Assert.AreEqual(-1L, (-double.Epsilon).ToIndex());

    [TestMethod]
    public void ToIndexNegativeZeroIsZero() => Assert.AreEqual(0L, (-0d).ToIndex());

    [TestMethod]
    public void ToIndexZeroIsZero() => Assert.AreEqual(0L, 0d.ToIndex());

    [TestMethod]
    public void ToIndexEpsilonIsOne() => Assert.AreEqual(1L, double.Epsilon.ToIndex());

    [TestMethod]
    public void ToIndexMaxValue() => double.MaxValue.ToIndex();

    [TestMethod]
    public void ToIndexPositiveInifinty() => double.PositiveInfinity.ToIndex();

    [TestMethod]
    public void ToIndexIsReversibleByFromIndex() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.AreEqual(value, value.ToIndex().FromIndex());
        }
    }
    #endregion

    #region FromIndex
    [TestMethod]
    public void FromIndexZeroIsZero() => Assert.AreEqual(0d, 0L.FromIndex());

    [TestMethod]
    public void FromIndexNegativeEpsilonIsNegativeOne() => Assert.AreEqual(-double.Epsilon, (-1L).FromIndex());

    [TestMethod]
    public void FromIndexEpsilonIsOne() => Assert.AreEqual(double.Epsilon, 1L.FromIndex());

    [TestMethod]
    public void FromIndexIsReversibleByToIndex() {
        for (var i = 0; i < 10_000; i++) {
            var index = Random.Shared.FiniteDoubleIndex();
            Assert.AreEqual(index, index.FromIndex().ToIndex());
        }
    }
    #endregion

    #region Next
    [TestMethod]
    public void NextGivesBiggerValue() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.IsLessThan(value.Next(), value);
        }
    }

    [TestMethod]
    public void NextGivesIndexPlusOne() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.AreEqual(value.ToIndex() + 1L, value.Next().ToIndex());
        }
    }

    [TestMethod]
    public void NextOnZeroGivesEpsilon() => Assert.AreEqual(double.Epsilon, 0d.Next());

    [TestMethod]
    public void NextOnNegativeEpsilonGivesZero() => Assert.AreEqual(0d, (-double.Epsilon).Next());

    [TestMethod]
    public void NextOnPositiveInfinityGivesArgumentException() => Assert.Throws<ArgumentException>(() => double.PositiveInfinity.Next());

    [TestMethod]
    public void NextOnNaNGivesArgumentException() => Assert.Throws<ArgumentException>(() => double.NaN.Next());

    [TestMethod]
    public void NextIsReversableByPrevious() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.AreEqual(value, value.Next().Previous());
        }
    }
    #endregion

    #region Previous
    [TestMethod]
    public void PreviousGivesSmallerValue() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.IsGreaterThan(value.Previous(), value);
        }
    }

    [TestMethod]
    public void PreviousGivesIndexMinusOne() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.AreEqual(value.ToIndex() - 1, value.Previous().ToIndex());
        }
    }

    [TestMethod]
    public void PreviousOnEpsilonGivesZero() => Assert.AreEqual(0d, double.Epsilon.Previous());

    [TestMethod]
    public void PreviousOnZeroGivesNegativeEpsilon() => Assert.AreEqual(-double.Epsilon, 0d.Previous());

    [TestMethod]
    public void PreviousOnNegativeInfinityGivesArgumentException() => Assert.Throws<ArgumentException>(() => double.NegativeInfinity.Previous());

    [TestMethod]
    public void PreviousOnNaNGivesArgumentException() => Assert.Throws<ArgumentException>(() => double.NaN.Previous());

    [TestMethod]
    public void PreviousIsReversableByNext() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            Assert.AreEqual(value, value.Previous().Next());
        }
    }
    #endregion

    #region Increment
    /// <summary> Get the maximum amount that the <paramref name="value"/> can be incremented without overflowing </summary>
    /// <param name="value">The <see cref="double"/> to get the value for</param>
    /// <returns>the maximum amount that the <paramref name="value"/> can be incremented without overflowing</returns>
    private static ulong MaxIncrementable(double value) => Math.Min(long.MaxValue, (ulong)DoubleExtensions.MaxIndex - (ulong)value.ToIndex() + 1ul);

    [TestMethod]
    public void IncrementGivesBiggerValue() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = (ulong)Random.Shared.NextInt64((long)MaxIncrementable(value));
            Assert.IsLessThan(value.Increment(amount), value);
        }
    }

    [TestMethod]
    public void IncrementGivesIndexPlusAmount() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = Random.Shared.NextInt64((long)MaxIncrementable(value));
            Assert.AreEqual(value.ToIndex() + amount, value.Increment((ulong)amount).ToIndex());
        }
    }

    [TestMethod]
    public void IncrementFromNegativeInfinityToPositiveInfinity() {
        var value = DoubleExtensions.MinIndex.FromIndex();
        var amount = (ulong)((BigInteger)DoubleExtensions.MaxIndex - DoubleExtensions.MinIndex);
        Assert.AreEqual(DoubleExtensions.MaxIndex.FromIndex(), value.Increment(amount));
    }

    [TestMethod]
    public void IncrementIsReversableByDecrement() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = (ulong)Random.Shared.NextInt64((long)MaxIncrementable(value));
            Assert.AreEqual(value, value.Increment(amount).Decrement(amount));
        }
    }
    #endregion

    #region Decrement
    /// <summary> Get the maximum amount that the <paramref name="value"/> can be decremented without underflowing </summary>
    /// <param name="value">The <see cref="double"/> to get the value for</param>
    /// <returns>the maximum amount that the <paramref name="value"/> can be decremented without underflowing</returns>
    private static ulong MaxDecrementable(double value) => Math.Min(long.MaxValue, 1ul - (ulong)DoubleExtensions.MinIndex + (ulong)value.ToIndex());

    [TestMethod]
    public void DecrementGivesSmallerValue() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
            Assert.IsGreaterThan(value.Decrement(amount), value);
        }
    }

    [TestMethod]
    public void DecrementGivesIndexMinusAmount() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
            Assert.AreEqual(value.ToIndex() - (long)amount, value.Decrement(amount).ToIndex());
        }
    }

    [TestMethod]
    public void DecrementIsReversableByIncrement() {
        for (var i = 0; i < 10_000; i++) {
            var value = Random.Shared.FiniteDouble();
            var amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
            Assert.AreEqual(value, value.Decrement(amount).Increment(amount));
        }
    }
    #endregion
}
