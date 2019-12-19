using System;
using OpenTK;
using WhittedRaytracer.Raytracing.SceneObjects.Primitives;

namespace WhittedRaytracer.Raytracing.SceneObjects {
    /// <summary> The screen plane object in the 3d scene </summary>
    class ScreenPlane : ISceneObject {
        /// <summary> The camera the screen plane is linked to </summary>
        public readonly Camera Camera;
        /// <summary> The 2d window the screen plane is linked to </summary>
        public readonly IScreen Screen;
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

        /// <summary> The scale to draw the debug information </summary>
        public float DebugScale => Math.Min(Screen.Width, Screen.Height) / 16f;

        Vector3 center;

        /// <summary> Create a new screen plane linked to a camera </summary>
        /// <param name="camera">The camera to link the screen plane to</param>
        /// <param name="screen">The screen to draw the 2d projection to</param>
        public ScreenPlane(Camera camera, IScreen screen) {
            Camera = camera;
            Screen = screen;
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
        }

        /// <summary> Get the position of a pixel on the screen plane in worldspace </summary>
        /// <param name="x">The x position of the pixel</param>
        /// <param name="y">The y position of the pixel</param>
        /// <returns>The position of the pixel in worldspace</returns>
        public Vector3 GetPixelPosition(int x, int y) {
            return TopLeft + (float)x / (Screen.Width - 1) * (TopRight - TopLeft) + (float)y / (Screen.Height - 1) * (BottomLeft - TopLeft);
        }

        #region Debug Drawing
        /// <summary> Draw a ray from topview for debug purposes </summary>
        /// <param name="ray">The ray to draw</param>
        /// <param name="color">The color to draw the ray with</param>
        public void DrawRay(Ray ray, int color = 0xffffff) {
            if (ray.RecursionDepth > 0) color -= ray.RecursionDepth * 0x555511;
            int x1 = TX(ray.Origin.X);
            int y1 = TZ(ray.Origin.Z);
            int x2 = TX(ray.Origin.X + ray.Direction.X * ray.Length);
            int y2 = TZ(ray.Origin.Z + ray.Direction.Z * ray.Length);
            Screen.Line(x1, y1, x2, y2, color);
        }

        /// <summary> Draw the camera from topview for debug purposes </summary>
        /// <param name="color">The color to draw the camera with</param>
        public void DrawCamera(int color = 0xffff00) {
            int x1 = TX(Camera.Position.X) - 1;
            int y1 = TZ(Camera.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            Screen.Box(x1, y1, x2, y2, color);
        }

        /// <summary> Draw the screen plane from topview for debug purposes </summary>
        /// <param name="color">The color to draw the screenplane with</param>
        public void DrawScreenPlane(int color = 0xffff00) {
            Screen.Line(TX(TopLeft.X), TZ(TopLeft.Z), TX(TopRight.X), TZ(TopRight.Z), color);
            Screen.Line(TX(TopRight.X), TZ(TopRight.Z), TX(BottomRight.X), TZ(BottomRight.Z), color);
            Screen.Line(TX(BottomRight.X), TZ(BottomRight.Z), TX(BottomLeft.X), TZ(BottomLeft.Z), color);
            Screen.Line(TX(BottomLeft.X), TZ(BottomLeft.Z), TX(TopLeft.X), TZ(TopLeft.Z), color);
        }

        /// <summary> Draw a lightsource from topview for debug purposes </summary>
        /// <param name="light">The light to draw</param>
        public void DrawLight(PointLight light) {
            int x1 = TX(light.Position.X) - 1;
            int y1 = TZ(light.Position.Z) - 1;
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            Screen.Box(x1, y1, x2, y2, light.Color.ToIntColor());
        }

        /// <summary> Draw a sphere from topview for debug purposes </summary>
        /// <param name="sphere">The sphere to draw</param>
        public void DrawSphere(Sphere sphere) {
            for (int i = 0; i < 128; i++) {
                int x1 = TX(sphere.Position.X + (float)Math.Cos(i / 128f * 2 * Math.PI) * sphere.Radius);
                int y1 = TZ(sphere.Position.Z + (float)Math.Sin(i / 128f * 2 * Math.PI) * sphere.Radius);
                int x2 = TX(sphere.Position.X + (float)Math.Cos((i + 1) / 128f * 2 * Math.PI) * sphere.Radius);
                int y2 = TZ(sphere.Position.Z + (float)Math.Sin((i + 1) / 128f * 2 * Math.PI) * sphere.Radius);
                Screen.Line(x1, y1, x2, y2, sphere.Material.Color.ToIntColor());
            }
        }

        /// <summary> Transform the x coordinate from the 3d scene to the 2d screen in topview </summary>
        /// <param name="x">The x coordinate in the 3d scene</param>
        /// <returns>The x coordinate on the 2d screen</returns>
        public int TX(float x) {
            return Screen.Width / 2 + (int)(DebugScale * (x - Camera.Position.X));
        }

        /// <summary> Transform the z coordinate from the 3d scene to the 2d screen in topview </summary>
        /// <param name="z">The z coordinate in the 3d scene</param>
        /// <returns>The z coordinate on the 2d screen</returns>
        public int TZ(float z) {
            return Screen.Height / 2 + (int)(DebugScale * (z - Camera.Position.Z));
        }
        #endregion
    }
}
