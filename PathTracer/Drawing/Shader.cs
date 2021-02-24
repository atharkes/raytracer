using OpenTK.Graphics.ES30;
using System;
using System.IO;
using System.Text;

namespace PathTracer.Drawing {
    public class Shader : IDisposable {
        public readonly int Handle;
        bool disposedValue = false;

        public Shader(string vertexPath, string fragmentPath) {
            /// Create Vertex Shader
            string VertexShaderSource;
            using (StreamReader reader = new StreamReader(vertexPath, Encoding.UTF8)) {
                VertexShaderSource = reader.ReadToEnd();
            }
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            string infoLogVert = GL.GetShaderInfoLog(VertexShader);
            if (infoLogVert != string.Empty) {
                System.Console.WriteLine(infoLogVert);
            }
            /// Create Fragment Shader
            string FragmentShaderSource;
            using (StreamReader reader = new StreamReader(fragmentPath, Encoding.UTF8)) {
                FragmentShaderSource = reader.ReadToEnd();
            }
            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(FragmentShader);
            if (infoLogFrag != string.Empty) {
                System.Console.WriteLine(infoLogFrag);
            }
            /// Link Shaders
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
            if (!disposedValue) {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        ~Shader() {
            GL.DeleteProgram(Handle);
        }
    }
}
