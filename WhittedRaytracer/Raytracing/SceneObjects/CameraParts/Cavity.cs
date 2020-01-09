using OpenTK;

namespace WhittedRaytracer.Raytracing.SceneObjects.CameraObjects {
    /// <summary> A cavity of the accumulator that catches light </summary>
    struct Cavity {
        /// <summary> The amount of light in the cavity </summary>
        public Vector3 Light;
        /// <summary> The amount of photons caught by the cavity </summary>
        public int Samples;

        /// <summary> Add a photon to the cavity </summary>
        /// <param name="photon">The photon to add to the cavity</param>
        public void AddPhoton(Vector3 photon) {
            Light += photon;
            Samples++;
        }
    }
}
