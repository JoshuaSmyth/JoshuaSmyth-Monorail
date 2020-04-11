using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using System;

namespace SampleGame
{
    public class MySampleGame : Game
    {
        ShaderProgram m_ShaderProgram;
        VertexArrayObject m_TriVertexArrayObject;
        VertexArrayObject m_QuadVertexArrayObject;

        VertexArrayObject m_Cube;

        Texture2D m_Texture;
        Texture2D m_AwesomeFace;

        Matrix4 m_ModelMatrix;

        Matrix4 m_ViewMatrix;
        Matrix4 m_ProjMatrix;

        float rot;

        Vector3[] Positions;

        QuadBatch m_QuadBatch; // = new QuadBatch();    // TODO This doesn't work as The game doesn't seem to be initalised correctly at this stage.

        public override void Load()
        {
            m_QuadBatch = new QuadBatch();

            m_ShaderProgram = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/vert1.glsl", "Resources/Shaders/Fragment/frag1.glsl");
            
            // Create Triangle
            {
                var triVerts = new VertexPositionColor[3];
                triVerts[0].Position = new Vector3(-0.5f, -0.5f, 0.0f);
                triVerts[1].Position = new Vector3(0.5f, -0.5f, 0.0f);
                triVerts[2].Position = new Vector3(0.0f, 0.5f, 0.0f);

                triVerts[0].Color = new Vector3(1, 0, 0);
                triVerts[1].Color = new Vector3(1, 1, 1);
                triVerts[2].Color = new Vector3(0, 0, 1);

                m_TriVertexArrayObject = new VertexArrayObject();
                m_TriVertexArrayObject.BindArrayBuffer(triVerts, VertexPositionColor.Stride, VertexPositionColor.AttributeLengths, VertexPositionColor.AttributeOffsets);
            }
            
            // Create Textured Indexed Quad
            {
                var verts = Geometry.CreateIndexedQuadVerts();
                uint[] indices = Geometry.CreateIndexedQuadIndicies();
               
                m_QuadVertexArrayObject = new VertexArrayObject();
                m_QuadVertexArrayObject.BindElementsArrayBuffer(verts, indices, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);
            }

            // Create Cube
            {
                var verts = Geometry.CreateCube();
                m_Cube = new VertexArrayObject();
                m_Cube.BindArrayBuffer(verts, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);

                // Create Cube Positions
                {
                    Positions = new Vector3[10];
                    Positions[0] = new Vector3(  0.0f,  0.0f,   0.0f);
                    Positions[1] = new Vector3(  2.0f,  5.0f, -15.0f);
                    Positions[2] = new Vector3( -1.5f,  2.2f,  -2.5f);
                    Positions[3] = new Vector3( -3.8f, -2.0f, -12.3f);
                    Positions[4] = new Vector3(  2.4f, -0.4f,  -3.5f);
                    Positions[5] = new Vector3( -1.7f,  3.0f,  -7.5f);
                    Positions[6] = new Vector3(  1.3f, -2.0f,  -2.5f);
                    Positions[7] = new Vector3(  1.5f,  2.0f,  -2.5f);
                    Positions[8] = new Vector3(  1.5f,  0.2f,  -1.5f);
                    Positions[9] = new Vector3( -1.3f,  1.0f,  -1.5f);
                }
            }

            // Load Texture
            {
                m_Texture = Texture2D.CreateFromFile("Resources/Textures/texture1.png");
                m_AwesomeFace = Texture2D.CreateFromFile("Resources/Textures/awesomeface.png");
            }

            // Setup Matricies
            {
                m_ModelMatrix = Matrix4.CreateRotationX(MathHelper.ToRads(-55));
                m_ViewMatrix = Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, -2.0f));
                var aspect = GameWindow.ScreenWidth / (float)GameWindow.ScreenHeight;
                m_ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.ToRads(90), aspect, 0.5f, 100.0f);
            }
        }

        public override void Update()
        {
            base.Update();
            
            rot+= (float)(GameTime.ElapsedSeconds * 90.0f);
            m_ModelMatrix = Matrix4.CreateRotationX(MathHelper.ToRads(rot));

            if (this.Input.IsDown(KeyCode.KEYCODE_UP) || this.Input.IsDown(KeyCode.KEYCODE_W))
            {
                Console.WriteLine("KeyUp Pressed");

                for(int i=0;i<Positions.Length;i++)
                {
                    Positions[i].Z += 0.01f;
                }
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_DOWN) || this.Input.IsDown(KeyCode.KEYCODE_S))
            {
                Console.WriteLine("KeyDOWN Pressed");
                for (int i = 0; i < Positions.Length; i++)
                {
                    Positions[i].Z -= 0.01f;
                }
            }

            if (this.Input.WasPressed(KeyCode.KEYCODE_LEFT))
            {
                Console.WriteLine("KeyLEFT Pressed");
            }

            if (this.Input.WasPressed(KeyCode.KEYCODE_RIGHT))
            {
                Console.WriteLine("KeyRIGHT Pressed");
            }
        }

        public override void RenderScene()
        {
            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);
            GraphicsDevice.UseShaderProgram(m_ShaderProgram.ShaderProgramId);

            m_ShaderProgram.SetUniform("texture1", 0);
            m_ShaderProgram.SetUniform("texture2", 1);
            m_ShaderProgram.SetUniform("model", m_ModelMatrix);
            m_ShaderProgram.SetUniform("view", m_ViewMatrix);
            m_ShaderProgram.SetUniform("proj", m_ProjMatrix);

            GraphicsDevice.BindTexture2D(m_Texture.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(m_AwesomeFace.TextureId, OpenGL.TextureUnits.GL_TEXTURE1);

            GraphicsDevice.UseVertexArrayObject(m_Cube.VertexArrayObjectId);
            var cubeCount = 10;

            for (var i = 0; i < cubeCount; i++)
            {
                // calculate the model matrix for each object and pass it to shader before drawing
                var pos = Matrix4.CreateTranslation(Positions[i]);
                float angle = 20.0f * i;

                var rotAxis = new Vector3(1.0f, 0.0f, 1.0f);
                rotAxis = rotAxis.Normalize();
                var model = Matrix4.CreateRotation(rotAxis, MathHelper.ToRads(rot)) * pos;

                m_ShaderProgram.SetUniform("model", model);

                GraphicsDevice.DrawArrays(PrimitiveType.TriangleList, 0, 36);
            }
        }

        public override void Render2D()
        {
            // THis just draws the text behind atm
            GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);

            m_QuadBatch.Start();

            m_QuadBatch.DrawText("Awesome! Source!", Vector2.Zero, new TextureFont(), PresetColors.White);
            m_QuadBatch.DrawText("Awesome! Source!", new Vector2(10,200), new TextureFont(), PresetColors.White);
            m_QuadBatch.DrawText("Awesome! Source!", new Vector2(20, 400), new TextureFont(), PresetColors.White);

            m_QuadBatch.Commit();
        }
    }
}
