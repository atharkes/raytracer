using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using PathTracer.Pathtracing.Observers;
using System.Drawing;

namespace PathTracer.Drawing;

/// <summary> The main class derived from an OpenTK gamewindow </summary>
public class RenderWindow : GameWindow, IScreen {
    /// <summary> The identifier of the gamewindow </summary>
    public int GameWindowID { get; }
    /// <summary> The gamewindow </summary>
    public Surface Surface { get; }

    private readonly Shader shader;
    private int vertexBufferObject;
    private int vertexArrayObject;
    private int elementBufferObject;
    /// <summary> Vertices and texture coordinates for drawing the triangles with the texture </summary>
    private readonly float[] vertices = {
        // Positions        // Texture Coordinates
         1.0f,  1.0f, 0.0f, 1.0f, 0.0f,
         1.0f, -1.0f, 0.0f, 1.0f, 1.0f,
        -1.0f, -1.0f, 0.0f, 0.0f, 1.0f,
        -1.0f,  1.0f, 0.0f, 0.0f, 0.0f,
    };
    /// <summary> The indices of the vertices of the texture </summary>
    private readonly uint[] indices = {
        0, 1, 3,
        1, 2, 3
    };

    public RenderWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) {
        Surface = new Surface(nativeWindowSettings.ClientSize);
        GameWindowID = Surface.GenTexture();
        shader = new Shader("Drawing\\shader.vert", "Drawing\\shader.frag");
    }

    public void Clear(int color = 0) => Surface.Clear(color);
    public void Plot(int x, int y, int color = 0xffffff) => Surface.Plot(x, y, color);
    public void Plot(int i, int color = 0xffffff) => Surface.Plot(i, color);
    public void Line(int x1, int y1, int x2, int y2, int color = 0xffffff) => Surface.Line(x1, y1, x2, y2, color);
    public void Box(int x1, int y1, int x2, int y2, int color = 0xffffff) => Surface.Box(x1, y1, x2, y2, color);
    public void Print(string text, int x, int y, int color = 0xffffff) => Surface.Print(text, x, y, color);

    /// <summary> Called upon app init </summary>
    /// <param name="e">Arguments given</param>
    protected override void OnLoad() {
        GL.ClearColor(Color.Black);
        vertexBufferObject = GL.GenBuffer();
        elementBufferObject = GL.GenBuffer();
        vertexArrayObject = GL.GenVertexArray();
        // ..:: Initialization code (done once (unless your object frequently changes)) :: ..
        // Bind Vertex Array Object
        GL.BindVertexArray(vertexArrayObject);
        // Copy our vertices array in a buffer for OpenGL to use
        GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        // Then set our vertex attributes pointers
        var aPositionLocation = shader.GetAttribLocation("aPosition");
        GL.VertexAttribPointer(aPositionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float) * aPositionLocation);
        GL.EnableVertexAttribArray(aPositionLocation);
        var texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float) * texCoordLocation);
        GL.EnableVertexAttribArray(texCoordLocation);

        GL.Enable(EnableCap.Texture2D);
        GL.Disable(EnableCap.DepthTest);
        GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
        base.OnLoad();
    }

    /// <summary> Called upon window resize </summary>
    /// <param name="e">Arguments given</param>
    protected override void OnResize(ResizeEventArgs e) {
        Surface.Size = e.Size;
        GL.Viewport(0, 0, e.Width, e.Height);
        base.OnResize(e);
    }

    protected override void OnUpdateFrame(FrameEventArgs args) {
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
        base.OnUpdateFrame(args);
    }

    /// <summary> Called once per frame; render </summary>
    /// <param name="args">Arguments given</param>
    protected override void OnRenderFrame(FrameEventArgs args) {
        /// Clear Screen
        GL.Clear(ClearBufferMask.ColorBufferBit);
        /// Bind Texture
        GL.BindTexture(TextureTarget.Texture2D, GameWindowID);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Surface.Width, Surface.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, Surface.Pixels);
        /// Assign Shader
        GL.UseProgram(shader.Handle);
        /// Draw Triangles
        GL.BindVertexArray(vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        /// Done Drawing
        SwapBuffers();
        base.OnRenderFrame(args);
    }

    /// <summary> Dispose the <see cref="RenderWindow"/> </summary>
    /// <param name="disposing">Whether to recursivly dispose managed resources</param>
    protected override void Dispose(bool disposing) {
        shader.Dispose();
        var texture = GameWindowID;
        GL.DeleteTextures(1, ref texture);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(vertexBufferObject);
        GL.DeleteBuffer(elementBufferObject);
        GL.DeleteVertexArray(vertexArrayObject);
        base.Dispose(disposing);
    }
}