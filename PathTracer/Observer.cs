using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.CameraParts;
using PathTracer.Pathtracing.SceneDescription.SceneObjects.Primitives;
using System;

namespace PathTracer {
    public class Observer : IObserver {
        public IScreen Screen { get; }

        public Camera Camera { get; }

        public Observer(IScreen screen, Camera camera) {
            Screen = screen;
            Camera = camera;
        }

        public void UpdateWindow() {
            throw new NotImplementedException();
        }

        public void HandleInput(KeyboardState keyboard, MouseState mouseState) {
            if (keyboard.IsKeyPressed(Keys.F1)) Camera.Config.DebugInfo = !Camera.Config.DebugInfo;
            if (keyboard.IsKeyPressed(Keys.F2)) Camera.Config.DrawingMode = Camera.Config.DrawingMode.Next();
            if (keyboard.IsKeyDown(Keys.Space)) Camera.Move(Camera.Up);
            if (keyboard.IsKeyDown(Keys.LeftShift)) Camera.Move(Camera.Down);
            if (keyboard.IsKeyDown(Keys.W)) Camera.Move(Camera.Front);
            if (keyboard.IsKeyDown(Keys.S)) Camera.Move(Camera.Back);
            if (keyboard.IsKeyDown(Keys.A)) Camera.Move(Camera.Left);
            if (keyboard.IsKeyDown(Keys.D)) Camera.Move(Camera.Right);
            if (keyboard.IsKeyPressed(Keys.KeyPadAdd)) Camera.FOV *= 1.1f;
            if (keyboard.IsKeyPressed(Keys.KeyPadSubtract)) Camera.FOV *= 0.9f;
            if (keyboard.IsKeyDown(Keys.Left)) Camera.Turn(Camera.Left);
            if (keyboard.IsKeyDown(Keys.Right)) Camera.Turn(Camera.Right);
            if (keyboard.IsKeyDown(Keys.Up)) Camera.Turn(Camera.Up);
            if (keyboard.IsKeyDown(Keys.Down)) Camera.Turn(Camera.Down);
        }

        
    }
}
