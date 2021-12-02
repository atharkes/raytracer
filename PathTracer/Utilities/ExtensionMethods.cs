using System;
using System.Diagnostics;

namespace PathTracer.Utilities {
    /// <summary> Extension methods for external objects </summary>
    public static class ExtensionMethods {
        /// <summary> Get the next <paramref name="value"/> of a <see cref="Enum"/> </summary>
        /// <typeparam name="T">The type of <see cref="Enum"/></typeparam>
        /// <param name="value">The current value of the <see cref="Enum"/></param>
        /// <returns>The next <paramref name="value"/> of the <see cref="Enum"/></returns>
        public static T Next<T>(this T value) where T : Enum {
            T[] values = (T[])Enum.GetValues(value.GetType());
            int index = (Array.IndexOf(values, value) + 1) % values.Length;
            return values[index];
        }

        /// <summary> Get the previous <paramref name="value"/> of a <see cref="Enum"/> </summary>
        /// <typeparam name="T">The type of the <see cref="Enum"/></typeparam>
        /// <param name="value">The current value of the <see cref="Enum"/></param>
        /// <returns>The previous <paramref name="value"/> of the <see cref="Enum"/></returns>
        public static T Previous<T>(this T value) where T : Enum {
            T[] values = (T[])Enum.GetValues(value.GetType());
            int index = (Array.IndexOf(values, value) - 1) % values.Length;
            return values[index];
        }

        /// <summary> Get the next <see cref="float"/> above the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="float"/></param>
        /// <returns>The next <see cref="float"/> above the <paramref name="value"/></returns>
        public static float Next(this float value) {
            Debug.Assert(float.IsNormal(value));
            int bits = BitConverter.SingleToInt32Bits(value);
            if (value > 0) {
                return BitConverter.Int32BitsToSingle(bits + 1);
            } else if (value < 0) {
                return BitConverter.Int32BitsToSingle(bits - 1);
            } else {
                return float.Epsilon;
            }
        }

        /// <summary> Get the next <see cref="float"/> above the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="float"/></param>
        /// <returns>The next <see cref="float"/> above the <paramref name="value"/></returns>
        public static float Previous(this float value) {
            Debug.Assert(float.IsNormal(value));
            int bits = BitConverter.SingleToInt32Bits(value);
            if (value > 0) {
                return BitConverter.Int32BitsToSingle(bits - 1);
            } else if (value < 0) {
                return BitConverter.Int32BitsToSingle(bits + 1);
            } else {
                return -float.Epsilon;
            }
        }

        /// <summary> Get the next <see cref="double"/> above the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="double"/></param>
        /// <returns>The next <see cref="double"/> above the <paramref name="value"/></returns>
        public static double Next(this double value) {
            Debug.Assert(double.IsNormal(value));
            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0) {
                return BitConverter.Int64BitsToDouble(bits + 1);
            } else if (value < 0) {
                return BitConverter.Int64BitsToDouble(bits - 1);
            } else {
                return double.Epsilon;
            }
        }

        /// <summary> Get the previous <see cref="double"/> below the specified <paramref name="value"/> </summary>
        /// <param name="value">The current value of the <see cref="double"/></param>
        /// <returns>The previous <see cref="double"/> below the <paramref name="value"/></returns>
        public static double Previous(this double value) {
            Debug.Assert(double.IsNormal(value));
            long bits = BitConverter.DoubleToInt64Bits(value);
            if (value > 0) {
                return BitConverter.Int64BitsToDouble(bits - 1);
            } else if (value < 0) {
                return BitConverter.Int64BitsToDouble(bits + 1);
            } else {
                return -double.Epsilon;
            }
        }
    }
}
