using OpenTK;

namespace WhittedRaytracer.Raytracing.SceneObjects {
    /// <summary> A lightsource for the 3d scene </summary>
    class Lightsource : ISceneObject {
        /// <summary> The position of the lightsource </summary>
        public Vector3 Position { get; set; }
        /// <summary> The color of the lightsource </summary>
        public Vector3 Color { get; set; }

        /// <summary> Create a new lightsource for the 3d scene </summary>
        /// <param name="position">The position of the lightsource</param>
        /// <param name="color">The color of the lightsource</param>
        public Lightsource(Vector3 position, Vector3 color) {
            Position = position;
            Color = color;
        }
    }
}