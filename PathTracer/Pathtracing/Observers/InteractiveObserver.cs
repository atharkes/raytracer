using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Geometry.Normals;
using PathTracer.Geometry.Vectors;
using PathTracer.Pathtracing.Observers.Accumulators;
using PathTracer.Pathtracing.Observers.Cameras;
using PathTracer.Pathtracing.Rays;
using PathTracer.Utilities;
using PathTracer.Utilities.Extensions;

namespace PathTracer.Pathtracing.Observers;

/// <summary> The drawing mode of the accumulator </summary>
public enum DrawingMode {
    Light,
    BVHNodeTraversals,
    Intersections,
}

/// <summary> The different debug outputs </summary>
public enum DebugOutput {
    None,
    FrameTimes,
    Configuration,
    Validation,
}

/// <summary> An <see cref="IInteractiveObserver"/> that observes a <see cref="IScene"/> </summary>
public class InteractiveObserver : Observer, IInteractiveObserver {
    /// <summary> The <see cref="IScreen"/> used for visual output to the <see cref="InteractiveObserver"/> </summary>
    public IScreen Screen { get; }

    /// <summary> The targeted framerate of the raytracer </summary>
    public int TargetFrameRate { get; set; } = 20;
    /// <summary> Whether to draw the amount of BVH traversals instead of normal light </summary>
    public DrawingMode Drawing { get; set; }
    /// <summary> Whether it is drawing debug information </summary>
    public DebugOutput Debug { get; set; }
    /// <summary> The color of the debug output </summary>
    public int TextColor { get; set; } = 0xffffff;
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

    /// <summary> Create a new <see cref="InteractiveObserver"/> </summary>
    /// <param name="screen">The <see cref="IScreen"/> of the <see cref="InteractiveObserver"/></param>
    /// <param name="camera">The <see cref="ICamera"/> of the <see cref="InteractiveObserver"/></param>
    public InteractiveObserver(IScreen screen, ICamera camera) : base(camera, screen.Width, screen.Height) {
        Screen = screen;
        Screen.Resize += (e) => Accumulator = new Accumulator(e.Width, e.Height);
        Screen.Resize += (e) => Camera.AspectRatio = (float)e.Width / e.Height;
    }

    /// <summary> Draw a frame to the screen of the <see cref="IObserver"/> </summary>
    public void DrawFrame(Statistics statistics) {
        Screen.Clear();
        Accumulator.DrawImage(Screen, Drawing);
        DrawDebugInformation(statistics);
    }

    private void DrawDebugInformation(Statistics stats) {
        switch (Debug) {
            case DebugOutput.None:
                break;
            case DebugOutput.FrameTimes:
                Screen.Print($"Frame Times", 1, 1, TextColor);
                Screen.Print($"FPS: {(1000f / stats.FrameTime.LastTick.TotalMilliseconds).ToString(Format)}", 1, 17, TextColor);
                Screen.Print($"FPS (target): {TargetFrameRate}", 1, 33, TextColor);
                Screen.Print($"Samples/frame: {stats.SampleCountLastTick}", 1, 49, TextColor);
                Screen.Print($"Frametime (ms): {stats.FrameTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 65, TextColor);
                Screen.Print($"Integrator time (ms): {stats.IntegratorTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 81, TextColor);
                Screen.Print($"Drawing time (ms): {stats.DrawingTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 97, TextColor);
                Screen.Print($"OpenTK time (ms): {stats.OpenTKTime.LastTick.TotalMilliseconds.ToString(Format)}", 1, 113, TextColor);
                break;
            case DebugOutput.Configuration:
                Screen.Print($"Camera Configuration", 1, 1, TextColor);
                Screen.Print($"Lock (L): {CameraLock}", 1, 17, TextColor);
                Screen.Print($"Position: {Camera.Position.ToString(Format)}", 1, 33, TextColor);
                Screen.Print($"View direction: {Camera.ViewDirection.ToString(Format)}", 1, 49, TextColor);
                Screen.Print($"FOV: {Camera.FieldOfViewAngles.ToString(Format)}", 1, 65, TextColor);
                Screen.Print($"Aspect ratio: {Camera.AspectRatio.ToString(Format)}", 1, 81, TextColor);
                Screen.Print($"Screen size: {Screen.Size}", 1, 97, TextColor);
                break;
            case DebugOutput.Validation:
                Screen.Print($"Validation", 1, 1, TextColor);
                Screen.Print($"Light: {Accumulator.AccumulatedRGB.ToString(Format)}", 1, 17, TextColor);
                Screen.Print($"Samples: {stats.SampleCount}", 1, 33, TextColor);
                break;
        }
    }

    /// <summary> Handle input for the <see cref="InteractiveObserver"/> </summary>
    /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
    /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
    public void HandleInput(KeyboardState keyboard, MouseState mouse) {
        if (keyboard.IsKeyPressed(Keys.F1)) Drawing = Drawing.Next();
        if (keyboard.IsKeyPressed(Keys.F2)) Debug = Debug.Next();
        if (keyboard.IsKeyPressed(Keys.F3)) TextColor = 0xffffff - TextColor;
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
