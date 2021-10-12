using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras.Parts;
using PathTracer.Utilities;

namespace PathTracer {
    /// <summary> An <see cref="IObserver"/> that observes a <see cref="IScene"/> </summary>
    public class Observer : IObserver {
        /// <summary> The <see cref="IScreen"/> used for giving visual output </summary>
        public IScreen Screen { get; }
        /// <summary> The virtual <see cref="Camera"/> object of the <see cref="Observer"/> </summary>
        public Camera Camera { get; }

        /// <summary> The targeted framerate of the raytracer </summary>
        public int TargetFrameRate { get; set; } = 30;
        /// <summary> Whether to draw the amount of BVH traversals instead of normal light </summary>
        public DrawingMode DrawingMode { get; set; } = DrawingMode.Light;
        /// <summary> Whether it is drawing debug information </summary>
        public bool DebugInfo { get; set; } = false;

        /// <summary> The speed at which the camera moves </summary>
        public float MoveSpeed { get; set; } = 0.1f;
        /// <summary> The sensitivity of turning the camera </summary>
        public float RotateSensitivity { get; set; } = 5f;
        /// <summary> The sensitivity when changing the FOV of the camera </summary>
        public float FOVSensitivity { get; set; } = 0.1f;

        /// <summary> Create a new <see cref="Observer"/> </summary>
        /// <param name="screen">The <see cref="IScreen"/> of the <see cref="Observer"/></param>
        /// <param name="camera">The camera of the <see cref="Observer"/></param>
        public Observer(IScreen screen, Camera camera) {
            Screen = screen;
            Camera = camera;
        }

        /// <summary> Draw a frame to the screen of the <see cref="IObserver"/> </summary>
        public void DrawFrame() {
            Screen.Clear();
            Camera.ScreenPlane.Accumulator.DrawImage(Screen, DrawingMode);
            if (DebugInfo) DrawDebugInformation();
        }

        void DrawDebugInformation() {
            Screen.Print($"FPS: {1000 / (int)Renderer.Statistics.FrameTime.LastTick.TotalMilliseconds}", 1, 1);
            Screen.Print($"Light: {Camera.ScreenPlane.Accumulator.AverageLight()}", 1, 17);
            Screen.Print($"Frame Time (ms): {(int)Renderer.Statistics.FrameTime.LastTick.TotalMilliseconds}", 1, 33);
            Screen.Print($"Integrator Time (ms): {(int)Renderer.Statistics.IntegratorTime.LastTick.TotalMilliseconds}", 1, 49);
            Screen.Print($"Drawing Time (ms): {(int)Renderer.Statistics.DrawingTime.LastTick.TotalMilliseconds}", 1, 65);
            Screen.Print($"OpenTK Time (ms): {(int)Renderer.Statistics.OpenTKTime.LastTick.TotalMilliseconds}", 1, 81);
            Screen.Print($"FOV: {Camera.FOV}", 1, 97);
        }

        /// <summary> Handle input for the <see cref="Observer"/> </summary>
        /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
        /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
        public void HandleInput(KeyboardState keyboard, MouseState mouse) {
            if (keyboard.IsKeyPressed(Keys.F1)) DebugInfo = !DebugInfo;
            if (keyboard.IsKeyPressed(Keys.F2)) DrawingMode = DrawingMode.Next();
            if (keyboard.IsKeyDown(Keys.Space)) Camera.Move(Camera.Up * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.LeftShift)) Camera.Move(Camera.Down * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.W)) Camera.Move(Camera.Front * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.A)) Camera.Move(Camera.Left * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.S)) Camera.Move(Camera.Back * MoveSpeed);
            if (keyboard.IsKeyDown(Keys.D)) Camera.Move(Camera.Right * MoveSpeed);
            if (keyboard.IsKeyPressed(Keys.KeyPadAdd)) Camera.FOV *= 1f + FOVSensitivity;
            if (keyboard.IsKeyPressed(Keys.KeyPadSubtract)) Camera.FOV /= 1f + FOVSensitivity;
            if (keyboard.IsKeyDown(Keys.Up)) Camera.Rotate(Camera.Left, -RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Down)) Camera.Rotate(Camera.Left, RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Right)) Camera.Rotate(Camera.Up, -RotateSensitivity);
            if (keyboard.IsKeyDown(Keys.Left)) Camera.Rotate(Camera.Up, RotateSensitivity);
        }
    }
}
