using OpenTK.Mathematics;
using System;

namespace PathTracer.Spectra {
    public abstract class Spectrum : ISpectrum {
        public int IntRGB() {
            throw new NotImplementedException();
        }

        public Vector3 VectorRGB() {
            throw new NotImplementedException();
        }
    }
}
