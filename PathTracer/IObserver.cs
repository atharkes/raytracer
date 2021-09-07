using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;

namespace PathTracer {
    public interface IObserver {
        IScreen Screen { get; }
        Camera Camera { get; }

        void HandleInput(KeyboardState keyboard, MouseState mouseState);
        void UpdateWindow();
    }
}
