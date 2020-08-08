using Monorail.Graphics;
using Monorail.Mathlib;
using OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monorail.Platform
{
    public class ScreenSpaceQuad : IDisposable
    {

        VertexArrayObject m_VertexArrayObject;

        DefaultQuadBatchVertex[] m_Verticies;
        uint[] m_Indicies;



        public void Create()
        {
            m_Verticies = new DefaultQuadBatchVertex[4];

            var size = 1.0f;
            m_Verticies[0].Position = new Vector3(size, size, 0.0f);       // Top Right
            m_Verticies[1].Position = new Vector3(size, -size, 0.0f);            // Bottom Right
            m_Verticies[2].Position = new Vector3(-size, -size, 0.0f);                 // Bottom Left
            m_Verticies[3].Position = new Vector3(-size, size, 0.0f);            // Top Left

            m_Verticies[0].Color = new Vector3(1f, 1f, 1.0f);
            m_Verticies[1].Color = new Vector3(1.0f, 1.0f, 1.0f);
            m_Verticies[2].Color = new Vector3(1f, 1f, 1.0f);
            m_Verticies[3].Color = new Vector3(1f, 1.0f, 1.0f);

            m_Verticies[0].Texture = new Vector2(1, 1f);
            m_Verticies[1].Texture = new Vector2(1, 0);
            m_Verticies[2].Texture = new Vector2(0, 0);
            m_Verticies[3].Texture = new Vector2(0, 1f);

            m_Indicies = new uint[6];
            m_Indicies[0] = 0;
            m_Indicies[1] = 1;
            m_Indicies[2] = 3;
            m_Indicies[3] = 1;
            m_Indicies[4] = 2;
            m_Indicies[5] = 3;


            // Create VBOs
            m_VertexArrayObject = new VertexArrayObject();
            m_VertexArrayObject.InitElementsArrayBuffer<DefaultQuadBatchVertex>(DefaultQuadBatchVertex.Stride, DefaultQuadBatchVertex.AttributeLengths, DefaultQuadBatchVertex.AttributeOffsets, BufferUsageHint.GL_STATIC_DRAW);
            m_VertexArrayObject.SetIndexData32(m_Indicies);
            m_VertexArrayObject.SetVertexData<DefaultQuadBatchVertex>(m_Verticies);
        }

        public void Draw(uint textureId)
        {
            GameWindow.GraphicsDevice.BindTexture2D(textureId, OpenGL.TextureUnits.GL_TEXTURE0);
            GameWindow.GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE1);

            GameWindow.GraphicsDevice.BindShaderProgram(GameWindow.FullScreenShader.ShaderProgramId);

            GameWindow.QuadBatchShader.SetUniform("texture1", 0);

            GlBindings.BindVertexArray(m_VertexArrayObject.VaoId);
            GlBindings.DrawElements(PrimitiveType.TriangleList, 1 * 6, DrawElementsType.UnsignedInt, 0);

            GlBindings.BindVertexArray(0);
        }

        public void Dispose()
        {
            // TODO Free the vertex and Index Buffers
        }
    }
}
