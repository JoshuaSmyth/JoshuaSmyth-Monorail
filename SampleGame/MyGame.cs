using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using System;

namespace SampleGame
{


    public class MySampleGame : Game
    {
        // TODO Create resource->Shader manager
        ShaderProgram m_ShaderProgram;
        ShaderProgram m_SkyboxShader;

        // TODO Create resource->Geometry Manager
        VertexArrayObject m_SkyBox;
        VertexArrayObject m_TriVertexArrayObject;
        VertexArrayObject m_QuadVertexArrayObject;
        VertexArrayObject m_Cube;

        // TODO Create resource->Texture Manager
        Texture2D m_Texture;
        Texture2D m_AwesomeFace;
        Texture2D m_Default;

        // TODO Create resource->Font Manager
        TextureFont m_FontAriel;

        // TODO Create resource->CubeMap Manager
        TextureCubeMap m_CubeMap;

        //
        QuadBatch m_QuadBatch;

        // TODO Implement multiple cameras
        GameCamera camera;

        float rot;
        Vector3[] Positions;




        public override void Load()
        {
            camera = new GameCamera(new Vector3(0,1,-3), new Vector3(0,1,0), 90, 0);

            m_QuadBatch = new QuadBatch();

            m_ShaderProgram = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/vert1.glsl", "Resources/Shaders/Fragment/frag1.glsl");
            m_SkyboxShader = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.skybox.glsl", "Resources/Shaders/Fragment/f.skybox.glsl");

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
                var verts = Geometry.CreateIndexedQuadVerts(scale:4.0f);
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

            // Create Skyubox
            {
                var verts = Geometry.CreateSkyBox();
                m_SkyBox = new VertexArrayObject();
                m_SkyBox.BindArrayBuffer(verts, VertexPosition.Stride, VertexPosition.AttributeLengths, VertexPosition.AttributeOffsets);

                // Load Skybox Texture
                    m_CubeMap = TextureCubeMap.CreateFromFile("Resources/Textures/Skybox/front.png",
                                                              "Resources/Textures/Skybox/back.png",
                                                              "Resources/Textures/Skybox/bottom.png",
                                                              "Resources/Textures/Skybox/top.png",
                                                              "Resources/Textures/Skybox/left.png",
                                                              "Resources/Textures/Skybox/right.png");
                
            }

            // Load Font
            {
                m_FontAriel = TextureFont.CreateFromFile("Resources/Fonts/ariel.fnt");
            }

            // Load Texture
            {
                m_AwesomeFace = Texture2D.CreateFromFile("Resources/Textures/awesomeface.png");
                m_Texture = Texture2D.CreateFromFile("Resources/Textures/texture1.png");
                m_Default = Texture2D.CreateFromFile("Resources/Textures/default.png");
            }



        }

        public override void Update()
        {
            base.Update();
            camera.Update();

            rot += (float)(GameTime.ElapsedSeconds * 15.0f);

            if (this.Input.IsDown(KeyCode.KEYCODE_W))
            {
                camera.MoveForward(0.01f);
            }

            if ( this.Input.IsDown(KeyCode.KEYCODE_S))
            {
                camera.MoveForward(-0.01f);
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_A))
            {
                if (this.Input.IsDown(KeyCode.KEYCODE_LSHIFT))
                {
                    camera.MoveRight(-0.01f);
                }
                else
                {
                    camera.RotateYaw(-0.5f);
                }
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_D))
            {
                if (this.Input.IsDown(KeyCode.KEYCODE_LSHIFT))
                {
                    camera.MoveRight(0.01f);
                }
                else
                {
                    camera.RotateYaw(0.5f);
                }
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_UP))
            {
                camera.AdjustPitch(-0.5f);
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_DOWN))
            {
                camera.AdjustPitch(0.5f);
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_LEFT))
            {
                camera.MoveRight(-0.01f);
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_RIGHT))
            {
                camera.MoveRight(0.01f);
            }
        }

        public override void RenderScene()
        {
            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);

            // Render the skybox (TODO Move to the end of the pipeline)
            {
                GraphicsDevice.Disable(OpenGL.Enable.GL_DEPTH_TEST);

            //    GraphicsDevice.DepthFunc(OpenGL.DepthFunc.GL_LEQUAL);
                GraphicsDevice.BindTexture(m_CubeMap.TextureId, OpenGL.TextureType.GL_TEXTURE_CUBE_MAP, OpenGL.TextureUnits.GL_TEXTURE0);

                GraphicsDevice.UseShaderProgram(m_SkyboxShader.ShaderProgramId);
                m_SkyboxShader.SetUniform("view", camera.GetLookAtMatrix());
                m_SkyboxShader.SetUniform("proj", camera.ProjMatrix);
                m_SkyboxShader.SetUniform("skybox", 0);

                GraphicsDevice.UseVertexArrayObject(m_SkyBox.VertexArrayObjectId);
                GraphicsDevice.DrawArrays(PrimitiveType.TriangleList, 0, 36);
                //  GraphicsDevice.DepthFunc(OpenGL.DepthFunc.GL_LESS);
                GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);

            }

            
            // Render the cubes

            GraphicsDevice.UseShaderProgram(m_ShaderProgram.ShaderProgramId);
            m_ShaderProgram.SetUniform("texture1", 0);
            m_ShaderProgram.SetUniform("texture2", 1);
            m_ShaderProgram.SetUniform("model", camera.WorldMatrix);
            m_ShaderProgram.SetUniform("view", camera.ViewMatrix);
            m_ShaderProgram.SetUniform("proj", camera.ProjMatrix);

            // Render the Floor quad
            {
                GraphicsDevice.BindTexture2D(m_Default.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
                GraphicsDevice.BindTexture2D(m_Default.TextureId, OpenGL.TextureUnits.GL_TEXTURE1);
                GraphicsDevice.SetTextureSamplingAttribute(OpenGL.TextureAttributeValue.GL_NEAREST);

                GraphicsDevice.UseVertexArrayObject(m_QuadVertexArrayObject.VertexArrayObjectId);
                GraphicsDevice.DrawElements(PrimitiveType.TriangleList, 1 * 6, DrawElementsType.UnsignedInt, 0);

                GraphicsDevice.SetTextureSamplingAttribute(OpenGL.TextureAttributeValue.GL_LINEAR);
            }


            if (true)
            {
                GraphicsDevice.BindTexture2D(m_Texture.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
                GraphicsDevice.BindTexture2D(m_AwesomeFace.TextureId, OpenGL.TextureUnits.GL_TEXTURE1);

                GraphicsDevice.UseVertexArrayObject(m_Cube.VertexArrayObjectId);
                var cubeCount = Positions.Length;

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

            

            // Unbind textures
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE1);
        }

        public override void Render2D()
        {
            // THis just draws the text behind atm
            //GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);

            GraphicsDevice.BindTexture2D(m_Texture.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(m_AwesomeFace.TextureId, OpenGL.TextureUnits.GL_TEXTURE1);

            GraphicsDevice.Enable(OpenGL.Enable.GL_BLEND);
            GraphicsDevice.BlendFunc(OpenGL.BlendFunc.GL_SRC_ALPHA, OpenGL.BlendFunc.GL_ONE_MINUS_SRC_ALPHA);


            m_QuadBatch.Start();

            m_QuadBatch.DrawText("Awesome! Source!", Vector2.Zero, m_FontAriel, PresetColors.White);
            m_QuadBatch.DrawText("Awesome! Source!", new Vector2(10,80), m_FontAriel, PresetColors.White);
            m_QuadBatch.DrawText("Awesome! Source!", new Vector2(20, 160), m_FontAriel, PresetColors.White);

            m_QuadBatch.Commit();

            GraphicsDevice.Disable(OpenGL.Enable.GL_BLEND);

            // Unbind textures
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE1);
        }
    }
}
