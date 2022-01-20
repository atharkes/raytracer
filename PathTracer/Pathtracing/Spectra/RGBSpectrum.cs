using PathTracer.Geometry.Vectors;
using System;
using System.Diagnostics;

namespace PathTracer.Pathtracing.Spectra {
    /// <summary> A color <see cref="ISpectrum"/> that holds a single value for red, green, and blue </summary>
    public struct RGBSpectrum : ISpectrum, IEquatable<RGBSpectrum> {
        /// <summary> The <see cref="RGBSpectrum"/> that represents no light </summary>
        public static readonly RGBSpectrum Black = new(0f);
        /// <summary> The <see cref="RGBSpectrum"/> that represents light of all wavelengths </summary>
        public static readonly RGBSpectrum White = new(1f);

        /// <summary> The color representing off-white </summary>
        public static readonly RGBSpectrum OffWhite = new(0.9f);
        /// <summary> The color representing gray </summary>
        public static readonly RGBSpectrum LightGray = new(0.8f);
        /// <summary> The color representing dark gray </summary>
        public static readonly RGBSpectrum Gray = new(0.7f);
        /// <summary> The color representing dark gray </summary>
        public static readonly RGBSpectrum DarkGray = new(0.4f);
        /// <summary> The color representing red </summary>
        public static readonly RGBSpectrum Red = new(0.8f, 0.2f, 0.2f);
        /// <summary> The color representing green </summary>
        public static readonly RGBSpectrum Green = new(0.2f, 0.8f, 0.2f);
        /// <summary> The color representing blue </summary>
        public static readonly RGBSpectrum Blue = new(0.2f, 0.2f, 0.8f);
        /// <summary> The color representing yellow </summary>
        public static readonly RGBSpectrum Yellow = new(0.8f, 0.8f, 0.2f);
        /// <summary> The color representing purple </summary>
        public static readonly RGBSpectrum Purple = new(0.8f, 0.2f, 0.8f);
        /// <summary> The color representing cyan </summary>
        public static readonly RGBSpectrum Cyan = new(0.2f, 0.8f, 0.8f);

        readonly Vector3 rgb;

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

        /// <summary> Create an <see cref="RGBSpectrum"/> with the same red green and blue values </summary>
        /// <param name="rgb">The red green and blue value</param>
        public RGBSpectrum(float rgb) : this(rgb, rgb, rgb) { }

        /// <summary> Convert the <see cref="RGBSpectrum"/> to an <see cref="RGBSpectrum"/> </summary>
        /// <returns>An <see cref="RGBSpectrum"/></returns>
        public RGBSpectrum ToRGBSpectrum() => this;

        /// <summary> Convert the <see cref="RGBSpectrum"/> to an rgb vector </summary>
        /// <returns>An rgb vector</returns>
        public Vector3 ToRGBVector() => rgb;

        /// <summary> Convert the <see cref="RGBSpectrum"/> to an rgb integer </summary>
        /// <returns>An rgb integer</returns>
        public int ToRGBInt() {
            Vector3 color = Vector3.Clamp(rgb, Vector3.Zero, Vector3.One);
            int r = ((int)(color.X * 255f)) << 16;
            int g = ((int)(color.Y * 255f)) << 8;
            int b = ((int)(color.Z * 255f)) << 0;
            return r + g + b;
        }

        public override string ToString() => rgb.ToString();
        public string ToString(string? format) => rgb.ToString(format);
        public override int GetHashCode() => rgb.GetHashCode();
        public override bool Equals(object? obj) => obj is RGBSpectrum rgb && Equals(rgb);
        public bool Equals(ISpectrum? other) => other is RGBSpectrum rgb && Equals(rgb);
        public bool Equals(RGBSpectrum other) => rgb.Equals(other.rgb);

        public static bool operator ==(RGBSpectrum left, RGBSpectrum right) => left.Equals(right);
        public static bool operator !=(RGBSpectrum left, RGBSpectrum right) => !(left == right);

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
