using OpenTK.Mathematics;
using PathTracer.Pathtracing.Rays;
using PathTracer.Pathtracing.SceneObjects.CameraParts;
using PathTracer.Utilities;
using System;

namespace PathTracer.Pathtracing.SceneObjects {
    /// <summary> The camera object in the 3d scene </summary>
    public class Camera : ISceneObject {
        /// <summary> The screen plane in front of the camera </summary>
        public readonly ScreenPlane ScreenPlane;
        /// <summary> The input handler of the camera </summary>
        public readonly InputHandler InputHandler;
        /// <summary> The statitics of the camera </summary>
        public readonly Statistics Statistics = new Statistics();
        /// <summary> The configuration of the camera </summary>
        public readonly Config Config = Config.LoadFromFile();

        /// <summary> The position of the camera </summary>
        public Vector3 Position { get => Config.Position; set => Move(value - Position); }
        /// <summary> The direction the camera is facing </summary>
        public Vector3 ViewDirection { get => Config.ViewDirection; set => SetViewDirection(value); }
        /// <summary> The field of view of the camera. It determines the distance to the screen plane </summary>
        public float FOV { get => Config.FOV; set => SetFOV(value); }
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

        /// <summary> Create a new camera object </summary>
        /// <param name="screen">The screen to draw the raytracing to</param>
        /// <param name="position">The position of the camera</param>
        /// <param name="viewDirection">The direction the camera is facing</param>
        public Camera(IScreen screen, Vector3? position = null, Vector3? viewDirection = null) {
            if (position.HasValue) Config.Position = position.Value;
            if (viewDirection.HasValue) Config.ViewDirection = viewDirection.Value;
            ScreenPlane = new ScreenPlane(this, screen);
            InputHandler = new InputHandler(this);
        }

        /// <summary> Move the camera in a direction </summary>
        /// <param name="direction">The direction to move the camera in</param>
        public void Move(Vector3 direction) {
            Config.Position += direction * Config.MoveSpeed;
            ScreenPlane.Update();
        }
        
        /// <summary> Turn the view direction of the camera </summary>
        /// <param name="direction">The direction to turn the camera in</param>
        public void Turn(Vector3 direction) {
            Vector3 newDirection = Config.ViewDirection + direction * Config.ViewSensitivity;
            Config.ViewDirection = newDirection.Normalized();
            ScreenPlane.Update();
        }

        /// <summary> Set the view direction of the camera </summary>
        /// <param name="newViewDirection">The new view direction of the camera</param>
        public void SetViewDirection(Vector3 newViewDirection) {
            Config.ViewDirection = newViewDirection.Normalized();
            ScreenPlane.Update();
        }

        /// <summary> Set the field of view of the camera </summary>
        /// <param name="newFOV">The new field of view</param>
        public void SetFOV(float newFOV) {
            Config.FOV = newFOV;
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
        public CameraRay CreateCameraRay(int x, int y) {
            return new CameraRay(Position, ScreenPlane.GetPixelPosition(x, y) - Position, ScreenPlane.Accumulator.Cavities[x + y * ScreenPlane.Screen.Width]);
        }

        /// <summary> Returns how many rays should be traced in the next tick </summary>
        /// <returns>Amount of rays to trace in the next tick</returns>
        public int RayCountNextTick() {
            if (Statistics.FrameTime.LastTick == default) return Config.MinimumRayCount;
            double frameTimeDifference = Config.TargetFrameTime.TotalMilliseconds - (Statistics.FrameTime.LastTick.TotalMilliseconds + Statistics.MultithreadingOverhead.LastTick.TotalMilliseconds);
            double excessTraceFactor = frameTimeDifference / Statistics.TracingTime.LastTick.TotalMilliseconds;
            return Math.Max(Config.MinimumRayCount, (int)(Statistics.RaysTracedLastTick * (1f + excessTraceFactor)));
        }
    }
}