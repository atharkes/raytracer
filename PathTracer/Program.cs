using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PathTracer.Drawing;
using PathTracer.Multithreading;

namespace PathTracer {
    public static class Program {
        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new Threadpool();
        /// <summary> The window supplied by OpenTK to render to </summary>
        public static readonly RenderWindow Window = new RenderWindow(GameWindowSettings.Default, NativeWindowSettings.Default) {
            RenderFrequency = 0,
            UpdateFrequency = 0,
            Location = new Vector2i(40, 80),
            Size = new Vector2i(1280, 720),
            Title = "C# .NET 5 OpenTK Pathtracer",
            WindowBorder = WindowBorder.Resizable
        };
        /// <summary> The renderer to supply images </summary>
        public static readonly Renderer Renderer = new Renderer(Window.GameWindow);

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Closed += Renderer.Scene.Camera.Config.SaveToFile;
            Window.Run();
            Renderer.Scene.Camera.Config.SaveToFile();
        }

        private static void UpdateRenderer(FrameEventArgs obj) {
            Renderer.Tick();
        }

        private static void HandleInput(FrameEventArgs obj) {
            Renderer.Scene.Camera.InputHandler.HandleKeyboardInput(Window.KeyboardState);
        }
    }
}
