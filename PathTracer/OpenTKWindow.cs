using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Drawing;
using System;
using System.Drawing;

namespace PathTracer {
    /// <summary> The main class derived from an OpenTK gamewindow </summary>
    public class OpenTKWindow : GameWindow {
        /// <summary> The identifier of the gamewindow </summary>
        public static int GameWindowID { get; private set; }
        /// <summary> The gamewindow </summary>
        public static Surface GameWindow { get; private set; }

        Main main;

        Shader shader;
        int vertexBufferObject;
        int vertexArrayObject;
        int elementBufferObject;
        /// <summary> Vertices and texture coordinates for drawing the triangles with the texture </summary>
        readonly float[] vertices = {
            // Positions        // Texture Coordinates
             1.0f,  1.0f, 0.0f, 1.0f, 0.0f,
             1.0f, -1.0f, 0.0f, 1.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
            -1.0f,  1.0f, 0.0f, 0.0f, 0.0f,
        };
        /// <summary> The indices of the vertices of the texture </summary>
        readonly uint[] indices = { 
            0, 1, 3,
            1, 2, 3
        };

        public OpenTKWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        /// <summary> Called upon app init </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnLoad() {
            GL.ClearColor(Color.Black);
            shader = new Shader("shader.vert", "shader.frag");
            vertexBufferObject = GL.GenBuffer();
            elementBufferObject = GL.GenBuffer();
            vertexArrayObject = GL.GenVertexArray();
            // ..:: Initialization code (done once (unless your object frequently changes)) :: ..
            // 1. bind Vertex Array Object
            GL.BindVertexArray(vertexArrayObject);
            // 2. copy our vertices array in a buffer for OpenGL to use
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
            // 3. then set our vertex attributes pointers
            int aPositionLocation = shader.GetAttribLocation("aPosition");
            GL.VertexAttribPointer(aPositionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float) * aPositionLocation);
            GL.EnableVertexAttribArray(aPositionLocation);
            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float) * texCoordLocation);
            GL.EnableVertexAttribArray(texCoordLocation);
            
            GL.Enable(EnableCap.Texture2D);
            GL.Disable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GameWindow = new Surface();
            GameWindowID = GameWindow.GenTexture();
            //ClientSize = new Size(GameWindow.Width, GameWindow.Height);
            Location = new Vector2i(50, 80);
            main = new Main(GameWindow);
            base.OnLoad();
        }

        /// <summary> Called upon app close </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnUnload() {
            main.Scene.Camera.Config.SaveToFile();
            shader.Dispose();
            int texture = GameWindowID;
            GL.DeleteTextures(1, ref texture);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(elementBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);
            Environment.Exit(0);
            base.OnUnload();
        }

        /// <summary> Called upon window resize </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnResize(ResizeEventArgs e) {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args) {
            if (KeyboardState.IsAnyKeyDown) {
                HandleInput();
            }
            base.OnUpdateFrame(args);
        }

        void HandleInput() {
            if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
            var scene = main.Scene;
            if (KeyboardState.IsKeyDown(Keys.F1)) scene.Camera.Config.DebugInfo = !scene.Camera.Config.DebugInfo;
            if (KeyboardState.IsKeyDown(Keys.F2)) scene.Camera.Config.DrawBVHTraversal = !scene.Camera.Config.DrawBVHTraversal;
            if (KeyboardState.IsKeyDown(Keys.Space)) scene.Camera.Move(main.Scene.Camera.Up);
            if (KeyboardState.IsKeyDown(Keys.LeftShift)) scene.Camera.Move(scene.Camera.Down);
            if (KeyboardState.IsKeyDown(Keys.W)) scene.Camera.Move(scene.Camera.Front);
            if (KeyboardState.IsKeyDown(Keys.S)) scene.Camera.Move(scene.Camera.Back);
            if (KeyboardState.IsKeyDown(Keys.A)) scene.Camera.Move(scene.Camera.Left);
            if (KeyboardState.IsKeyDown(Keys.D)) scene.Camera.Move(scene.Camera.Right);
            if (KeyboardState.IsKeyDown(Keys.KeyPadAdd)) scene.Camera.FOV *= 1.1f;
            if (KeyboardState.IsKeyDown(Keys.KeyPadSubtract)) scene.Camera.FOV *= 0.9f;
            if (KeyboardState.IsKeyDown(Keys.Left)) scene.Camera.Turn(scene.Camera.Left);
            if (KeyboardState.IsKeyDown(Keys.Right)) scene.Camera.Turn(scene.Camera.Right);
            if (KeyboardState.IsKeyDown(Keys.Up)) scene.Camera.Turn(scene.Camera.Up);
            if (KeyboardState.IsKeyDown(Keys.Down)) scene.Camera.Turn(scene.Camera.Down);
        }

        /// <summary> Called once per frame; render </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnRenderFrame(FrameEventArgs e) {
            main.Tick();
            /// Clear Screen
            GL.Clear(ClearBufferMask.ColorBufferBit);
            /// Bind Texture
            GL.BindTexture(TextureTarget.Texture2D, GameWindowID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, GameWindow.Width, GameWindow.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, GameWindow.Pixels);
            /// Assign Shader
            GL.UseProgram(shader.Handle);
            /// Draw Triangles
            GL.BindVertexArray(vertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            /// Done Drawing
            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}