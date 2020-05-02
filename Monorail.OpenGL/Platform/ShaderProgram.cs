using OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Monorail.Mathlib;

namespace Monorail.Platform
{
    public class ShaderProgram
    {
        public int ShaderProgramId { get;  private set; }
        
        public string VertexShaderFileName { get; private set; }

        public string FragmentShaderFileName { get; private set; }

        public string VertexShaderCode { get; private set; }

        public string FragmentShaderCode { get; private set; }

        // TODO Store Shader Code


        internal static ShaderProgram CreateFromString(string vertexShaderCode, string fragmentShaderCode)
        {
            using (TracedStopwatch.Start("load shader from string: "))
            {
                var rv = new ShaderProgram
                {
                    VertexShaderFileName = "Created From String",
                    FragmentShaderFileName = "Created From String",
                    VertexShaderCode = vertexShaderCode,
                    FragmentShaderCode = fragmentShaderCode
                };

                var vertexShader = OpenGL.GlBindings.CreateShader(ShaderType.VertexShader);
                GlBindings.ShaderSource(vertexShader, vertexShaderCode);
                GlBindings.CompileShader(vertexShader);

                int result;

                GlBindings.GetShaderInfo(vertexShader, ShaderParameter.CompileStatus, out result);
                if (result != (int)Result.Ok)
                {
                    var error = GlBindings.GetShaderInfoLog(vertexShader);
                    Console.WriteLine("Shader Compilation Failed:/r/n" + error);
                }

                // Fragment Shader

                var fragmentShader = OpenGL.GlBindings.CreateShader(ShaderType.FragmentShader);
                GlBindings.ShaderSource(fragmentShader, fragmentShaderCode);
                GlBindings.CompileShader(fragmentShader);
                GlBindings.GetShaderInfo(fragmentShader, ShaderParameter.CompileStatus, out result);
                if (result != (int)Result.Ok)
                {
                    var error = OpenGL.GlBindings.GetShaderInfoLog(fragmentShader);
                    Console.WriteLine("Shader Compilation Failed:/r/n" + error);
                }



                // Link Shaders
                rv.ShaderProgramId = OpenGL.GlBindings.CreateProgram();
                GlBindings.AttachShader(rv.ShaderProgramId, vertexShader);
                GlBindings.AttachShader(rv.ShaderProgramId, fragmentShader);
                GlBindings.LinkProgram(rv.ShaderProgramId);

                GlBindings.GetProgramInfo(rv.ShaderProgramId, ProgramParameter.LinkStatus, out result);
                if (result != (int)Result.Ok)
                {
                    var error = OpenGL.GlBindings.GetProgramInfoLog(rv.ShaderProgramId);
                    Console.WriteLine("Shader Link Failed:/r/n" + error);
                }

                GlBindings.GetProgramInfo(rv.ShaderProgramId, ProgramParameter.GL_ACTIVE_UNIFORMS, out result);
                //if (result != (int)Result.Ok)
                {
                    Console.WriteLine("Active Uniform Count:" + (int)result);
                }

                for (var i = 0; i < result; i++)
                {
                    var uniformInfo = GlBindings.GetActiveUniformInfo(rv.ShaderProgramId, i);
                    Console.WriteLine("Active Uniform: " + uniformInfo);

                }

                /*
                 * glGetProgramiv(program, GL_ACTIVE_UNIFORMS, &count);
                    printf("Active Uniforms: %d\n", count);

                 */

                GlBindings.DeleteShader(fragmentShader);
                GlBindings.DeleteShader(vertexShader);

                return rv;
            }
        }

        internal static ShaderProgram CreateFromFile(string vertexShaderFileName, string fragmentShaderFileName)
        {
            using (TracedStopwatch.Start("load shader from file: "))
            {
                Console.WriteLine("VERT:" + vertexShaderFileName);
                Console.WriteLine("FRAG:" + fragmentShaderFileName);

                // Vertex Shader
                var vertShader = File.ReadAllText(vertexShaderFileName);
                var fragShader = File.ReadAllText(fragmentShaderFileName);

                var rv = CreateFromString(vertShader, fragShader);

                rv.VertexShaderFileName = vertexShaderFileName;
                rv.FragmentShaderFileName = fragmentShaderFileName;
                return rv;
            }
        }

        public void SetUniform(string uniformParameterName, Vector3 value)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(ShaderProgramId, uniformParameterName);
            GlBindings.Uniform3f(location, value.X, value.Y, value.Z);
        }

        public void SetUniform(string uniformParameterName, int value)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(ShaderProgramId, uniformParameterName);
            GlBindings.Uniform1i(location, value);
        }

        public void SetUniform(string uniformParameterName, Matrix4 transform)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(ShaderProgramId, uniformParameterName);
            GlBindings.UniformMatrix4fv(location, 1, 0, transform);
        }
    }
}
