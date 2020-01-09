using OpenTK;

namespace WhittedRaytracer.Raytracing.SceneObjects.CameraParts {
    /// <summary> A cavity of the accumulator that catches light </summary>
    class Cavity {
        /// <summary> The amount of light in the cavity </summary>
        public Vector3 Light { get; private set; } = Vector3.Zero;
        /// <summary> The amount of photons caught by the cavity </summary>
        public int Samples { get; private set; } = 0;

        /// <summary> Add a photon to the cavity </summary>
        /// <param name="photon">The photon to add to the cavity</param>
        public void AddPhoton(Vector3 photon) {
            Light += photon;
            Samples++;
        }

        /// <summary> Clear the light in the cavity </summary>
        public void Clear() {
            Light = Vector3.Zero;
            Samples = 0;
        }
    }
}
