using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using PathTracer.Drawing;
using PathTracer.Integrators;
using PathTracer.Multithreading;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Aggregates;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using PathTracer.Utilities;

namespace PathTracer {
    public static class Program {
        /// <summary> The threadpool of this application </summary>
        public static readonly Threadpool Threadpool = new();
        /// <summary> The configuration of the renderer </summary>
        public static readonly Config Config = Config.LoadFromFile();
        /// <summary> The window supplied by OpenTK to render to </summary>
        public static readonly RenderWindow Window = new(GameWindowSettings.Default, NativeWindowSettings.Default) {
            RenderFrequency = 0,
            UpdateFrequency = 0,
            Location = Config.WindowPosition,
            Size = Config.WindowSize,
            Title = "C# .NET 5 OpenTK Pathtracer",
            WindowBorder = WindowBorder.Resizable
        };
        /// <summary> The scene to render </summary>
        public static readonly Scene Scene = Scene.Default(Window.GameWindow);
        /// <summary> The <see cref="IIntegrator"/> to integrate the scene </summary>
        public static readonly IIntegrator Integrator = new BackwardsSampler();
        /// <summary> The <see cref="IObserver"/> viewing the scene </summary>
        public static readonly IObserver Observer = new Observer(Window.GameWindow, new Camera(Config.Position, Config.Rotation, Config.AspectRatio, Config.FOV)) {
            DrawingMode = Config.DrawingMode,
            DebugInfo = Config.DebugInfo
        };
        /// <summary> The <see cref="IRenderer"/> to supply images </summary>
        public static readonly IRenderer Renderer = new Renderer(Scene, Integrator, Observer);

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            Window.UpdateFrame += HandleInput;
            Window.RenderFrame += UpdateRenderer;
            Window.Run();
            Config.SaveToFile(Renderer);
        }

        static void UpdateRenderer(FrameEventArgs obj) {
            Renderer.Render(Renderer.Observer.TargetFrameTime);
        }

        static void HandleInput(FrameEventArgs obj) {
            Renderer.Observer.HandleInput(Window.KeyboardState, Window.MouseState);
        }
    }
}
