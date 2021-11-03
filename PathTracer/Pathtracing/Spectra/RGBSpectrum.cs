using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Spectra {
    /// <summary> A color <see cref="ISpectrum"/> that holds a single value for red, green, and blue </summary>
    public struct RGBSpectrum : ISpectrum, IEquatable<RGBSpectrum> {
        Vector3 rgb;

        /// <summary> Create an <see cref="RGBSpectrum"/> using an <paramref name="rgb"/> vector </summary>
        /// <param name="rgb">The rgb vector that holds the colors</param>
        public RGBSpectrum(Vector3 rgb) {
            Debug.Assert(!Vector3.IsNaN(rgb.Value));
            this.rgb = rgb;
        }

        /// <summary> Create an <see cref="RGBSpectrum"/> using individual <paramref name="red"/>, <paramref name="green"/>, and <paramref name="blue"/> values </summary>
        /// <param name="red">The red component</param>
        /// <param name="green">The green component</param>
        /// <param name="blue">The blue component</param>
        public RGBSpectrum(float red, float green, float blue) {
            rgb = new Vector3(red, green, blue);
        }

        /// <summary> Convert the <see cref="RGBSpectrum"/> to an rgb vector </summary>
        /// <returns>An rgb vector</returns>
        public Vector3 ToRGBVector() => rgb;

        /// <summary> Convert the <see cref="ISpectrum"/> to an rgb integer </summary>
        /// <returns>An rgb integer</returns>
        public int ToRGBInt() {
            Vector3 color = Vector3.Clamp(rgb, Vector3.Zero, Vector3.One);
            int r = (int)(color.X * 255) << 16;
            int g = (int)(color.Y * 255) << 8;
            int b = (int)(color.Z * 255) << 0;
            return r + g + b;
        }

        public static bool operator ==(RGBSpectrum left, RGBSpectrum right) => left.Equals(right);
        public static bool operator !=(RGBSpectrum left, RGBSpectrum right) => !(left == right);

        /// <summary> Convert the <see cref="RGBSpectrum"/> to a <see cref="string"/> </summary>
        /// <returns>A <see cref="string"/> representing the <see cref="RGBSpectrum"/></returns>
        public override string ToString() => rgb.ToString();
        public override int GetHashCode() => rgb.GetHashCode();
        public override bool Equals(object? obj) => obj is RGBSpectrum s && Equals(s);
        public bool Equals(ISpectrum? other) => other is RGBSpectrum s && Equals(s);
        public bool Equals(RGBSpectrum other) => rgb.Equals(other.rgb);

        /// <summary> Add two <see cref="RGBSpectrum"/> </summary>
        /// <param name="left">The left <see cref="RGBSpectrum"/></param>
        /// <param name="right">The right <see cref="RGBSpectrum"/></param>
        /// <returns>The two <see cref="RGBSpectrum"/> added</returns>
        public static RGBSpectrum operator +(RGBSpectrum left, RGBSpectrum right) => new(left.rgb + right.rgb);

        /// <summary> Subtract two <see cref="RGBSpectrum"/> </summary>
        /// <param name="left">The <see cref="RGBSpectrum"/> to subtract from</param>
        /// <param name="right">The <see cref="RGBSpectrum"/> used for the subtraction</param>
        /// <returns>The <paramref name="right"/> <see cref="RGBSpectrum"/> subtracted from the <paramref name="left"/> <see cref="RGBSpectrum"/></returns>
        public static RGBSpectrum operator -(RGBSpectrum left, RGBSpectrum right) => new(left.rgb - right.rgb);

        /// <summary> Multiply two <see cref="RGBSpectrum"/> </summary>
        /// <param name="left">The left <see cref="RGBSpectrum"/></param>
        /// <param name="right">The right <see cref="RGBSpectrum"/></param>
        /// <returns>The two <see cref="RGBSpectrum"/> multiplied</returns>
        public static RGBSpectrum operator *(RGBSpectrum left, RGBSpectrum right) => new(left.rgb * right.rgb);

        /// <summary> Multiple an <see cref="RGBSpectrum"/> by a value </summary>
        /// <param name="left">The left <see cref="RGBSpectrum"/></param>
        /// <param name="right">The value to multiply with</param>
        /// <returns>The <paramref name="left"/> <see cref="RGBSpectrum"/> multiplied by the <paramref name="right"/> value</returns>
        public static RGBSpectrum operator *(RGBSpectrum left, float right) => new(left.rgb * right);

        /// <summary> Divide an <see cref="RGBSpectrum"/> by a value </summary>
        /// <param name="left">The <see cref="RGBSpectrum"/> to be divided</param>
        /// <param name="right">The value to divide with</param>
        /// <returns>The <paramref name="left"/> <see cref="RGBSpectrum"/> divided by the <paramref name="right"/> value</returns>
        public static RGBSpectrum operator /(RGBSpectrum left, float right) => new(left.rgb / right);
    }
}
