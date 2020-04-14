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
        
        public static ShaderProgram CreateFromFile(string vertexShaderFileName, string fragmentShaderFileName)
        {
            using (TracedStopwatch.Start("load shader"))
            {
                // TODO:(Joshua) This should probably be moved from a static method into a method onto a new PlatformAssetLoader class
                var rv = new ShaderProgram();
                {
                    // Vertex Shader
                    var vertShader = File.ReadAllText(vertexShaderFileName);
                    var vertexShader = OpenGL.GlBindings.CreateShader(ShaderType.VertexShader);
                    GlBindings.ShaderSource(vertexShader, vertShader);
                    GlBindings.CompileShader(vertexShader);

                    int result;

                    GlBindings.GetShaderInfo(vertexShader, ShaderParameter.CompileStatus, out result);
                    if (result != (int)Result.Ok)
                    {
                        var error = GlBindings.GetShaderInfoLog(vertexShader);
                        Console.WriteLine("Shader Compilation Failed:/r/n" + error);
                    }

                    // Fragment Shader
                    var fragShader = File.ReadAllText(fragmentShaderFileName);
                    var fragmentShader = OpenGL.GlBindings.CreateShader(ShaderType.FragmentShader);
                    GlBindings.ShaderSource(fragmentShader, fragShader);
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

                    GlBindings.DeleteShader(fragmentShader);
                    GlBindings.DeleteShader(vertexShader);
                }

                return rv;
            }
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
