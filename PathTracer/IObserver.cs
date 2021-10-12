using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Cameras.Parts;
using System;

namespace PathTracer {
    /// <summary> An observer of a <see cref="IScene"/> </summary>
    public interface IObserver {
        /// <summary> The gamewindow of the <see cref="IObserver"/> </summary>
        IScreen Screen { get; }
        /// <summary> The virtual camera object of the <see cref="IObserver"/> </summary>
        Camera Camera { get; }

        /// <summary> The targeted framerate of the <see cref="IObserver"/> </summary>
        int TargetFrameRate { get; set; }
        /// <summary> The targeted frame time of the <see cref="IObserver"/> </summary>
        TimeSpan TargetFrameTime => new(0, 0, 0, 0, 1000 / TargetFrameRate);
        /// <summary> The <see cref="DrawingMode"/> of the <see cref="IObserver"/> </summary>
        DrawingMode DrawingMode { get; set; }
        /// <summary> Whether the debug info is being drawn for the <see cref="IObserver"/> </summary>
        bool DebugInfo { get; set; }

        /// <summary> The move speed of the camera of the <see cref="IObserver"/> </summary>
        float MoveSpeed { get; set; }
        /// <summary> The sensitivity of turning the camera of the <see cref="IObserver"/> </summary>
        float RotateSensitivity { get; set; }
        /// <summary> The sensitivity when changing the FOV of the camera of the <see cref="IObserver"/> </summary>
        float FOVSensitivity { get; set; }
        
        /// <summary> Draw a frame to the screen of the <see cref="IObserver"/> </summary>
        void DrawFrame();

        /// <summary> Handle input for the <see cref="IObserver"/> </summary>
        /// <param name="keyboard">The <see cref="KeyboardState"/> to handle input from</param>
        /// <param name="mouse">The <see cref="MouseState"/> to handle input from</param>
        void HandleInput(KeyboardState keyboard, MouseState mouse);
    }
}
