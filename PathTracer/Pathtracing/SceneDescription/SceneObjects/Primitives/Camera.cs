using OpenTK.Mathematics;
using PathTracer.Pathtracing.Paths;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;
using System;

namespace PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives {
    /// <summary> The camera object in the 3d scene </summary>
    public class Camera : Aggregate {
        /// <summary> The screen plane in front of the camera </summary>
        public readonly ScreenPlane ScreenPlane;

        /// <summary> The position of the camera </summary>
        public Vector3 Position { get => position; set => SetPosition(value); }
        /// <summary> The direction the camera is facing </summary>
        public Vector3 ViewDirection { get => viewDirection; set => SetViewDirection(value); }
        /// <summary> The field of view of the camera. It determines the distance to the screen plane </summary>
        public float FOV { get => fov; set => SetFOV(value); }
        /// <summary> Vector going up from the view direction of the camera </summary>
        public Vector3 Up => Vector3.Cross(Left, ViewDirection).Normalized();
        /// <summary> Vector going down from the view direction of the camera </summary>
        public Vector3 Down => -Up;
        /// <summary> Vector going left from the view direction of the camera </summary>
        public Vector3 Left => new Vector3(-ViewDirection.Z, 0, ViewDirection.X).Normalized();
        /// <summary> Vector going right from the view direction of the camera </summary>
        public Vector3 Right => -Left;
        /// <summary> Vector going along the view direction of the camera </summary>
        public Vector3 Front => ViewDirection;
        /// <summary> Vector going from the view direction of the camera </summary>
        public Vector3 Back => -ViewDirection;

        Vector3 position, viewDirection;
        float fov;

        /// <summary> Create a new camera object </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="position">The position of the camera</param>
        /// <param name="viewDirection">The direction the camera is facing</param>
        public Camera(IScreen screen, Vector3? position = null, Vector3? viewDirection = null) {
            if (position.HasValue) this.position = position.Value;
            if (viewDirection.HasValue) this.viewDirection = viewDirection.Value;
            ScreenPlane = new ScreenPlane(this, screen);
        }

        /// <summary> Move the camera in a direction </summary>
        /// <param name="direction">The direction to move the camera in</param>
        public void Move(Vector3 direction) => SetPosition(position + direction);

        /// <summary> Set the position of the <see cref="Camera"/> </summary>
        /// <param name="newPosition">The new position of the <see cref="Camera"/></param>
        public void SetPosition(Vector3 newPosition) {
            position = newPosition;
            ScreenPlane.Update();
        }

        /// <summary> Turn the view direction of the camera </summary>
        /// <param name="direction">The direction to turn the camera in</param>
        public void Rotate(Vector3 direction) => SetViewDirection((viewDirection + direction).Normalized());

        /// <summary> Set the view direction of the camera </summary>
        /// <param name="newViewDirection">The new view direction of the camera</param>
        public void SetViewDirection(Vector3 newViewDirection) {
            viewDirection = newViewDirection.Normalized();
            ScreenPlane.Update();
        }

        /// <summary> Set the field of view of the camera </summary>
        /// <param name="newFOV">The new field of view</param>
        public void SetFOV(float newFOV) {
            fov = newFOV;
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