using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PathTracer.Drawing;
using PathTracer.Integrators;
using PathTracer.Multithreading;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;

namespace PathTracer {
    public static class Program {
       
        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new();
        /// <summary> The window supplied by OpenTK to render to </summary>
        public static readonly RenderWindow Window = new(GameWindowSettings.Default, NativeWindowSettings.Default) {
            RenderFrequency = 0,
            UpdateFrequency = 0,
            Location = new Vector2i(40, 80),
            Size = new Vector2i(1280, 720),
            Title = "C# .NET 5 OpenTK Pathtracer",
            WindowBorder = WindowBorder.Resizable
        };
        /// <summary> The scene to render </summary>
        public static Scene Scene { get; } = Scene.Default(Window.GameWindow);
        /// <summary> The renderer to supply images </summary>
        public static IRenderer Renderer { get; } = new Renderer(Scene, new BackwardsSampler(), new Observer(Window.GameWindow, new Camera(Window.GameWindow)));

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Run();
        }

        static void UpdateRenderer(FrameEventArgs obj) {
            Renderer.Render(Renderer.Observer.TargetFrameTime);
        }

        static void HandleInput(FrameEventArgs obj) {
            Renderer.Observer.HandleInput(Window.KeyboardState, Window.MouseState);
        }
    }
}
