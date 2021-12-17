using OpenTK.Graphics.ES30;
using System;
using System.IO;
using System.Text;

namespace PathTracer.Drawing {
    public class Shader : IDisposable {
        public readonly int Handle;
        bool disposed = false;

        public Shader(string vertexPath, string fragmentPath) {
            /// Create Vertex Shader
            string VertexShaderSource;
            using (StreamReader reader = new(vertexPath, Encoding.UTF8)) {
                VertexShaderSource = reader.ReadToEnd();
            }
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != string.Empty) {
                Console.WriteLine(infoLogVert);
            }
            /// Create Fragment Shader
            string FragmentShaderSource;
            using (StreamReader reader = new(fragmentPath, Encoding.UTF8)) {
                FragmentShaderSource = reader.ReadToEnd();
            }
            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != string.Empty) {
                Console.WriteLine(infoLogFrag);
            }
            /// Create Program
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);
            GL.LinkProgram(Handle);
            /// Cleanup
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public int GetAttribLocation(string attribName) {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void Use() {
            GL.UseProgram(Handle);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                GL.DeleteProgram(Handle);
                disposed = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        ~Shader() {
            Dispose(disposing: false);
        }
    }
}
