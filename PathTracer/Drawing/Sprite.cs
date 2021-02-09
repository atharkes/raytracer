using OpenTK.Graphics.OpenGL;

namespace PathTracer.Drawing {
    /// <summary> A sprite to draw on the gamewindow </summary>
    public class Sprite {
        readonly int textureID;
        readonly Surface bitmap;

        /// <summary> Load a new sprite from a file </summary>
        /// <param name="fileName">The name of the file to load the sprite from</param>
        public Sprite(string fileName) {
            bitmap = new Surface(fileName);
            textureID = bitmap.GenTexture();
        }

        /// <summary> Draw a texture at some coordinates on the screen </summary>
        /// <param name="x">The x position to draw the sprite at</param>
        /// <param name="y">The y position to draw the sprite at</param>
        /// <param name="scale">The scale to draw the sprite at</param>
        public void Draw(float x, float y, float scale = 1.0f) {
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Begin(PrimitiveType.Quads);
            float u1 = (x * 2 - 0.5f * scale * bitmap.Width) / OpenTKWindow.GameWindow.Width - 1;
            float v1 = 1 - (y * 2 - 0.5f * scale * bitmap.Height) / OpenTKWindow.GameWindow.Height;
            float u2 = ((x + 0.5f * scale * bitmap.Width) * 2) / OpenTKWindow.GameWindow.Width - 1;
            float v2 = 1 - ((y + 0.5f * scale * bitmap.Height) * 2) / OpenTKWindow.GameWindow.Height;
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(u1, v2);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(u2, v2);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(u2, v1);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(u1, v1);
            GL.End();
            GL.Disable(EnableCap.Blend);
        }
    }
}
