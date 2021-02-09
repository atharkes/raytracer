using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace PathTracer {
    public static class Program {
        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            var gameWindowSettings = new GameWindowSettings() { IsMultiThreaded = true, RenderFrequency = 60, UpdateFrequency = 60 };
            var nativeWindowSettings = new NativeWindowSettings() { Size = new Vector2i(1280, 720), Title = "C# .NET 5 OpenTK Pathtracer" };
            using OpenTKWindow app = new OpenTKWindow(gameWindowSettings, nativeWindowSettings);
            app.Run();
        }
    }
}
