using OpenTK;

namespace Raytracer.Objects {
    /// <summary> A lightsource for the 3d scene </summary>
    class Lightsource {
        /// <summary> The position of the lightsource </summary>
        public Vector3 Position;
        /// <summary> The color of the lightsource </summary>
        public Vector3 Color;

        /// <summary> Create a new lightsource for the 3d scene </summary>
        /// <param name="position">The position of the lightsource</param>
        /// <param name="color">The color of the lightsource</param>
        public Lightsource(Vector3 position, Vector3 color) {
            Position = position;
            Color = color;
        }
    }
}