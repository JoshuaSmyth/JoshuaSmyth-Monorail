
using Monorail.Graphics;
using Monorail.Mathlib;
using OpenGL;

namespace Monorail.Platform
{
    public interface IPlatformGraphicsDevice
    {
        void Enable(Enable enableFlags);

        void Disable(Enable enableFlags);

        void Clear(Vector4 color, ClearBufferMask mask = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        void UseShaderProgram(int shaderId);

        void UseVertexArrayObject(int vertexArrayObjectId);

        void DrawArrays(PrimitiveType triangles, int offset, int vertexCount);

        void DrawElements(PrimitiveType primitiveType, int count, DrawElementsType elementType, int offset);

        void BindTexture(int textureId, TextureType textureType, TextureUnits textureUnit =TextureUnits.GL_TEXTURE0);

        void BindTexture2D(int textureId, TextureUnits textureUnit = TextureUnits.GL_TEXTURE0);

        void SetTextureSamplingAttribute(TextureAttributeValue attribute);
    }
}
