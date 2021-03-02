using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Drawing;

namespace PathTracer.Drawing {
    /// <summary> The main class derived from an OpenTK gamewindow </summary>
    public class RenderWindow : GameWindow {
        /// <summary> The identifier of the gamewindow </summary>
        public int GameWindowID { get; }
        /// <summary> The gamewindow </summary>
        public Surface GameWindow { get; }

        readonly Shader shader;
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

        public RenderWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
            GameWindow = new Surface();
            GameWindowID = GameWindow.GenTexture();
            shader = new Shader("Drawing\\shader.vert", "Drawing\\shader.frag");
        }

        /// <summary> Called upon app init </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnLoad() {
            GL.ClearColor(Color.Black);
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
            base.OnLoad();
        }

        /// <summary> Called upon app close </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnUnload() {
            shader.Dispose();
            int texture = GameWindowID;
            GL.DeleteTextures(1, ref texture);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(vertexBufferObject);
            GL.DeleteBuffer(elementBufferObject);
            GL.DeleteVertexArray(vertexArrayObject);
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
        }

        /// <summary> Called once per frame; render </summary>
        /// <param name="e">Arguments given</param>
        protected override void OnRenderFrame(FrameEventArgs e) {
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