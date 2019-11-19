using System;
using OpenTK;

namespace Raytracer {
    /// <summary> The screen plane object in the 3d scene </summary>
    class ScreenPlane {
        /// <summary> The center of the screen plane </summary>
        public Vector3 Center;
        /// <summary> A corner of the screen plane </summary>
        public Vector3 TopLeft, TopRight, BottomLeft, BottomRight;

        /// <summary> Update the position of the screen plane </summary>
        /// <param name="cam">The camera the screen plane is in front of</param>
        public void UpdatePosition(Camera cam) {
            Center = cam.Position + cam.ViewDirection * 1 / ((float)Math.Tan(cam.FOV / 360 * Math.PI));
            TopLeft = Center + cam.Left + cam.Up;
            TopRight = Center - cam.Left + cam.Up;
            BottomLeft = Center + cam.Left - cam.Up;
            BottomRight = Center - cam.Left - cam.Up;
        }
    }
}
