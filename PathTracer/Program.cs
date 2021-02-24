using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Drawing;

namespace PathTracer {
    public static class Program {
        static OpenTKWindow? window;
        static Renderer? renderer;

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main() {
            var gameWindowSettings = new GameWindowSettings() { IsMultiThreaded = true, RenderFrequency = 60, UpdateFrequency = 60 };
            var nativeWindowSettings = new NativeWindowSettings() { Size = new Vector2i(1280, 720), Title = "C# .NET 5 OpenTK Pathtracer" };
            window = new OpenTKWindow(gameWindowSettings, nativeWindowSettings);
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
            var scene = renderer?.Scene;
            var keyboard = window?.KeyboardState;
            if (scene != null && keyboard != null) {
                if (keyboard.IsKeyDown(Keys.F1)) scene.Camera.Config.DebugInfo = !scene.Camera.Config.DebugInfo;
                if (keyboard.IsKeyDown(Keys.F2)) scene.Camera.Config.DrawBVHTraversal = !scene.Camera.Config.DrawBVHTraversal;
                if (keyboard.IsKeyDown(Keys.Space)) scene.Camera.Move(scene.Camera.Up);
                if (keyboard.IsKeyDown(Keys.LeftShift)) scene.Camera.Move(scene.Camera.Down);
                if (keyboard.IsKeyDown(Keys.W)) scene.Camera.Move(scene.Camera.Front);
                if (keyboard.IsKeyDown(Keys.S)) scene.Camera.Move(scene.Camera.Back);
                if (keyboard.IsKeyDown(Keys.A)) scene.Camera.Move(scene.Camera.Left);
                if (keyboard.IsKeyDown(Keys.D)) scene.Camera.Move(scene.Camera.Right);
                if (keyboard.IsKeyDown(Keys.KeyPadAdd)) scene.Camera.FOV *= 1.1f;
                if (keyboard.IsKeyDown(Keys.KeyPadSubtract)) scene.Camera.FOV *= 0.9f;
                if (keyboard.IsKeyDown(Keys.Left)) scene.Camera.Turn(scene.Camera.Left);
                if (keyboard.IsKeyDown(Keys.Right)) scene.Camera.Turn(scene.Camera.Right);
                if (keyboard.IsKeyDown(Keys.Up)) scene.Camera.Turn(scene.Camera.Up);
                if (keyboard.IsKeyDown(Keys.Down)) scene.Camera.Turn(scene.Camera.Down);
            }
        }
    }
}
