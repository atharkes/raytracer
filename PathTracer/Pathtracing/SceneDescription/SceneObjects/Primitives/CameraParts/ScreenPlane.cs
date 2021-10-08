using OpenTK.Mathematics;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using System;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts {
    /// <summary> The screen plane object in the 3d scene </summary>
    public class ScreenPlane : ISceneObject {
        /// <summary> The camera the screen plane is linked to </summary>
        public readonly Camera Camera;
        /// <summary> The 2d window the screen plane is linked to </summary>
        public readonly IScreen Screen;
        /// <summary> The accumulator that accumulates light </summary>
        public readonly Accumulator Accumulator;
        /// <summary> The aspect ratio of the screen plane </summary>
        public float AspectRatio => (float)Screen.Width / Screen.Height;

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
        /// <param name="screen">The screen to draw the 2d projection to</param>
        public ScreenPlane(Camera camera, IScreen screen) {
            Camera = camera;
            Screen = screen;
            Accumulator = new Accumulator(screen?.Width ?? 0, screen?.Height ?? 0);
            Update();
        }

        /// <summary> Set the position of the screen plane and move the camera with it </summary>
        /// <param name="newPosition">The new position of the screen plane</param>
        public void SetCenter(Vector3 newPosition) {
            Camera.Position = newPosition - Camera.ViewDirection * 1 / ((float)Math.Tan(Camera.FOV / 360 * Math.PI));
            Update();
        }

        /// <summary> Update the position of the screen plane </summary>
        /// <param name="cam">The camera the screen plane is in front of</param>
        public void Update() {
            center = Camera.Position + Camera.ViewDirection * 1 / ((float)Math.Tan(Camera.FOV / 360 * Math.PI));
            TopLeft = Center + Camera.Left * AspectRatio + Camera.Up;
            TopRight = Center - Camera.Left * AspectRatio + Camera.Up;
            BottomLeft = Center + Camera.Left * AspectRatio - Camera.Up;
            BottomRight = Center - Camera.Left * AspectRatio - Camera.Up;
            Accumulator.Clear();
        }

        /// <summary> Get the position of a pixel on the screen plane in worldspace </summary>
        /// <param name="x">The x position of the pixel</param>
        /// <param name="y">The y position of the pixel</param>
        /// <returns>The position of the pixel in worldspace</returns>
        public Vector3 GetPixelPosition(int x, int y) {
            return TopLeft + (float)x / (Screen.Width - 1) * (TopRight - TopLeft) + (float)y / (Screen.Height - 1) * (BottomLeft - TopLeft);
        }
    }
}
