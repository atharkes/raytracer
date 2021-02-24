using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Drawing;
using PathTracer.Multithreading;

namespace PathTracer {
    public static class Program {
        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new Threadpool();
        /// <summary> The window supplied by OpenTK to render to </summary>
        static RenderWindow? window;
        /// <summary> The renderer to supply images </summary>
        static Renderer? renderer;

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            var gameWindowSettings = new GameWindowSettings() { IsMultiThreaded = true, RenderFrequency = 60, UpdateFrequency = 60 };
            var nativeWindowSettings = new NativeWindowSettings() { Size = new Vector2i(1280, 720), Title = "C# .NET 5 OpenTK Pathtracer" };
            window = new RenderWindow(gameWindowSettings, nativeWindowSettings);
            renderer = new Renderer(window.GameWindow);
            window.UpdateFrame += HandleInput;
            window.RenderFrame += UpdateRenderer;
            window.Unload += renderer.Scene.Camera.Config.SaveToFile;
            window.Run();
        }

        private static void UpdateRenderer(FrameEventArgs obj) {
            renderer?.Tick();
        }

        private static void HandleInput(FrameEventArgs obj) {
            if (window != null && renderer != null) {
                renderer.Scene.Camera.InputHandler.HandleKeyboardInput(window.KeyboardState);
            }
        }
    }
}
