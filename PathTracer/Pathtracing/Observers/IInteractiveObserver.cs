using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Utilities;

namespace PathTracer.Pathtracing.Observers;

/// <summary> An interactive <see cref="IObserver"/> </summary>
internal interface IInteractiveObserver : IObserver {
    /// <summary> The <see cref="IScreen"/> used for visual output to the <see cref="IInteractiveObserver"/> </summary>
    IScreen Screen { get; }

    /// <summary> The targeted framerate of the <see cref="IInteractiveObserver"/> </summary>
    int TargetFrameRate { get; set; }
    /// <summary> The targeted frame time of the <see cref="IInteractiveObserver"/> </summary>
    TimeSpan TargetFrameTime => new(0, 0, 0, 0, 1000 / TargetFrameRate);
    /// <summary> The <see cref="DrawingMode"/> of the <see cref="IInteractiveObserver"/> </summary>
    DrawingMode Drawing { get; set; }
    /// <summary> Which debug information is being shown to the <see cref="IInteractiveObserver"/> </summary>
    DebugOutput Debug { get; set; }
    /// <summary> The color of the debug output </summary>
    int TextColor { get; set; }

    /// <summary> Whether the <see cref="ICamera"/> is locked in place </summary>
    bool CameraLock { get; set; }
    /// <summary> The move speed of the <see cref="ICamera"/> of the <see cref="IInteractiveObserver"/> </summary>
    float MoveSpeed { get; set; }
    /// <summary> The sensitivity of turning the <see cref="ICamera"/> of the <see cref="IInteractiveObserver"/> </summary>
    float RotateSensitivity { get; set; }
    /// <summary> The sensitivity when changing the FOV of the <see cref="ICamera"/> of the <see cref="IInteractiveObserver"/> </summary>
    float FOVSensitivity { get; set; }

    /// <summary> Draw a frame to the screen of the <see cref="IInteractiveObserver"/> </summary>
    /// <param name="stats">The statistics of the <see cref="IRenderer"/></param>
    void DrawFrame(Statistics stats);

    /// <summary> Handle input for the <see cref="IInteractiveObserver"/> </summary>
    /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
    /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
    void HandleInput(KeyboardState keyboard, MouseState mouse);
}
