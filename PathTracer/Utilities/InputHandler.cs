using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Raytracing.SceneObjects;

namespace PathTracer.Utilities {
    public class InputHandler {
        /// <summary> The camera to handle input for </summary>
        public Camera Camera { get; }

        /// <summary> Create a new <see cref="InputHandler"/> for a <paramref name="camera"/> </summary>
        /// <param name="camera">The <see cref="Camera"/> to handle input for</param>
        public InputHandler(Camera camera) {
            Camera = camera;
        }

        /// <summary> Handle <paramref name="keyboard"/> input for a camera </summary>
        /// <param name="keyboard">The <see cref="KeyboardState"/> to read the input from</param>
        public void HandleKeyboardInput(KeyboardState keyboard) {
            if (keyboard.IsKeyDown(Keys.F1)) Camera.Config.DebugInfo = !Camera.Config.DebugInfo;
            if (keyboard.IsKeyDown(Keys.F2)) Camera.Config.DrawBVHTraversal = !Camera.Config.DrawBVHTraversal;
            if (keyboard.IsKeyDown(Keys.Space)) Camera.Move(Camera.Up);
            if (keyboard.IsKeyDown(Keys.LeftShift)) Camera.Move(Camera.Down);
            if (keyboard.IsKeyDown(Keys.W)) Camera.Move(Camera.Front);
            if (keyboard.IsKeyDown(Keys.S)) Camera.Move(Camera.Back);
            if (keyboard.IsKeyDown(Keys.A)) Camera.Move(Camera.Left);
            if (keyboard.IsKeyDown(Keys.D)) Camera.Move(Camera.Right);
            if (keyboard.IsKeyDown(Keys.KeyPadAdd)) Camera.FOV *= 1.1f;
            if (keyboard.IsKeyDown(Keys.KeyPadSubtract)) Camera.FOV *= 0.9f;
            if (keyboard.IsKeyDown(Keys.Left)) Camera.Turn(Camera.Left);
            if (keyboard.IsKeyDown(Keys.Right)) Camera.Turn(Camera.Right);
            if (keyboard.IsKeyDown(Keys.Up)) Camera.Turn(Camera.Up);
            if (keyboard.IsKeyDown(Keys.Down)) Camera.Turn(Camera.Down);
        }
    }
}
