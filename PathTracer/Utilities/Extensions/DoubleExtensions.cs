using System;

namespace PathTracer.Utilities.Extensions {
    /// <summary> Extension methods for <see cref="double"/> </summary>
    public static class DoubleExtensions {
        /// <summary> The minimum <see cref="long"/> that can represent a <see cref="double"/>-index </summary>
        public static readonly long MinIndex = ToIndex(double.NegativeInfinity);
        /// <summary> The maximum <see cref="long"/> that can represent a <see cref="double"/>-index </summary>
        public static readonly long MaxIndex = ToIndex(double.PositiveInfinity);

        /// <summary> Convert a <paramref name="value"/> to a <see cref="double"/>-index </summary>
        /// <param name="value">the <see cref="double"/> to convert to a <see cref="double"/>-index </param>
        /// <returns>An <see cref="long"/> representing the <see cref="double"/>-index of the <paramref name="value"/></returns>
        /// <exception cref="ArgumentException"><see cref="double.Nan"/> does not have a valid <see cref="double"/>-index representation</exception>
        public static long ToIndex(this double value) {
            if (double.IsNaN(value)) {
                throw new ArgumentException($"{value} does not have a sensical index representation");
            } else {
                long bits = BitConverter.DoubleToInt64Bits(value);
                return bits < 0 ? (bits ^ long.MaxValue) + 1 : bits;
            }
        }

        /// <summary> Convert a <see cref="double"/>-<paramref name="index"/> to a <see cref="double"/> </summary>
        /// <param name="index">The <see cref="long"/> representing the <see cref="double"/>-index</param>
        /// <returns>The <see cref="double"/> according to the <see cref="double"/>-<paramref name="index"/></returns>
        /// <exception cref="ArgumentException">Not all <see cref="long"/> values are valid <see cref="double"/>-indices</exception>
        public static double FromIndex(this long index) {
            if (index < MinIndex || index > MaxIndex) {
                throw new ArgumentException($"{index} does not have a sensical floating point representation");
            } else {
                long bits = index < 0 ? (index - 1) ^ long.MaxValue : index;
                return BitConverter.Int64BitsToDouble(bits);
            }
        }

        /// <summary> Get the next <see cref="double"/> above the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="double"/></param>
        /// <returns>The next <see cref="double"/> above the <paramref name="value"/></returns>
        /// <exception cref="ArgumentException"><see cref="double.PositiveInfinity"/> and <see cref="double.NaN"/> do not have a next</exception>
        public static double Next(this double value) {
            if (double.IsPositiveInfinity(value)) {
                throw new ArgumentException($"{value} does not have a valid next");
            } else {
                return (value.ToIndex() + 1).FromIndex();
            }
        }

        /// <summary> Increment the <paramref name="value"/> by an <paramref name="amount"/> </summary>
        /// <param name="value">The <see cref="double"/> value to increment</param>
        /// <param name="amount">The amount of steps to increment the <paramref name="value"/></param>
        /// <returns>The <paramref name="value"/> incremented by the <paramref name="amount"/></returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> incremented by <paramref name="amount"/> overflows into <see cref="double.NaN"/></exception>
        public static double Increment(this double value, ulong amount) {
            long index = value.ToIndex() + (long)amount;
            if (index > MaxIndex) {
                throw new ArgumentException($"{value} incremented by {amount} causes overflow into NaN");
            } else {
                return index.FromIndex();
            }
        }

        /// <summary> Get the next <see cref="double"/> above the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="double"/></param>
        /// <returns>The next <see cref="double"/> above the <paramref name="value"/></returns>
        /// <exception cref="ArgumentException"><see cref="double.NegativeInfinity"/> and <see cref="double.NaN"/> do not have a previous</exception>
        public static double Previous(this double value) {
            if (double.IsNegativeInfinity(value)) {
                throw new ArgumentException($"{value} does not have a valid previous");
            } else {
                return (value.ToIndex() - 1).FromIndex();
            }
        }

        /// <summary> Decrement the <paramref name="value"/> by an <paramref name="amount"/> </summary>
        /// <param name="value">The <see cref="double"/> value to decrement</param>
        /// <param name="amount">The amount of steps to decrement the <paramref name="value"/></param>
        /// <returns>The <paramref name="value"/> decremented by the <paramref name="amount"/></returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> decremented by <paramref name="amount"/> underflows into <see cref="double.NaN"/></exception>
        public static double Decrement(this double value, ulong amount) {
            long index = value.ToIndex() - (long)amount;
            if (index < MinIndex) {
                throw new ArgumentException($"{value} decremented by {amount} causes underflow into NaN");
            } else {
                return index.FromIndex();
            }
        }
    }
}
