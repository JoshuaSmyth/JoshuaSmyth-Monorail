using Monorail.Graphics;
using OpenGL;
using System;

namespace Monorail.Platform
{
    public class OpenGLGraphicsDevice : IPlatformGraphicsDevice
    {
        IntPtr OpenGLContext;
        GraphicsCapabilities capabilities;

        public OpenGLGraphicsDevice(IntPtr openGLContext)
        {
            OpenGLContext = openGLContext;
            capabilities = new GraphicsCapabilities();
        }

        public void Clear(Mathlib.Vector4 color, ClearBufferMask mask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit)
        {
            GlBindings.ClearColor(color.X, color.Y, color.Z, color.W);
            GlBindings.Clear(mask);
        }

        public void BindShaderProgram(int shaderId)
        {
            GlBindings.UseProgram(shaderId);
        }

        public void BindVertexArrayObject(int vertexArrayObjectId)
        {
            GlBindings.BindVertexArray(vertexArrayObjectId);
        }

        public void DrawArrays(PrimitiveType primitiveType, int offset, int vertexCount)
        {
            GlBindings.DrawArrays(primitiveType, offset, vertexCount);
        }

        public void DrawElements(PrimitiveType primitiveType, int count, DrawElementsType elementType, int offset)
        {
            GlBindings.DrawElements(primitiveType, count, elementType, offset);
        }

        public void BindTexture(int textureId, TextureType textureType, TextureUnits textureUnit=TextureUnits.GL_TEXTURE0)
        {
            GlBindings.ActiveTexture(textureUnit);
            GlBindings.BindTexture(textureType, textureId);
        }


        public void BindTexture2D(int textureId, TextureUnits textureUnit = TextureUnits.GL_TEXTURE0)
        {
            GlBindings.ActiveTexture(textureUnit);
            GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, textureId);
        }

        public void Enable(Enable enableFlags)
        {
            GlBindings.Enable(enableFlags);
        }

        public void Disable(Enable enableFlags)
        {
            GlBindings.Disable(enableFlags);
        }

        public void SetTextureSamplingAttribute(TextureAttributeValue attribute)
        {
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, attribute);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, attribute);
        }

        public void DepthFunc(DepthFunc depthFunc)
        {
            GlBindings.DepthFunc(depthFunc);
        }

        public void BlendFunc(BlendFunc srcBlend, BlendFunc dstBlend)
        {
            GlBindings.BlendFunc(srcBlend, dstBlend);
        }

        public void FillMode(Mode mode)
        {
            GlBindings.PolygonMode(Face.GL_FRONT_AND_BACK, mode);
        }

        public void SetViewport(int x, int y, uint width, uint height)
        {
            GlBindings.glViewport(x, y, width, height);
        }
    }
}
