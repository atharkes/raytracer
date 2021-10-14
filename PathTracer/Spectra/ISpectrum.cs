using OpenTK.Mathematics;

namespace PathTracer.Spectra {
    /// <summary> A spectrum of electromagnetic radiation that can contain different wavelengths  </summary>
    public interface ISpectrum {
        /// <summary> An <see cref="ISpectrum"/> that represents no light </summary>
        static ISpectrum Black => new RGBSpectrum(Vector3.Zero);
        /// <summary> The minimum wavelength of visible light </summary>
        const float MinimumWavelength = 4e-7f;
        /// <summary> The maximum wavelength of visible light </summary>
        const float MaximumWavelength = 7e-7f;

        /// <summary> Whether the <see cref="ISpectrum"/> is black </summary>
        bool IsBlack { get; }

        /// <summary> Convert the <see cref="ISpectrum"/> to an rgb integer </summary>
        /// <returns>An RGB integer</returns>
        int ToRGBInt();

        /// <summary> Convert the <see cref="ISpectrum"/> to an rgb vector </summary>
        /// <returns>An RGB vector</returns>
        Vector3 ToRGBVector();

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
            return (RGBSpectrum)left * right;
        }

        /// <summary> Divide an <see cref="ISpectrum"/> by a value </summary>
        /// <param name="left">The <see cref="ISpectrum"/> to be divided</param>
        /// <param name="right">The value to divide with</param>
        /// <returns>The <paramref name="left"/> <see cref="ISpectrum"/> divided by the <paramref name="right"/> value</returns>
        public static ISpectrum operator /(ISpectrum left, float right) {
            return (RGBSpectrum)left / right;
        }
    }
}
