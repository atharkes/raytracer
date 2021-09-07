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
        public int IntRGB();

        /// <summary> Convert the <see cref="ISpectrum"/> to a vector </summary>
        /// <returns>An RGB vector</returns>
        public Vector3 VectorRGB();
    }
}
