using Monorail.Graphics;
using Monorail.Mathlib;
using OpenGL;
using System;

namespace Monorail.Platform
{
    public class QuadBatch : IDisposable
    {
        // FEATURES
        // - Bind on Texture Units (up to 16) note: Textures must all be the same size. Suggest 2k by 2k
        // - CPU RectClip, GPU Rotation? GPU Scale?
        // - Seperate Depth Buffer (Requires clearing the depth buffer, so 2D objects should be drawn last)
        // - Support custom vertex Format and custom shader program. E.g To Support Normal Mapped Sprites
        // - Pass BlendState (Additive/Alpha/Non-Premultiplied?)

        // The above should mean that a lot of sprites can be drawn in 1 draw call. (65k indicies = ~10k sprites)

        private struct QuadRecord
        {
            public int textureId;

            public float U;
            public float V;

            public float UW;
            public float VH;

            public float X;
            public float Y;
            public float W;
            public float H;
        }
        
        public static uint[] CreateIndexedQuadIndicies(int quadCount)
        {
            // TODO Change to UShort
            var rv = new uint[6 * quadCount];
            for (uint i = 0; i < quadCount; i++)
            {
                rv[i*6 + 0] = 0 + i * 4;
                rv[i*6 + 1] = 1 + i * 4;
                rv[i*6 + 2] = 3 + i * 4;
                rv[i*6 + 3] = 1 + i * 4;
                rv[i*6 + 4] = 2 + i * 4;
                rv[i*6 + 5] = 3 + i * 4;
            }

            return rv;
        }

        private static DefaultQuadBatchVertex[] CreateIndexedQuadVerts(int quadCount)
        {
            // Origin is bottom left
            // UV is bottom left but inversed Y (TODO Fix this)

            var rv = new DefaultQuadBatchVertex[4 * quadCount];
            for (int i = 0; i < quadCount; i++)
            {
                rv[i*4 + 0].Position = new Vector3(32.0f, 32.0f, 0.0f);       // Top Right
                rv[i*4 + 1].Position = new Vector3(32.0f, 0, 0.0f);            // Bottom Right
                rv[i*4 + 2].Position = new Vector3(0, 0, 0.0f);                 // Bottom Left
                rv[i*4 + 3].Position = new Vector3(0, 32.0f, 0.0f);            // Top Left

                rv[i*4 + 0].Color = new Vector3(1f, 1f, 1.0f);
                rv[i*4 + 1].Color = new Vector3(1.0f, 1.0f, 1.0f);
                rv[i*4 + 2].Color = new Vector3(1f, 1f, 1.0f);
                rv[i*4 + 3].Color = new Vector3(1f, 1.0f, 1.0f);

                rv[i*4 + 0].Texture = new Vector2(1f, 1f);                      // Note: Inverted Y
                rv[i*4 + 1].Texture = new Vector2(1f, 0f);
                rv[i*4 + 2].Texture = new Vector2(0f, 0f);
                rv[i*4 + 3].Texture = new Vector2(0f, 1f);
            }

            return rv;
        }

        const int QUAD_BUFFER_SIZE = 2048;

        int m_quadRecordCount;

        QuadRecord[] quadRecords = new QuadRecord[QUAD_BUFFER_SIZE];

        DefaultQuadBatchVertex[] m_Verticies;
        uint[] m_Indicies;

        VertexArrayObject m_VertexArrayObject;
        
        public QuadBatch()
        {
            m_Verticies = CreateIndexedQuadVerts(QUAD_BUFFER_SIZE);
            m_Indicies = CreateIndexedQuadIndicies(QUAD_BUFFER_SIZE);

            m_VertexArrayObject = new VertexArrayObject();
            m_VertexArrayObject.InitElementsArrayBuffer<DefaultQuadBatchVertex>(DefaultQuadBatchVertex.Stride, DefaultQuadBatchVertex.AttributeLengths, DefaultQuadBatchVertex.AttributeOffsets, BufferUsageHint.GL_DYNAMIC_DRAW);
            m_VertexArrayObject.SetIndexData<uint>(m_Indicies);
            m_VertexArrayObject.SetVertexData<DefaultQuadBatchVertex>(m_Verticies);
        }

        public void Start()
        {
            m_quadRecordCount = 0;
        }

        public void Commit()
        {
            Flush();
        }

        public void Dispose()
        {
            // TODO Free the vertex and Index Buffers
        }

        public void DrawText(string text, Vector2 position, TextureFont font, Vector4 color, TextureUnits textureUnit=TextureUnits.GL_TEXTURE0, int layer=0)
        {
            // TODO? Make a seperate FontBatcher?
            // See http://www.angelcode.com/products/bmfont/doc/render_text.html
            // To improve the rendering...

            var fontSheetWidth = (float)font.Font.Common.ScaleW;
            var fontSheetHeight = (float)font.Font.Common.ScaleH;

            for (int i = 0; i < text.Length; i++)
            {

                var quad = new QuadRecord();
                quad.X = 32 + i * 32 + 16 * i + position.X;
                quad.Y = 32 + (2*i) + position.Y;
                quad.W = 32;
                quad.H = 32;

                // Compute the U, V and UW, UH values
                var charId = (byte)text[i];
                var fontChar = font.CharLookup[charId];

                quad.U = fontChar.X / fontSheetWidth;
                quad.V = fontChar.Y / fontSheetHeight;

                quad.UW = fontChar.Width / fontSheetWidth;
                quad.VH = fontChar.Height / fontSheetHeight;



                PushQuad(quad);
            }

            var device = GameWindow.GraphicsDevice;

            var sX = 2 / (float)GameWindow.ScreenWidth;
            var sY = 2 / (float)GameWindow.ScreenHeight;

            var oX = -1;
            var oY = 1;

            var quadCount = m_quadRecordCount;

            // Edit Vertex Data
            for (int i = 0; i < quadCount; i++)
            {
                var record = quadRecords[i];
                m_Verticies[i * 4 + 0].Position = new Vector3(oX + (record.X + record.W) * sX, oY - (record.Y + record.H) * sY, 0.0f);       // Top Right
                m_Verticies[i * 4 + 1].Position = new Vector3(oX + (record.X + record.W) * sX, oY - (record.Y * sY), 0.0f);                  // Bottom Right
                m_Verticies[i * 4 + 2].Position = new Vector3(oX + record.X * sX, oY - (record.Y * sY), 0.0f);                               // Bottom Left
                m_Verticies[i * 4 + 3].Position = new Vector3(oX + record.X * sX, oY - (record.Y + record.H) * sY, 0.0f);                    // Top Left

                
                m_Verticies[i * 4 + 0].Texture = new Vector2(record.U + record.UW, record.V + record.VH);                      // Note: Inverted Y
                m_Verticies[i * 4 + 1].Texture = new Vector2(record.U + record.UW, record.V);
                m_Verticies[i * 4 + 2].Texture = new Vector2(record.U, record.V);
                m_Verticies[i * 4 + 3].Texture = new Vector2(record.U, record.V + record.VH);
                
            }

            m_VertexArrayObject.SetVertexData<DefaultQuadBatchVertex>(m_Verticies, 0, quadCount * 6);


            // TODO Make a font.glsl shader
        //    GlBindings.PolygonMode(Face.GL_FRONT_AND_BACK, Mode.GL_LINE);

            GameWindow.GraphicsDevice.BindTexture2D(font.FontTexture.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
            GameWindow.GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE1);

            GameWindow.GraphicsDevice.UseShaderProgram(GameWindow.QuadBatchShader.ShaderProgramId);

            GameWindow.QuadBatchShader.SetUniform("texture1", 0);

            GlBindings.BindVertexArray(m_VertexArrayObject.VaoId);
            GlBindings.DrawElements(PrimitiveType.TriangleList, quadCount * 6, DrawElementsType.UnsignedInt, 0);

       //     GlBindings.PolygonMode(Face.GL_FRONT_AND_BACK, Mode.GL_FILL);

            m_quadRecordCount = 0;
        }

        public void DrawText(string text, Rect clipRect, TextureUnits textureUnit)
        {
            // TODO Convert chars to quadrecord
            //throw new NotImplementedException();
        }

        public void DrawSprite(int x, int y)
        {
            var quad = new QuadRecord();
            quad.X = x;
            quad.Y = y;
            PushQuad(quad);
        }

        private void Flush()
        {
            if (m_quadRecordCount > 0)
            {
                var device = GameWindow.GraphicsDevice;

                var sX = 2 / (float)GameWindow.ScreenWidth;
                var sY = 2 / (float)GameWindow.ScreenHeight;

                var oX = -1;
                var oY = 1;

                var quadCount = m_quadRecordCount;

                // Edit Vertex Data
                for (int i = 0; i < quadCount; i++)
                {
                    var record = quadRecords[i];
                    m_Verticies[i * 4 + 0].Position = new Vector3(oX + (record.X + record.W) * sX, oY - (record.Y + record.H) * sY, 0.0f);       // Top Right
                    m_Verticies[i * 4 + 1].Position = new Vector3(oX + (record.X + record.W) * sX, oY - (record.Y * sY), 0.0f);                  // Bottom Right
                    m_Verticies[i * 4 + 2].Position = new Vector3(oX + record.X * sX, oY - (record.Y * sY), 0.0f);                               // Bottom Left
                    m_Verticies[i * 4 + 3].Position = new Vector3(oX + record.X * sX, oY - (record.Y + record.H) * sY, 0.0f);                    // Top Left
                }

                m_VertexArrayObject.SetVertexData<DefaultQuadBatchVertex>(m_Verticies, 0, quadCount * 6);

                // TODO Access the IPlatformGraphicsDevice
                GameWindow.GraphicsDevice.UseShaderProgram(GameWindow.QuadBatchShader.ShaderProgramId);
                GameWindow.QuadBatchShader.SetUniform("texture1", 0);
                GameWindow.QuadBatchShader.SetUniform("texture2", 1);

                GlBindings.BindVertexArray(m_VertexArrayObject.VaoId);
                GlBindings.DrawElements(PrimitiveType.TriangleList, quadCount * 6, DrawElementsType.UnsignedInt, 0);

                m_quadRecordCount = 0;
            }
        }

        private void PushQuad(QuadRecord record)
        {

            if (m_quadRecordCount == QUAD_BUFFER_SIZE)
            {
                Flush();
            }

            quadRecords[m_quadRecordCount] = record;
            m_quadRecordCount++;
        }
    }
}
