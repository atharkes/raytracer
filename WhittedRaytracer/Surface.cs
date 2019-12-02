using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using WhittedRaytracer.Raytracing.SceneObjects;

namespace WhittedRaytracer {
    public class Sprite {
        static public Surface Target;
        
        readonly int textureID;
        Surface bitmap;

        public Sprite(string fileName) {
            bitmap = new Surface(fileName);
            textureID = bitmap.GenTexture();
        }

        public void Draw(float x, float y, float scale = 1.0f) {
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Begin(PrimitiveType.Quads);
            float u1 = (x * 2 - 0.5f * scale * bitmap.Width) / Target.Width - 1;
            float v1 = 1 - (y * 2 - 0.5f * scale * bitmap.Height) / Target.Height;
            float u2 = ((x + 0.5f * scale * bitmap.Width) * 2) / Target.Width - 1;
            float v2 = 1 - ((y + 0.5f * scale * bitmap.Height) * 2) / Target.Height;
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(u1, v2);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(u2, v2);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(u2, v1);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(u1, v1);
            GL.End();
            GL.Disable(EnableCap.Blend);
        }
    }

    public class Surface : IScreen {
        public int Width { get; }
        public int Height { get; }
        public int[] Pixels;

        static bool fontReady = false;
        static Surface font;
        static int[] fontRedir;

        public Surface(int width = 512, int height = 512) {
            Width = width;
            Height = height;
            Pixels = new int[width * height];
        }

        public Surface(string fileName) {
            Bitmap bmp = new Bitmap(fileName);
            Width = bmp.Width;
            Height = bmp.Height;
            Pixels = new int[Width * Height];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            IntPtr ptr = data.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, Pixels, 0, Width * Height);
            bmp.UnlockBits(data);
        }

        public int GenTexture() {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Pixels);
            return id;
        }

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

        public void Print(string t, int x, int y, int c) {
            if (!fontReady) {
                font = new Surface("../../assets/font.png");
                string ch = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";
                fontRedir = new int[256];
                for (int i = 0; i < 256; i++) fontRedir[i] = 0;
                for (int i = 0; i < ch.Length; i++) {
                    int l = ch[i];
                    fontRedir[l & 255] = i;
                }
                fontReady = true;
            }
            for (int i = 0; i < t.Length; i++) {
                int f = fontRedir[t[i] & 255];
                int dest = x + i * 12 + y * Width;
                int src = f * 12;
                for (int v = 0; v < font.Height; v++, src += font.Width, dest += Width) {
                    for (int u = 0; u < 12; u++) {
                        if ((font.Pixels[src + u] & 0xffffff) != 0) Pixels[dest + u] = c;
                    }
                }
            }
        }

        
    }
}
