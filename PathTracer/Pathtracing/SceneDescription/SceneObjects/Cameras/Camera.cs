using OpenTK.Mathematics;
using PathTracer.Pathtracing.Paths;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras.Parts;
using PathTracer.Pathtracing.SceneDescription.Shapes.Planars;
using System;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras {
    /// <summary> An <see cref="ICamera"/> scene object </summary>
    public class Camera : Aggregate, ICamera {
        /// <summary> The position of the <see cref="Camera"/> </summary>
        public Vector3 Position { get => position; set => SetPosition(value); }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="Camera"/> </summary>
        public Quaternion Rotation { get => rotation; set => SetRotation(value); }
        /// <summary> The field of view of the <see cref="Camera"/> </summary>
        public float FOV { get => fov; set => SetFOV(value); }
        /// <summary> The viewing direction of the <see cref="Camera"/> </summary>
        public Vector3 ViewDirection => Rotation * ICamera.DefaultFront;

        readonly ScreenPlane screenPlane;
        Vector3 position;
        Quaternion rotation;
        float fov;

        /// <summary> Create a new camera object </summary>
        /// <param name="position">The position of the <see cref="Camera"/></param>
        /// <param name="rotation">The rotation <see cref="Quaternion"/> of the <see cref="Camera"/></param>
        /// <param name="aspectRatio">The aspect ratio of the <see cref="Camera"/></param>
        /// <param name="fov">The field of view of the <see cref="Camera"/></param>
        public Camera(Vector3 position, Quaternion rotation, float aspectRatio, float fov) {
            this.position  = position;
            this.rotation = rotation;
            this.fov = fov;
            Rectangle screenPlaneRectangle = new(Position, new Vector2(aspectRatio, 1), rotation);
            screenPlane = new ScreenPlane(screenPlaneRectangle, );
            ResetScreenPlane();
            Items.Add(screenPlane);
        }

        /// <summary> Move the <see cref="Camera"/> in a <paramref name="direction"/> </summary>
        /// <param name="direction">The direction in which to move the <see cref="Camera"/></param>
        public void Move(Vector3 direction) => SetPosition(position + direction);

        /// <summary> Set the position of the <see cref="Camera"/> </summary>
        /// <param name="position">The new position of the <see cref="Camera"/></param>
        public void SetPosition(Vector3 position) {
            this.position = position;
            ResetScreenPlane();
        }

        /// <summary> Rotate the <see cref="Camera"/> around a specified <paramref name="axis"/> </summary>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        public void Rotate(Vector3 axis, float degrees) => SetRotation(Rotation * Quaternion.FromAxisAngle(axis, degrees).Normalized());

        /// <summary> Set the rotation <see cref="Quaternion"/> of the <see cref="Camera"/> </summary>
        /// <param name="rotation">The new rotation <see cref="Quaternion"/></param>
        public void SetRotation(Quaternion rotation) {
            this.rotation = rotation;
            ResetScreenPlane();
        }

        /// <summary> Set the view direction of the camera </summary>
        /// <param name="direction">The direction the <see cref="Camera"/> will view towards</param>
        public void SetViewDirection(Vector3 direction) {
            Vector3 side = Vector3.Cross(direction, Vector3.UnitY).Normalized();
            Vector3 up = Vector3.Cross(side, direction).Normalized();
            Matrix3 lookAtMatrix = new(side, up, direction);
            lookAtMatrix.Transpose();
            SetRotation(Quaternion.FromMatrix(lookAtMatrix));
        }

        /// <summary> Set the field of view of the camera </summary>
        /// <param name="degrees">The new degrees of the field of view</param>
        public void SetFOV(float degrees) {
            fov = degrees;
            ResetScreenPlane();
        }

        public void ResetScreenPlane() {
            float distance = 0.5f * screenPlane.Rectangle.Height / ((float)Math.Tan(FOV / 360 * Math.PI));
            screenPlane.Rectangle.Position = Position + ViewDirection * distance;
            screenPlane.Rectangle.Rotation = Rotation;
            screenPlane.Accumulator.Clear();
        }

        /// <summary> Get a random amount of camera rays </summary>
        /// <param name="amount">The amount of random rays to get</param>
        /// <returns>An array with random camera rays</returns>
        public CameraRay[] GetCameraRays(int amount, Random random) {
            CameraRay[] rays = new CameraRay[amount];
            for (int i = 0; i < amount; i++) {
                int x = random.Next(0, screenPlane.Screen.Width);
                int y = random.Next(0, screenPlane.Screen.Height);
                rays[i] = CreateCameraRay(x, y);
            }
            return rays;
        }

        /// <summary> Create a primary ray from the camera through a pixel on the screen plane </summary>
        /// <param name="x">The x position of the pixel</param>
        /// <param name="y">The y position of the pixel</param>
        /// <returns>The ray from the camera through the screen plane</returns>
        public CameraRay CreateCameraRay(int x, int y, float xOffset, float yOffset) {
            Cavity cavity = screenPlane.Accumulator.Cavities[x + y * screenPlane.Screen.Width];
            SurfacePoint origin = new SurfacePoint(cavity, Position, Front);
            Vector3 direction = screenPlane.GetPixelPosition(x, y) - Position;
            return new CameraRay(origin, direction, cavity);
        }
    }
}