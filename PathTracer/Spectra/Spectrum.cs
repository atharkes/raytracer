using OpenTK.Mathematics;
using System;

namespace PathTracer.Spectra {
    public abstract class Spectrum : ISpectrum {
        public int ToRGBInt() {
            throw new NotImplementedException();
        }

        public Vector3 ToRGBVector() {
            throw new NotImplementedException();
        }
    }
}
