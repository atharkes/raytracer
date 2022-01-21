using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathTracer.Utilities.Extensions;
using System;

namespace UnitTests.Utilities.Extensions {
    [TestClass]
    public class SingleExtensionsTests {
        #region ToIndex
        [TestMethod]
        public void ToIndexNaNGivesException() => Assert.ThrowsException<ArgumentException>(() => float.NaN.ToIndex());

        [TestMethod]
        public void ToIndexNegativeInfinity() => float.NegativeInfinity.ToIndex();

        [TestMethod]
        public void ToIndexMinValue() => float.MinValue.ToIndex();

        [TestMethod]
        public void ToIndexNegativeEpsilonIsNegativeOne() => Assert.AreEqual(-1, (-float.Epsilon).ToIndex());

        [TestMethod]
        public void ToIndexNegativeZeroIsZero() => Assert.AreEqual(0, (-0f).ToIndex());

        [TestMethod]
        public void ToIndexZeroIsZero() => Assert.AreEqual(0, 0f.ToIndex());

        [TestMethod]
        public void ToIndexEpsilonIsOne() => Assert.AreEqual(1, float.Epsilon.ToIndex());

        [TestMethod]
        public void ToIndexMaxValue() => float.MaxValue.ToIndex();

        [TestMethod]
        public void ToIndexPositiveInifinty() => float.PositiveInfinity.ToIndex();

        [TestMethod]
        public void ToIndexIsReversibleByFromIndex() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.AreEqual(value, value.ToIndex().FromIndex());
            }
        }
        #endregion

        #region FromIndex
        [TestMethod]
        public void FromIndexZeroIsZero() => Assert.AreEqual(0f, 0.FromIndex());

        [TestMethod]
        public void FromIndexNegativeEpsilonIsNegativeOne() => Assert.AreEqual(-float.Epsilon, (-1).FromIndex());

        [TestMethod]
        public void FromIndexEpsilonIsOne() => Assert.AreEqual(float.Epsilon, 1.FromIndex());

        [TestMethod]
        public void FromIndexIsReversibleByToIndex() {
            for (int i = 0; i < 10_000; i++) {
                int index = Random.Shared.FiniteSingleIndex();
                Assert.AreEqual(index, index.FromIndex().ToIndex());
            }
        }
        #endregion

        #region Next
        [TestMethod]
        public void NextGivesBiggerValue() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.IsTrue(value < value.Next());
            }
        }

        [TestMethod]
        public void NextGivesIndexPlusOne() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.AreEqual(value.ToIndex() + 1, value.Next().ToIndex());
            }
        }

        [TestMethod]
        public void NextOnZeroGivesEpsilon() => Assert.AreEqual(float.Epsilon, 0f.Next());

        [TestMethod]
        public void NextOnNegativeEpsilonGivesZero() => Assert.AreEqual(0f, (-float.Epsilon).Next());

        [TestMethod]
        public void NextOnPositiveInfinityGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => float.PositiveInfinity.Next());

        [TestMethod]
        public void NextOnNaNGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => float.NaN.Next());

        [TestMethod]
        public void NextIsReversableByPrevious() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.AreEqual(value, value.Next().Previous());
            }
        }
        #endregion

        #region Previous
        [TestMethod]
        public void PreviousGivesSmallerValue() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.IsTrue(value > value.Previous());
            }
        }

        [TestMethod]
        public void PreviousGivesIndexMinusOne() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.AreEqual(value.ToIndex() - 1, value.Previous().ToIndex());
            }
        }

        [TestMethod]
        public void PreviousOnEpsilonGivesZero() => Assert.AreEqual(0f, float.Epsilon.Previous());

        [TestMethod]
        public void PreviousOnZeroGivesNegativeEpsilon() => Assert.AreEqual(-float.Epsilon, 0f.Previous());

        [TestMethod]
        public void PreviousOnNegativeInfinityGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => float.NegativeInfinity.Previous());

        [TestMethod]
        public void PreviousOnNaNGivesArgumentException() => Assert.ThrowsException<ArgumentException>(() => float.NaN.Previous());

        [TestMethod]
        public void PreviousIsReversableByNext() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                Assert.AreEqual(value, value.Previous().Next());
            }
        }
        #endregion

        #region Increment
        /// <summary> Get the maximum amount that the <paramref name="value"/> can be incremented without overflowing </summary>
        /// <param name="value">The <see cref="float"/> to get the value for</param>
        /// <returns>the maximum amount that the <paramref name="value"/> can be incremented without overflowing</returns>
        static uint MaxIncrementable(float value) => (uint)SingleExtensions.MaxIndex - (uint)value.ToIndex() + 1u;

        [TestMethod]
        public void IncrementGivesBiggerValue() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxIncrementable(value));
                Assert.IsTrue(value < value.Increment(amount));
            }
        }

        [TestMethod]
        public void IncrementGivesIndexPlusAmount() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxIncrementable(value));
                Assert.AreEqual(value.ToIndex() + amount, value.Increment(amount).ToIndex());
            }
        }

        [TestMethod]
        public void IncrementIsReversableByDecrement() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxIncrementable(value));
                Assert.AreEqual(value, value.Increment(amount).Decrement(amount));
            }
        }
        #endregion

        #region Decrement
        /// <summary> Get the maximum amount that the <paramref name="value"/> can be decremented without underflowing </summary>
        /// <param name="value">The <see cref="float"/> to get the value for</param>
        /// <returns>the maximum amount that the <paramref name="value"/> can be decremented without underflowing</returns>
        static uint MaxDecrementable(float value) => 1u - (uint)SingleExtensions.MinIndex + (uint)value.ToIndex();

        [TestMethod]
        public void DecrementGivesSmallerValue() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxDecrementable(value));
                Assert.IsTrue(value > value.Decrement(amount));
            }
        }

        [TestMethod]
        public void DecrementGivesIndexMinusAmount() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxDecrementable(value));
                Assert.AreEqual(value.ToIndex() - amount, value.Decrement(amount).ToIndex());
            }
        }

        [TestMethod]
        public void DecrementIsReversableByIncrement() {
            for (int i = 0; i < 10_000; i++) {
                float value = Random.Shared.FiniteSingle();
                uint amount = (uint)Random.Shared.NextInt64(MaxDecrementable(value));
                Assert.AreEqual(value, value.Decrement(amount).Increment(amount));
            }
        }
        #endregion
    }
}
