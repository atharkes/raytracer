using OpenTK.Mathematics;
using PathTracer.Pathtracing.Paths;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;
using System;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives {
    /// <summary> The camera object in the 3d scene </summary>
    public class Camera : Aggregate {
        /// <summary> The default viewing direction of the <see cref="Camera"/> when no rotation is applied </summary>
        public static readonly Vector3 DefaultFront = Vector3.UnitZ;
        /// <summary> The default right vector of the <see cref="Camera"/> when no rotation is applied </summary>
        public static readonly Vector3 DefaultRight = Vector3.UnitX;
        /// <summary> The default up vector of the <see cref="Camera"/> when no rotation is applied </summary>
        public static readonly Vector3 DefaultUp = Vector3.UnitY;

        /// <summary> The screen plane in front of the camera </summary>
        public readonly ScreenPlane ScreenPlane;

        /// <summary> The position of the <see cref="Camera"/> </summary>
        public Vector3 Position { get => position; set => SetPosition(value); }
        /// <summary> The rotation <see cref="Quaternion"/> of the <see cref="Camera"/> </summary>
        public Quaternion Rotation { get => rotation; set => SetRotation(value); }
        /// <summary> The field of view of the camera. It determines the distance to the screen plane </summary>
        public float FOV { get => fov; set => SetFOV(value); }

        /// <summary> The direction the camera is facing </summary>
        public Vector3 ViewDirection { get => Rotation * DefaultFront; set => SetViewDirection(value); }
        /// <summary> Vector going along the view direction of the camera </summary>
        public Vector3 Front => ViewDirection;
        /// <summary> Vector going from the view direction of the camera </summary>
        public Vector3 Back => -ViewDirection;
        /// <summary> Vector going up from the view direction of the camera </summary>
        public Vector3 Up => Rotation * DefaultUp;
        /// <summary> Vector going down from the view direction of the camera </summary>
        public Vector3 Down => -Up;
        /// <summary> Vector going right from the view direction of the camera </summary>
        public Vector3 Right => Rotation * DefaultRight;
        /// <summary> Vector going left from the view direction of the camera </summary>
        public Vector3 Left => -Right;

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
            ScreenPlane = new ScreenPlane(this, screen);
        }

        /// <summary> Move the camera in a direction </summary>
        /// <param name="direction">The direction to move the camera in</param>
        public void Move(Vector3 direction) => SetPosition(position + direction);

        /// <summary> Set the position of the <see cref="Camera"/> </summary>
        /// <param name="position">The new position of the <see cref="Camera"/></param>
        public void SetPosition(Vector3 position) {
            this.position = position;
            ScreenPlane.Update();
        }

        /// <summary> Turn the view direction of the <see cref="Camera"/> </summary>
        /// <param name="axis">The axis to rotate around</param>
        /// <param name="degrees">The amount of degrees to rotate</param>
        public void Rotate(Vector3 axis, float degrees) => SetRotation(Rotation * Quaternion.FromAxisAngle(axis, degrees).Normalized());

        /// <summary> Set the rotation <see cref="Quaternion"/> of the <see cref="Camera"/> </summary>
        /// <param name="rotation">The new rotation <see cref="Quaternion"/></param>
        public void SetRotation(Quaternion rotation) {
            this.rotation = rotation;
            ScreenPlane.Update();
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
            ScreenPlane.Update();
        }

        /// <summary> Get a random amount of camera rays </summary>
        /// <param name="amount">The amount of random rays to get</param>
        /// <returns>An array with random camera rays</returns>
        public CameraRay[] GetCameraRays(int amount, Random random) {
            CameraRay[] rays = new CameraRay[amount];
            for (int i = 0; i < amount; i++) {
                int x = random.Next(0, ScreenPlane.Screen.Width);
                int y = random.Next(0, ScreenPlane.Screen.Height);
                rays[i] = CreateCameraRay(x, y);
            }
            return rays;
        }

        /// <summary> Create a primary ray from the camera through a pixel on the screen plane </summary>
        /// <param name="x">The x position of the pixel</param>
        /// <param name="y">The y position of the pixel</param>
        /// <returns>The ray from the camera through the screen plane</returns>
        public CameraRay CreateCameraRay(int x, int y, float xOffset, float yOffset) {
            Cavity cavity = ScreenPlane.Accumulator.Cavities[x + y * ScreenPlane.Screen.Width];
            SurfacePoint origin = new SurfacePoint(cavity, Position, Front);
            Vector3 direction = ScreenPlane.GetPixelPosition(x, y) - Position;
            return new CameraRay(origin, direction, cavity);
        }
    }
}