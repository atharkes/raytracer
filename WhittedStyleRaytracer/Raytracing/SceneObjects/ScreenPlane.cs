using System;
using OpenTK;

namespace WhittedStyleRaytracer.Raytracing.SceneObjects {
    /// <summary> The screen plane object in the 3d scene </summary>
    class ScreenPlane : ISceneObject {
        /// <summary> The camera the screen plane is linked to </summary>
        public readonly Camera Camera;
        /// <summary> The 2d window the screen plane is linked to </summary>
        public readonly IScreen Screen;

        /// <summary> The position of the screen plane. Equals the center of the screen plane </summary>
        public Vector3 Position { get => Center; set => Center = value; }
        /// <summary> The center of the screen plane </summary>
        public Vector3 Center { get => center; set => SetCenter(value); }

        /// <summary> The top left corner of the screen plane </summary>
        public Vector3 TopLeft { get; private set; }
        /// <summary> The top right corner of the screen plane </summary>
        public Vector3 TopRight { get; private set; }
        /// <summary> The bottom left corner of the screen plane </summary>
        public Vector3 BottomLeft { get; private set; }
        /// <summary> The bottom right corner of the screen plane </summary>
        public Vector3 BottomRight { get; private set; }

        Vector3 center;

        /// <summary> Create a new screen plane linked to a camera </summary>
        /// <param name="camera">The camera to link the screen plane to</param>
        public ScreenPlane(Camera camera, IScreen screen) {
            Camera = camera;
            Screen = screen;
            UpdatePosition();
        }

        /// <summary> Set the position of the screen plane and move the camera with it </summary>
        /// <param name="newPosition">The new position of the screen plane</param>
        public void SetCenter(Vector3 newPosition) {
            Camera.Position = newPosition - Camera.ViewDirection * 1 / ((float)Math.Tan(Camera.FOV / 360 * Math.PI));
            UpdatePosition();
        }

        /// <summary> Update the position of the screen plane </summary>
        /// <param name="cam">The camera the screen plane is in front of</param>
        public void UpdatePosition() {
            center = Camera.Position + Camera.ViewDirection * 1 / ((float)Math.Tan(Camera.FOV / 360 * Math.PI));
            TopLeft = center + Camera.Left + Camera.Up;
            TopRight = center - Camera.Left + Camera.Up;
            BottomLeft = center + Camera.Left - Camera.Up;
            BottomRight = center - Camera.Left - Camera.Up;
        }
    }
}
