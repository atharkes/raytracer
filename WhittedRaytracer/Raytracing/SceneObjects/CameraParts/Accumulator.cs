using OpenTK;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer.Raytracing.SceneObjects.CameraObjects {
    /// <summary> An accumulator that accumulates the light of photons </summary>
    class Accumulator {
        /// <summary> The width of the accumulator </summary>
        public readonly int Width;
        /// <summary> The height of the accumulator </summary>
        public readonly int Height;

        /// <summary> The cavities in which the light is accumulated </summary>
        Cavity[] cavities;

        /// <summary> Create a new accumulator </summary>
        /// <param name="width">The width of the accumulator</param>
        /// <param name="height">The height of the accumulator</param>
        public Accumulator(int width, int height) {
            Width = width;
            Height = height;
            cavities = new Cavity[width * height];
        }

        /// <summary> Draws an image from the photons in the accumulator to a screen </summary>
        public void DrawImage(IScreen screen) {
            for (int i = 0; i < cavities.Length; i++) {
                screen.Plot(i, (cavities[i].Light / cavities[i].Samples).ToIntColor());
            }
        }

        /// <summary> Clear the accumulator </summary>
        public void Clear() {
            cavities = new Cavity[Width * Height];
        }

        /// <summary> Add a photon to a cavity in the accumulator </summary>
        /// <param name="x">The x coordinate of the cavity</param>
        /// <param name="y">The y coordinate of the cavity</param>
        /// <param name="photon">The photon to add to a cavity in the accumulator</param>
        public void AddPhoton(int x, int y, Vector3 photon) {
            cavities[x + y * Width].AddPhoton(photon);
        }
    }
}
