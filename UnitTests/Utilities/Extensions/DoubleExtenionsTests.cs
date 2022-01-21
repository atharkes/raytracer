using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Utilities.Extensions;
using System;
using System.Numerics;

namespace UnitTests.Utilities.Extensions {
    [TestClass]
    public class DoubleExtenionsTests {
        #region ToIndex
        [TestMethod]
        public void ToIndexNaNGivesException() => Assert.ThrowsException<ArgumentException>(() => double.NaN.ToIndex());

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
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
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
            for (int i = 0; i < 10_000; i++) {
                long index = Random.Shared.FiniteDoubleIndex();
                Assert.AreEqual(index, index.FromIndex().ToIndex());
            }
        }
        #endregion

        #region Next
        [TestMethod]
        public void NextGivesBiggerValue() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.IsTrue(value < value.Next());
            }
        }

        [TestMethod]
        public void NextGivesIndexPlusOne() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.AreEqual(value.ToIndex() + 1L, value.Next().ToIndex());
            }
        }

        [TestMethod]
        public void NextOnZeroGivesEpsilon() => Assert.AreEqual(double.Epsilon, 0d.Next());

        [TestMethod]
        public void NextOnNegativeEpsilonGivesZero() => Assert.AreEqual(0d, (-double.Epsilon).Next());

        [TestMethod]
        public void NextOnPositiveInfinityGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => double.PositiveInfinity.Next());

        [TestMethod]
        public void NextOnNaNGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => double.NaN.Next());

        [TestMethod]
        public void NextIsReversableByPrevious() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.AreEqual(value, value.Next().Previous());
            }
        }
        #endregion

        #region Previous
        [TestMethod]
        public void PreviousGivesSmallerValue() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.IsTrue(value > value.Previous());
            }
        }

        [TestMethod]
        public void PreviousGivesIndexMinusOne() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.AreEqual(value.ToIndex() - 1, value.Previous().ToIndex());
            }
        }

        [TestMethod]
        public void PreviousOnEpsilonGivesZero() => Assert.AreEqual(0d, double.Epsilon.Previous());

        [TestMethod]
        public void PreviousOnZeroGivesNegativeEpsilon() => Assert.AreEqual(-double.Epsilon, 0d.Previous());

        [TestMethod]
        public void PreviousOnNegativeInfinityGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => double.NegativeInfinity.Previous());

        [TestMethod]
        public void PreviousOnNaNGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => double.NaN.Previous());

        [TestMethod]
        public void PreviousIsReversableByNext() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                Assert.AreEqual(value, value.Previous().Next());
            }
        }
        #endregion

        #region Increment
        /// <summary> Get the maximum amount that the <paramref name="value"/> can be incremented without overflowing </summary>
        /// <param name="value">The <see cref="double"/> to get the value for</param>
        /// <returns>the maximum amount that the <paramref name="value"/> can be incremented without overflowing</returns>
        static ulong MaxIncrementable(double value) => Math.Min(long.MaxValue, (ulong)DoubleExtensions.MaxIndex - (ulong)value.ToIndex() + 1ul);

        [TestMethod]
        public void IncrementGivesBiggerValue() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                ulong amount = (ulong)Random.Shared.NextInt64((long)MaxIncrementable(value));
                Assert.IsTrue(value < value.Increment(amount));
            }
        }

        [TestMethod]
        public void IncrementGivesIndexPlusAmount() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                long amount = Random.Shared.NextInt64((long)MaxIncrementable(value));
                Assert.AreEqual(value.ToIndex() + amount, value.Increment((ulong)amount).ToIndex());
            }
        }

        [TestMethod]
        public void IncrementFromNegativeInfinityToPositiveInfinity() {
            double value = DoubleExtensions.MinIndex.FromIndex();
            ulong amount = (ulong)((BigInteger)DoubleExtensions.MaxIndex - DoubleExtensions.MinIndex);
            Assert.AreEqual(DoubleExtensions.MaxIndex.FromIndex(), value.Increment(amount));
        }

        [TestMethod]
        public void IncrementIsReversableByDecrement() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                ulong amount = (ulong)Random.Shared.NextInt64((long)MaxIncrementable(value));
                Assert.AreEqual(value, value.Increment(amount).Decrement(amount));
            }
        }
        #endregion

        #region Decrement
        /// <summary> Get the maximum amount that the <paramref name="value"/> can be decremented without underflowing </summary>
        /// <param name="value">The <see cref="double"/> to get the value for</param>
        /// <returns>the maximum amount that the <paramref name="value"/> can be decremented without underflowing</returns>
        static ulong MaxDecrementable(double value) => Math.Min(long.MaxValue, 1ul - (ulong)DoubleExtensions.MinIndex + (ulong)value.ToIndex());

        [TestMethod]
        public void DecrementGivesSmallerValue() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                ulong amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
                Assert.IsTrue(value > value.Decrement(amount));
            }
        }

        [TestMethod]
        public void DecrementGivesIndexMinusAmount() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                ulong amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
                Assert.AreEqual(value.ToIndex() - (long)amount, value.Decrement(amount).ToIndex());
            }
        }

        [TestMethod]
        public void DecrementIsReversableByIncrement() {
            for (int i = 0; i < 10_000; i++) {
                double value = Random.Shared.FiniteDouble();
                ulong amount = (ulong)Random.Shared.NextInt64((long)MaxDecrementable(value));
                Assert.AreEqual(value, value.Decrement(amount).Increment(amount));
            }
        }
        #endregion
    }
}
