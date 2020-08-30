
using Monorail.Mathlib;
using Monorail.Platform;
using OpenGL;

namespace Monorail.Graphics
{
    public class RenderQueue
    {
        IPlatformGraphicsDevice m_Graphics;
        public RenderQueue(IPlatformGraphicsDevice graphicsDevice)
        {
            m_Graphics = graphicsDevice;
        }

        public void Render(RenderableObject renderObject, GameCamera camera)
        {
            // TODO this should actually enqueue rather than render but hey!
            if (renderObject.IsWireframe)
            {
                m_Graphics.FillMode(OpenGL.Mode.GL_LINE);
            }

            if (renderObject.DepthBufferEnabled == false)
            {
                m_Graphics.Disable(OpenGL.Enable.GL_DEPTH_TEST);
            }

            // TODO This texture setting needs improving
            if (renderObject.CubemapTextureId > -1)
            {
                m_Graphics.BindTexture(renderObject.CubemapTextureId, OpenGL.TextureType.GL_TEXTURE_CUBE_MAP, OpenGL.TextureUnits.GL_TEXTURE0);
            }

            if (renderObject.TextureIdA > -1)
            {
                m_Graphics.BindTexture(renderObject.TextureIdA, OpenGL.TextureType.GL_TEXTURE_2D, OpenGL.TextureUnits.GL_TEXTURE0);
            }

            if (renderObject.TextureIdB > -1)
            {
                m_Graphics.BindTexture(renderObject.TextureIdB, OpenGL.TextureType.GL_TEXTURE_2D, OpenGL.TextureUnits.GL_TEXTURE1);
            }


            m_Graphics.BindShaderProgram(renderObject.ShaderId);

            renderObject.OnApplyUniforms(this, camera);

            if (renderObject.BlendMode == BlendModes.Alpha)
            {
                m_Graphics.Enable(OpenGL.Enable.GL_BLEND);
                m_Graphics.BlendFunc(OpenGL.BlendFunc.GL_SRC_ALPHA, OpenGL.BlendFunc.GL_ONE_MINUS_SRC_ALPHA);


            }

            m_Graphics.BindVertexArrayObject(renderObject.VaoId);

            if (renderObject.HasIndexBuffer)
            {
                m_Graphics.DrawElements(PrimitiveType.TriangleList, renderObject.VertexCount, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                m_Graphics.DrawArrays(PrimitiveType.TriangleList, 0, renderObject.VertexCount);
            }

            if (renderObject.DepthBufferEnabled == false)
            {
                m_Graphics.Enable(OpenGL.Enable.GL_DEPTH_TEST);
            }

            if (renderObject.IsWireframe)
            {
                m_Graphics.FillMode(OpenGL.Mode.GL_FILL);
            }

            if (renderObject.BlendMode == BlendModes.Alpha)
            {
                m_Graphics.Disable(OpenGL.Enable.GL_BLEND);
            }
        }

        public void SetUniform(int shaderId, string uniformParameterName, Vector3 value)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(shaderId, uniformParameterName);
            GlBindings.Uniform3f(location, value.X, value.Y, value.Z);
        }

        public void SetUniform(int shaderId, string uniformParameterName, int value)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(shaderId, uniformParameterName);
            GlBindings.Uniform1i(location, value);
        }

        public void SetUniform(int shaderId, string uniformParameterName, Matrix4 transform)
        {
            // TODO Cache location
            var location = GlBindings.GetUniformLocation(shaderId, uniformParameterName);
            GlBindings.UniformMatrix4fv(location, 1, 0, transform);
        }
    }
}
