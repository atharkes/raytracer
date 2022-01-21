using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Observers.Accumulators;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.Observers {
    /// <summary> An <see cref="IObserver"/> that observes a <see cref="IScene"/> </summary>
    public class Observer : IObserver {
        /// <summary> The virtual <see cref="ICamera"/> object of the <see cref="Observer"/> </summary>
        public ICamera Camera { get; }
        /// <summary> The <see cref="IAccumulator"/> used for storing the samples registered by the <see cref="Observer"/> </summary>
        public IAccumulator Accumulator { get; private set; }
        /// <summary> The <see cref="IScreen"/> used for visual output to the <see cref="Observer"/> </summary>
        public IScreen Screen { get; }

        /// <summary> The targeted framerate of the raytracer </summary>
        public int TargetFrameRate { get; set; } = 20;
        /// <summary> Whether to draw the amount of BVH traversals instead of normal light </summary>
        public DrawingMode DrawingMode { get; set; }
        /// <summary> Whether it is drawing debug information </summary>
        public DebugOutput DebugInfo { get; set; }
        /// <summary> The color of the debug output </summary>
        public int DebugColor { get; set; } = 0xffffff;
        /// <summary> The debug format for floating point output </summary>
        public string Format { get; set; } = "0.00";
        /// <summary> Whether the camera is locked in place </summary>
        public bool CameraLock { get; set; }

        /// <summary> The speed at which the camera moves </summary>
        public float MoveSpeed { get; set; } = 0.05f;
        /// <summary> The sensitivity of turning the camera </summary>
        public float RotateSensitivity { get; set; } = 0.05f;
        /// <summary> The sensitivity when changing the FOV of the camera </summary>
        public float FOVSensitivity { get; set; } = 0.05f;

        /// <summary> Create a new <see cref="Observer"/> </summary>
        /// <param name="screen">The <see cref="IScreen"/> of the <see cref="Observer"/></param>
        /// <param name="camera">The <see cref="ICamera"/> of the <see cref="Observer"/></param>
        public Observer(IScreen screen, ICamera camera) {
            Camera = camera;
            Accumulator = new Accumulator(screen.Width, screen.Height);
            Screen = screen;
            Screen.Resize += (e) => Accumulator = new Accumulator(e.Width, e.Height);
            Screen.Resize += (e) => Camera.AspectRatio = (float)e.Width / e.Height;
            Camera.OnMoved += (_, _) => Accumulator.Clear();
            Camera.Film.SampleRegistered += (_, sample) => Accumulator.Add(sample);
        }

        /// <summary> Draw a frame to the screen of the <see cref="IObserver"/> </summary>
        public void DrawFrame(Statistics statistics) {
            Screen.Clear();
            Accumulator.DrawImage(Screen, DrawingMode);
            DrawDebugInformation(statistics);
        }

        void DrawDebugInformation(Statistics stats) {
            switch (DebugInfo) {
                case DebugOutput.None:
                    break;
                case DebugOutput.FrameTimes:
                    Screen.Print($"Frame Times", 1, 1, DebugColor);
                    Screen.Print($"FPS: {(1000f / stats.FrameTime.LastTick.TotalMilliseconds).ToString(Format)}", 1, 17, DebugColor);
                    Screen.Print($"FPS (target): {TargetFrameRate}", 1, 33, DebugColor);
                    Screen.Print($"Samples/frame: {stats.SampleCountLastTick}", 1, 49, DebugColor);
                    Screen.Print($"Frametime (ms): {stats.FrameTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 65, DebugColor);
                    Screen.Print($"Integrator time (ms): {stats.IntegratorTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 81, DebugColor);
                    Screen.Print($"Drawing time (ms): {stats.DrawingTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 97, DebugColor);
                    Screen.Print($"OpenTK time (ms): {stats.OpenTKTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 113, DebugColor);
                    break;
                case DebugOutput.Configuration:
                    Screen.Print($"Camera Configuration", 1, 1, DebugColor);
                    Screen.Print($"Lock (L): {CameraLock}", 1, 17, DebugColor);
                    Screen.Print($"Position: {Camera.Position.ToString(Format)}", 1, 33, DebugColor);
                    Screen.Print($"View direction: {Camera.ViewDirection.ToString(Format)}", 1, 49, DebugColor);
                    Screen.Print($"FOV: {Camera.FieldOfViewAngles.ToString(Format)}", 1, 65, DebugColor);
                    Screen.Print($"Aspect ratio: {Camera.AspectRatio.ToString(Format)}", 1, 81, DebugColor);
                    Screen.Print($"Screen size: {Screen.Size}", 1, 97, DebugColor);
                    break;
                case DebugOutput.Validation:
                    Screen.Print($"Validation", 1, 1, DebugColor);
                    Screen.Print($"Light: {Accumulator.AccumulatedRGB.ToString(Format)}", 1, 17, DebugColor);
                    Screen.Print($"Samples: {stats.SampleCount}", 1, 33, DebugColor);
                    break;
            }
        }

        /// <summary> Handle input for the <see cref="Observer"/> </summary>
        /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
        /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
        public void HandleInput(KeyboardState keyboard, MouseState mouse) {
            if (keyboard.IsKeyPressed(Keys.F1)) DrawingMode = DrawingMode.Next();
            if (keyboard.IsKeyPressed(Keys.F2)) DebugInfo = DebugInfo.Next();
            if (keyboard.IsKeyPressed(Keys.F3)) DebugColor = 0xffffff - DebugColor;
            if (keyboard.IsKeyPressed(Keys.L)) CameraLock = !CameraLock;
            if (!CameraLock) {
                if (mouse.WasButtonDown(MouseButton.Left) && !mouse.IsButtonDown(MouseButton.Left)) {
                    Vector2 position = new((float)mouse.X / Screen.Width, (float)mouse.Y / Screen.Height);
                    IRay ray = Camera.GetCameraRay(position, position);
                    Camera.SetViewDirection(ray.Direction);
                }
                if (keyboard.IsKeyDown(Keys.Space)) Camera.Move(Camera.Up * MoveSpeed);
                if (keyboard.IsKeyDown(Keys.LeftShift)) Camera.Move(Camera.Down * MoveSpeed);
                if (keyboard.IsKeyDown(Keys.W)) Camera.Move(Camera.Front * MoveSpeed);
                if (keyboard.IsKeyDown(Keys.A)) Camera.Move(Camera.Left * MoveSpeed);
                if (keyboard.IsKeyDown(Keys.S)) Camera.Move(Camera.Back * MoveSpeed);
                if (keyboard.IsKeyDown(Keys.D)) Camera.Move(Camera.Right * MoveSpeed);
                if (keyboard.IsKeyPressed(Keys.Equal)) Camera.HorizontalFOV *= 1f + FOVSensitivity;
                if (keyboard.IsKeyPressed(Keys.Minus)) Camera.HorizontalFOV /= 1f + FOVSensitivity;
                if (keyboard.IsKeyDown(Keys.Up)) Camera.Rotate(Normal3.DefaultRight, -RotateSensitivity);
                if (keyboard.IsKeyDown(Keys.Down)) Camera.Rotate(Normal3.DefaultRight, RotateSensitivity);
                if (keyboard.IsKeyDown(Keys.Right)) Camera.Rotate(Normal3.DefaultUp, RotateSensitivity);
                if (keyboard.IsKeyDown(Keys.Left)) Camera.Rotate(Normal3.DefaultUp, -RotateSensitivity);
                if (keyboard.IsKeyDown(Keys.Q)) Camera.Rotate(Normal3.DefaultFront, RotateSensitivity);
                if (keyboard.IsKeyDown(Keys.E)) Camera.Rotate(Normal3.DefaultFront, -RotateSensitivity);
            }
        }
    }
}
