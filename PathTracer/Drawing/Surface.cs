using OpenTK.Graphics.OpenGL;
using PathTracer.Raytracing.SceneObjects.CameraParts;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PathTracer.Drawing {
    /// <summary> A pixel surface to display </summary>
    public class Surface : IScreen {
        /// <summary> The width of the surface </summary>
        public int Width { get; }
        /// <summary> The height of the surface </summary>
        public int Height { get; }
        /// <summary> The pixels of the surface </summary>
        public readonly int[] Pixels;

        public static Surface Font { get; } = new Surface("../../../assets/font.png");
        static int[] fontRedir = FontRedir();
        static int[] FontRedir() {
            string ch = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";
            fontRedir = new int[256];
            for (int i = 0; i < 256; i++) fontRedir[i] = 0;
            for (int i = 0; i < ch.Length; i++) {
                int l = ch[i];
                fontRedir[l & 255] = i;
            }
            return fontRedir;
        }

        /// <summary> Create a new empty surface </summary>
        /// <param name="width">The width of the surface</param>
        /// <param name="height">The height of the surface</param>
        public Surface(int width = 1280, int height = 720) {
            Width = width;
            Height = height;
            Pixels = new int[width * height];
        }

        /// <summary> Create a new surface from a file </summary>
        /// <param name="fileName">The name of the file to create a surface of</param>
        public Surface(string fileName) {
            Bitmap bmp = new Bitmap(fileName);
            Width = bmp.Width;
            Height = bmp.Height;
            Pixels = new int[Width * Height];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, Pixels, 0, Width * Height);
            bmp.UnlockBits(data);
        }

        public int GenTexture() {
            int id = GL.GenTexture();
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
            int src = 0;
            int dst = 0;
            int srcwidth = Width;
            int srcheight = Height;
            int dstwidth = target.Width;
            int dstheight = target.Height;
            if ((srcwidth + x) > dstwidth) srcwidth = dstwidth - x;
            if ((srcheight + y) > dstheight) srcheight = dstheight - y;
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
            if ((srcwidth > 0) && (srcheight > 0)) {
                dst += x + dstwidth * y;
                for (int v = 0; v < srcheight; v++) {
                    for (int u = 0; u < srcwidth; u++) target.Pixels[dst + u] = Pixels[src + u];
                    dst += dstwidth;
                    src += Width;
                }
            }
        }

        public void Box(int x1, int y1, int x2, int y2, int c) {
            int dest = y1 * Width;
            for (int y = y1; y <= y2; y++, dest += Width) {
                Pixels[dest + x1] = c;
                Pixels[dest + x2] = c;
            }
            int dest1 = y1 * Width;
            int dest2 = y2 * Width;
            for (int x = x1; x <= x2; x++) {
                Pixels[dest1 + x] = c;
                Pixels[dest2 + x] = c;
            }
        }

        public void Bar(int x1, int y1, int x2, int y2, int c) {
            int dest = y1 * Width;
            for (int y = y1; y <= y2; y++, dest += Width) {
                for (int x = x1; x <= x2; x++) {
                    Pixels[dest + x] = c;
                }
            }
        }

        public void Line(int x1, int y1, int x2, int y2, int c) {
            if ((x1 < 0) || (y1 < 0) || (x2 < 0) || (y2 < 0) || (x1 >= Width) || (x2 >= Width) || (y1 >= Height) || (y2 >= Height)) return;
            if (Math.Abs(x2 - x1) >= Math.Abs(y2 - y1)) {
                if (x2 < x1) { int h = x1; x1 = x2; x2 = h; h = y2; y2 = y1; y1 = h; }
                int l = x2 - x1;
                if (l == 0) return;
                int dy = ((y2 - y1) * 8192) / l;
                y1 *= 8192;
                for (int i = 0; i < l; i++) {
                    Pixels[x1++ + (y1 / 8192) * Width] = c;
                    y1 += dy;
                }
            } else {
                if (y2 < y1) { int h = x1; x1 = x2; x2 = h; h = y2; y2 = y1; y1 = h; }
                int l = y2 - y1;
                if (l == 0) return;
                int dx = ((x2 - x1) * 8192) / l;
                x1 *= 8192;
                for (int i = 0; i < l; i++) {
                    Pixels[x1 / 8192 + y1++ * Width] = c;
                    x1 += dx;
                }
            }
        }

        public void Plot(int x, int y, int c) {
            if ((x >= 0) && (y >= 0) && (x < Width) && (y < Height)) Pixels[x + y * Width] = c;
        }

        public void Plot(int i, int c) {
            if (i < Pixels.Length) Pixels[i] = c;
        }

        public void Print(string t, int x, int y, int c) {
            for (int i = 0; i < t.Length; i++) {
                int f = fontRedir[t[i] & 255];
                int dest = x + i * 12 + y * Width;
                int src = f * 12;
                for (int v = 0; v < Font.Height; v++, src += Font.Width, dest += Width) {
                    for (int u = 0; u < 12; u++) {
                        if ((Font.Pixels[src + u] & 0xffffff) != 0) Pixels[dest + u] = c;
                    }
                }
            }
        }
    }
}
