using OpenTK.Mathematics;

namespace PathTracer.Spectra {
    /// <summary> A spectrum of electromagnetic radiation that can contain different wavelengths  </summary>
    public interface ISpectrum {
        /// <summary> The minimum wavelength of visible light </summary>
        public float MinimumWavelength => 4e-7f;
        /// <summary> The maximum wavelength of visible light </summary>
        public float MaximumWavelength => 7e-7f;

        /// <summary> Convert the <see cref="ISpectrum"/> to an integer </summary>
        /// <returns>An RGB integer</returns>
        public int ToRGBInt();

        /// <summary> Convert the <see cref="ISpectrum"/> to a vector </summary>
        /// <returns>An RGB vector</returns>
        public Vector3 ToRGBVector();


        public static ISpectrum operator +(ISpectrum left, ISpectrum right) {
            throw new System.NotImplementedException();
        }

        public static ISpectrum operator -(ISpectrum left, ISpectrum right) {
            throw new System.NotImplementedException();
        }

        public static ISpectrum operator *(ISpectrum left, ISpectrum right) {
            throw new System.NotImplementedException();
        }

        public static ISpectrum operator *(ISpectrum left, float right) {
            throw new System.NotImplementedException();
        }

        public static ISpectrum operator /(ISpectrum left, ISpectrum right) {
            throw new System.NotImplementedException();
        }

        public static ISpectrum operator /(ISpectrum left, float right) {
            throw new System.NotImplementedException();
        }
    }
}
