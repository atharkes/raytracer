using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;
using System.Drawing.Imaging;

namespace PathTracer.Drawing;

/// <summary> A pixel surface to display </summary>
public class Surface {
    /// <summary> The font used to write text with </summary>
    public static Surface Font { get; } = new Surface("../../../assets/font.png");

    private static int[] fontRedir = FontRedir();

    private static int[] FontRedir() {
        var ch = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";
        fontRedir = new int[256];
        for (var i = 0; i < 256; i++) fontRedir[i] = 0;
        for (var i = 0; i < ch.Length; i++) {
            int l = ch[i];
            fontRedir[l & 255] = i;
        }
        return fontRedir;
    }

    /// <summary> The size of the <see cref="Surface"/> </summary>
    public Vector2i Size {
        get => size;
        set {
            size = value;
            Pixels = new int[size.X * size.Y];
        }
    }
    /// <summary> The width of the <see cref="Surface"/> </summary>
    public int Width => Size.X;
    /// <summary> The height of the <see cref="Surface"/> </summary>
    public int Height => Size.Y;
    /// <summary> The pixel array of the <see cref="Surface"/> </summary>
    public int[] Pixels;

    private Vector2i size;

    /// <summary> Create a new <see cref="Surface"/> </summary>
    /// <param name="size">The size of the <see cref="Surface"/></param>
    public Surface(Vector2i size) {
        Size = size;
        Pixels = new int[size.X * size.Y];
    }

    /// <summary> Create a new <see cref="Surface"/> </summary>
    /// <param name="width">The width of the <see cref="Surface"/></param>
    /// <param name="height">The height of the <see cref="Surface"/></param>
    public Surface(int width, int height) {
        Size = new Vector2i(width, height);
        Pixels = new int[width * height];
    }

    /// <summary> Create a <see cref="Surface"/> from a file </summary>
    /// <param name="fileName">The name of the file to create a surface of</param>
    public Surface(string fileName) {
        Bitmap bmp = new(fileName);
        Size = new Vector2i(bmp.Width, bmp.Height);
        Pixels = new int[Width * Height];
        var data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        System.Runtime.InteropServices.Marshal.Copy(data.Scan0, Pixels, 0, Width * Height);
        bmp.UnlockBits(data);
    }

    public int GenTexture() {
        var id = GL.GenTexture();
        /// Bind Image
        GL.BindTexture(TextureTarget.Texture2D, id);
        /// Texture Wrapping Parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
        float[] borderColor = { 1.0f, 1.0f, 0.0f, 1.0f };
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
        /// Texture Filtering Parameters
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        /// Create Image
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Pixels);
        return id;
    }

    /// <summary> Clear the surface </summary>
    /// <param name="c">The color to clear the surface with</param>
    public void Clear(int c) {
        for (int s = Width * Height, p = 0; p < s; p++) Pixels[p] = c;
    }

    public void CopyTo(Surface target, int x = 0, int y = 0) {
        var src = 0;
        var dst = 0;
        var srcwidth = Width;
        var srcheight = Height;
        var dstwidth = target.Width;
        var dstheight = target.Height;
        if (srcwidth + x > dstwidth) srcwidth = dstwidth - x;
        if (srcheight + y > dstheight) srcheight = dstheight - y;
        if (x < 0) {
            src -= x;
            srcwidth += x;
            x = 0;
        }
        if (y < 0) {
            src -= y * Width;
            srcheight += y;
            y = 0;
        }
        if (srcwidth > 0 && srcheight > 0) {
            dst += x + dstwidth * y;
            for (var v = 0; v < srcheight; v++) {
                for (var u = 0; u < srcwidth; u++) target.Pixels[dst + u] = Pixels[src + u];
                dst += dstwidth;
                src += Width;
            }
        }
    }

    public void Box(int x1, int y1, int x2, int y2, int c) {
        var dest = y1 * Width;
        for (var y = y1; y <= y2; y++, dest += Width) {
            Pixels[dest + x1] = c;
            Pixels[dest + x2] = c;
        }
        var dest1 = y1 * Width;
        var dest2 = y2 * Width;
        for (var x = x1; x <= x2; x++) {
            Pixels[dest1 + x] = c;
            Pixels[dest2 + x] = c;
        }
    }

    public void Bar(int x1, int y1, int x2, int y2, int c) {
        var dest = y1 * Width;
        for (var y = y1; y <= y2; y++, dest += Width) {
            for (var x = x1; x <= x2; x++) {
                Pixels[dest + x] = c;
            }
        }
    }

    public void Line(int x1, int y1, int x2, int y2, int c) {
        if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0 || x1 >= Width || x2 >= Width || y1 >= Height || y2 >= Height) return;
        if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1)) {
            if (x2 < x1) {
                (x2, x1) = (x1, x2);
                (y1, y2) = (y2, y1);
            }
            var l = x2 - x1;
            if (l == 0) return;
            var dy = (y2 - y1) * 8192 / l;
            y1 *= 8192;
            for (var i = 0; i < l; i++) {
                Pixels[x1++ + y1 / 8192 * Width] = c;
                y1 += dy;
            }
        } else {
            if (y2 < y1) {
                (x2, x1) = (x1, x2);
                (y1, y2) = (y2, y1);
            }
            var l = y2 - y1;
            if (l == 0) return;
            var dx = (x2 - x1) * 8192 / l;
            x1 *= 8192;
            for (var i = 0; i < l; i++) {
                Pixels[x1 / 8192 + y1++ * Width] = c;
                x1 += dx;
            }
        }
    }

    public void Plot(int x, int y, int c) {
        if (x >= 0 && y >= 0 && x < Width && y < Height) Pixels[x + y * Width] = c;
    }

    public void Plot(int i, int c) {
        if (i < Pixels.Length) Pixels[i] = c;
    }

    public void Print(string t, int x, int y, int c) {
        for (var i = 0; i < t.Length; i++) {
            var f = fontRedir[t[i] & 255];
            var dest = x + i * 12 + y * Width;
            var src = f * 12;
            for (var v = 0; v < Font.Height; v++, src += Font.Width, dest += Width) {
                for (var u = 0; u < 12; u++) {
                    if ((Font.Pixels[src + u] & 0xffffff) != 0) Pixels[dest + u] = c;
                }
            }
        }
    }
}
