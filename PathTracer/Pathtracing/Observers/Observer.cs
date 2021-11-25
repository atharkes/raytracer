using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Geometry.Directions;
using PathTracer.Geometry.Positions;
using PathTracer.Pathtracing.Observers.Accumulators;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Utilities;

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
        public DrawingMode DrawingMode { get; set; } = DrawingMode.Light;
        /// <summary> Whether it is drawing debug information </summary>
        public bool DebugInfo { get; set; } = false;

        /// <summary> The speed at which the camera moves </summary>
        public float MoveSpeed { get; set; } = 0.1f;
        /// <summary> The sensitivity of turning the camera </summary>
        public float RotateSensitivity { get; set; } = 0.1f;
        /// <summary> The sensitivity when changing the FOV of the camera </summary>
        public float FOVSensitivity { get; set; } = 0.1f;

        /// <summary> Create a new <see cref="Observer"/> </summary>
        /// <param name="screen">The <see cref="IScreen"/> of the <see cref="Observer"/></param>
        /// <param name="camera">The <see cref="ICamera"/> of the <see cref="Observer"/></param>
        public Observer(IScreen screen, ICamera camera) {
            Camera = camera;
            Accumulator = new Accumulator(screen.Width, screen.Height);
            Screen = screen;
            Screen.OnResize += (_, size) => Accumulator = new Accumulator(size.X, size.Y);
            Screen.OnResize += (_, size) => Camera.AspectRatio = (float)size.X / size.Y;
            Camera.OnMoved += (_, _) => Accumulator.Clear();
            Camera.Film.SampleRegistered += (_, sample) => Accumulator.Add(sample);
        }

        /// <summary> Draw a frame to the screen of the <see cref="IObserver"/> </summary>
        public void DrawFrame(Statistics statistics) {
            Screen.Clear();
            Accumulator.DrawImage(Screen, DrawingMode);
            if (DebugInfo) DrawDebugInformation(statistics);
        }

        void DrawDebugInformation(Statistics stats) {
            Screen.Print($"FPS: {(int)(1000 / stats.FrameTime.LastTick.TotalMilliseconds)}", 1, 1);
            Screen.Print($"Light: {Accumulator.AccumulatedLight}", 1, 17);
            Screen.Print($"Samples: {stats.SampleCount}", 1, 33);
            Screen.Print($"Samples/frame: {stats.SampleCountLastTick}", 1, 49);
            Screen.Print($"Frame Time (ms): {(int)stats.FrameTime.LastTick.TotalMilliseconds}", 1, 65);
            Screen.Print($"Integrator Time (ms): {(int)stats.IntegratorTime.LastTick.TotalMilliseconds}", 1, 81);
            Screen.Print($"Drawing Time (ms): {(int)stats.DrawingTime.LastTick.TotalMilliseconds}", 1, 97);
            Screen.Print($"OpenTK Time (ms): {(int)stats.OpenTKTime.LastTick.TotalMilliseconds}", 1, 113);
            Screen.Print($"Horizontal FOV: {Camera.HorizontalFOV}", 1, 129);
        }

        /// <summary> Handle input for the <see cref="Observer"/> </summary>
        /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
        /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
        public void HandleInput(KeyboardState keyboard, MouseState mouse) {
            if (mouse.WasButtonDown(MouseButton.Left) && !mouse.IsButtonDown(MouseButton.Left)) {
                Position2 position = new((float)mouse.X / Screen.Width, (float)mouse.Y / Screen.Height);
                IRay ray = Camera.GetCameraRay(position, new Direction2(0.5f, 0.5f));
                Camera.SetViewDirection(ray.Direction);
            }
            if (keyboard.IsKeyPressed(Keys.F1)) DebugInfo = !DebugInfo;
            if (keyboard.IsKeyPressed(Keys.F2)) DrawingMode = DrawingMode.Next();
            if (keyboard.IsKeyDown(Keys.Space)) Camera.Move(Camera.Up * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.LeftShift)) Camera.Move(Camera.Down * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.W)) Camera.Move(Camera.Front * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.A)) Camera.Move(Camera.Left * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.S)) Camera.Move(Camera.Back * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.D)) Camera.Move(Camera.Right * MoveSpeed);
            if (keyboard.IsKeyPressed(Keys.KeyPadAdd)) Camera.HorizontalFOV *= 1f + FOVSensitivity;
            if (keyboard.IsKeyPressed(Keys.KeyPadSubtract)) Camera.HorizontalFOV /= 1f + FOVSensitivity;
            if (keyboard.IsKeyDown(Keys.Up)) Camera.Rotate(IDirection3.DefaultRight, -RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Down)) Camera.Rotate(IDirection3.DefaultRight, RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Right)) Camera.Rotate(IDirection3.DefaultUp, RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Left)) Camera.Rotate(IDirection3.DefaultUp, -RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Q)) Camera.Rotate(IDirection3.DefaultFront, RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.E)) Camera.Rotate(IDirection3.DefaultFront, -RotateSensitivity);
        }
    }
}
