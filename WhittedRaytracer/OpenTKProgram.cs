using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using WhittedRaytracer.Drawing;

namespace WhittedRaytracer {
    /// <summary> The main class derived from an OpenTK gamewindow </summary>
    public class OpenTKProgram : GameWindow {
        /// <summary> The identifier of the gamewindow </summary>
        public static int GameWindowID { get; private set; }
        /// <summary> The gamewindow </summary>
        public static Surface GameWindow { get; private set; }

        static Main main;

        /// <summary> Called upon app init </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnLoad(EventArgs e) {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GameWindow = new Surface();
            GameWindowID = GameWindow.GenTexture();
            ClientSize = new Size(GameWindow.Width, GameWindow.Height);
            Location = new Point(50, 80);
            main = new Main(GameWindow);
        }

        /// <summary> Called upon app close </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnUnload(EventArgs e) {
            main.Scene.Camera.Config.SaveToFile();
            int texture = GameWindowID;
            GL.DeleteTextures(1, ref texture);
            Environment.Exit(0);
        }

        /// <summary> Called upon window resize </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnResize(EventArgs e) {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        /// <summary> Called once per frame; app logic </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnUpdateFrame(FrameEventArgs e) {
            var keyboard = OpenTK.Input.Keyboard.GetState();
            if (keyboard[OpenTK.Input.Key.Escape]) Exit();
        }

        /// <summary> Called once per frame; render </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnRenderFrame(FrameEventArgs e) {
            main.Tick();
            // Convert Game.screen to OpenGL texture
            GL.BindTexture(TextureTarget.Texture2D, GameWindowID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          GameWindow.Width, GameWindow.Height, 0, PixelFormat.Bgra,
                          PixelType.UnsignedByte, GameWindow.Pixels);
            // Clear window contents
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Setup camera
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            // Draw screen filling quad
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-1.0f, 1.0f);
            GL.End();
            // Tell OpenTK we're done rendering
            SwapBuffers();
        }

        /// <summary> Entry point of the application </summary>
        /// <param name="args">Arguments given</param>
        public static void Main(string[] args) {
            using (OpenTKProgram app = new OpenTKProgram()) { app.Run(30.0, 0.0); }
        }
    }
}