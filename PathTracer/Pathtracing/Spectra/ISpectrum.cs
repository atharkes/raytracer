using PathTracer.Geometry.Vectors;
using System;

namespace PathTracer.Pathtracing.Spectra {
    /// <summary> A spectrum of electromagnetic radiation that can contain different wavelengths  </summary>
    public interface ISpectrum : IEquatable<ISpectrum> {
        /// <summary> The minimum wavelength of visible light </summary>
        const float MinimumWavelength = 4e-7f;
        /// <summary> The maximum wavelength of visible light </summary>
        const float MaximumWavelength = 7e-7f;

        /// <summary> The <see cref="ISpectrum"/> that represents no light </summary>
        static readonly ISpectrum Black = RGBColors.Black;
        /// <summary> The <see cref="ISpectrum"/> that represents light of all wavelengths </summary>
        static readonly ISpectrum White = RGBColors.White;

        /// <summary> Whether the <see cref="ISpectrum"/> is black </summary>
        bool IsBlack => Equals(Black);
        /// <summary> Whether the <see cref="ISpectrum"/> is white </summary>
        bool IsWhite => Equals(White);

        /// <summary> Convert the <see cref="ISpectrum"/> to an <see cref="RGBSpectrum"/> </summary>
        /// <returns>An <see cref="RGBSpectrum"/></returns>
        RGBSpectrum ToRGBSpectrum();

        /// <summary> Convert the <see cref="ISpectrum"/> to an rgb <see cref="Vector3"/> </summary>
        /// <returns>An RGB <see cref="Vector3"/></returns>
        Vector3 ToRGBVector();

        /// <summary> Convert the <see cref="ISpectrum"/> to an rgb <see cref="int"/> </summary>
        /// <returns>An RGB <see cref="int"/></returns>
        int ToRGBInt();

        /// <summary> Convert the <see cref="ISpectrum"/> to a <see cref="string"/> </summary>
        /// <returns>The <see cref="string"/></returns>
        string ToString();

        /// <summary> Convert the <see cref="ISpectrum"/> to a <see cref="string"/> using a <paramref name="format"/> </summary>
        /// <param name="format">The format of the <see cref="string"/></param>
        /// <returns>The <see cref="string"/> representing the <see cref="ISpectrum"/></returns>
        string ToString(string? format);

        /// <summary> Check whether the <see cref="ISpectrum"/> is equal to an <paramref name="other"/> </summary>
        /// <param name="other">The other <see cref="ISpectrum"/></param>
        /// <returns>Whether the <see cref="ISpectrum"/> is equal to the <paramref name="other"/></returns>
        bool IEquatable<ISpectrum>.Equals(ISpectrum? other) => this is RGBSpectrum rgb && rgb.Equals(other);

        /// <summary> Add two <see cref="ISpectrum"/> </summary>
        /// <param name="left">The left <see cref="ISpectrum"/></param>
        /// <param name="right">The right <see cref="ISpectrum"/></param>
        /// <returns>The two <see cref="ISpectrum"/> added</returns>
        public static ISpectrum operator +(ISpectrum left, ISpectrum right) {
            return (RGBSpectrum)left + (RGBSpectrum)right;
        }

        /// <summary> Subtract two <see cref="ISpectrum"/> </summary>
        /// <param name="left">The <see cref="ISpectrum"/> to subtract from</param>
        /// <param name="right">The <see cref="ISpectrum"/> used for the subtraction</param>
        /// <returns>The <paramref name="right"/> <see cref="ISpectrum"/> subtracted from the <paramref name="left"/> <see cref="ISpectrum"/></returns>
        public static ISpectrum operator -(ISpectrum left, ISpectrum right) {
            return (RGBSpectrum)left - (RGBSpectrum)right;
        }

        /// <summary> Multiply two <see cref="ISpectrum"/> </summary>
        /// <param name="left">The left <see cref="ISpectrum"/></param>
        /// <param name="right">The right <see cref="ISpectrum"/></param>
        /// <returns>The two <see cref="ISpectrum"/> multiplied</returns>
        public static ISpectrum operator *(ISpectrum left, ISpectrum right) {
            return (RGBSpectrum)left * (RGBSpectrum)right;
        }

        /// <summary> Multiple an <see cref="ISpectrum"/> by a value </summary>
        /// <param name="left">The left <see cref="ISpectrum"/></param>
        /// <param name="right">The value to multiply with</param>
        /// <returns>The <paramref name="left"/> <see cref="ISpectrum"/> multiplied by the <paramref name="right"/> value</returns>
        public static ISpectrum operator *(ISpectrum left, float right) {
            return left is RGBSpectrum rgb ? rgb * right : throw new NotImplementedException();
        }

        /// <summary> Divide an <see cref="ISpectrum"/> by a value </summary>
        /// <param name="left">The <see cref="ISpectrum"/> to be divided</param>
        /// <param name="right">The value to divide with</param>
        /// <returns>The <paramref name="left"/> <see cref="ISpectrum"/> divided by the <paramref name="right"/> value</returns>
        public static ISpectrum operator /(ISpectrum left, float right) {
            return left is RGBSpectrum rgb ? rgb / right : throw new NotImplementedException();
        }
    }
}
