
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

        void BindShaderProgram(int shaderId);

        void BindVertexArrayObject(int vertexArrayObjectId);

        void DrawArrays(PrimitiveType triangles, int offset, int vertexCount);

        void DrawElements(PrimitiveType primitiveType, int count, DrawElementsType elementType, int offset);

        void BindTexture(uint textureId, TextureType textureType, TextureUnits textureUnit =TextureUnits.GL_TEXTURE0);

        void BindTexture2D(uint textureId, TextureUnits textureUnit = TextureUnits.GL_TEXTURE0);

        void SetTextureSamplingAttribute(TextureAttributeValue attribute);

        void DepthFunc(DepthFunc depthFunc);

        void BlendFunc(BlendFunc srcBlend, BlendFunc dstBlend);

        void FillMode(Mode mode);

        void SetViewport(int x, int y, uint width, uint height);

        void SetRenderTarget(RenderTarget renderTarget);

        void StencilMask(byte value);

        void StencilOp(StencilEnum stencilFail, StencilEnum depthFail, StencilEnum depthStencilPass);

        void StencilFunc(CompareEnum func, int reference, uint mask);

        void ClearStencil(int value);

        void ColorMask(int r, int g, int b, int a);
    }
}
