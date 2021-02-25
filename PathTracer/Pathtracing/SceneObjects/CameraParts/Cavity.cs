using OpenTK.Mathematics;
using PathTracer.Utilities;

namespace PathTracer.Pathtracing.SceneObjects.CameraParts {
    /// <summary> A cavity of the accumulator that catches light </summary>
    public class Cavity {
        /// <summary> The amount of light in the cavity </summary>
        public Vector3 Light { get; private set; } = Vector3.Zero;
        /// <summary> Amount of times the bvh is traversed by photons in the cavity </summary>
        public int BVHTraversals { get; private set; } = 0;
        /// <summary> The amount of photons caught by the cavity </summary>
        public int Samples { get; private set; } = 0;

        /// <summary> Average light of the photons in the cavity </summary>
        public Vector3 AverageLight => Light == Vector3.Zero ? Vector3.Zero : Light / Samples;
        /// <summary> Average BVH traversals of photons in the cavity </summary>
        public int AverageBVHTraversals => Samples == 0 ? 0 : BVHTraversals / Samples;
        /// <summary> The green to red color fade for the BVH traversals </summary>
        public Vector3 AverageBVHTraversalColor => Utils.ColorScaleBlackGreenYellowRed(AverageBVHTraversals, 0, 255);

        /// <summary> Add a photon to the cavity </summary>
        /// <param name="photon">The photon to add to the cavity</param>
        public void AddPhoton(Vector3 photon, int bvhTraversals) {
            Light += photon;
            BVHTraversals += bvhTraversals;
            Samples++;
        }

        /// <summary> Clear the light in the cavity </summary>
        public void Clear() {
            Light = Vector3.Zero;
            BVHTraversals = 0;
            Samples = 0;
        }
    }
}
