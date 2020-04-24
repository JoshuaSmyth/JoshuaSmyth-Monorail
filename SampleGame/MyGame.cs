using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using OpenGL;
using System;

namespace SampleGame
{
    public class ResourceManager
    {
        // TODO;
    }

    public class MySampleGame : Game
    {
        RenderQueue m_RenderQueue;
        SkyBox m_SkyBox;
        BunnyModel m_BunnyModel;

        // TODO Create resource->Shader manager
        ShaderProgram m_QuadProgram;
        ShaderProgram m_terrainShder;
        ShaderProgram m_SkyboxShader;

        // TODO Create resource->Geometry Manager
        VertexArrayObject m_QuadVertexArrayObject;
        VertexArrayObject m_SkyBoxVAO;
        VertexArrayObject m_Cube;
        VertexArrayObject m_Bunny;
        VertexArrayObject m_Terrain;
        VertexArrayObject m_Water;

        // TODO Create resource->Texture Manager
        Texture2D m_Texture;
        Texture2D m_AwesomeFace;
        Texture2D m_Default;

        // TODO Create resource->Font Manager
        TextureFont m_FontAriel;

        // TODO Create resource->CubeMap Manager
        TextureCubeMap m_CubeMap;

        // TODO Create resource->Heightmap Manager
        HeightMapData m_HeightMapData;

        // TODO Create resource->QuadBatch Manager...
        QuadBatch m_QuadBatch;

        // TODO Implement multiple cameras
        GameCamera camera;

        float rot;
        Vector3[] Positions;

        bool IsWireframeMode;

        public override void Load()
        {
            camera = new GameCamera(new Vector3(0,1,-3), new Vector3(0,1,0), 90, 0);

            m_QuadBatch = new QuadBatch();

            m_QuadProgram = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/vert1.glsl", "Resources/Shaders/Fragment/frag1.glsl");
            m_SkyboxShader = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.skybox.glsl", "Resources/Shaders/Fragment/f.skybox.glsl");

            m_terrainShder = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.terrain.glsl", "Resources/Shaders/Fragment/f.terrain.glsl");
                        
            // Create Textured Indexed Quad
            {
                var verts = Geometry.CreateIndexedQuadVerts(scale:4.0f);
                uint[] indices = Geometry.CreateIndexedQuadIndicies();
               
                m_QuadVertexArrayObject = new VertexArrayObject();
                m_QuadVertexArrayObject.BindElementsArrayBuffer(verts, indices, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);
            }

            // Create Heightmap
            {
                m_HeightMapData = HeightMapData.LoadHeightmapData("Resources/Textures/Heightmaps/1.png");
                var model = ModelLoader.CreateTerrain(m_HeightMapData);

                m_Terrain = new VertexArrayObject();
                m_Terrain.BindElementsArrayBuffer(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);               
            }

            // Create Water
            {
                var model = ModelLoader.CreatePlane(512, 512, 12.2f);

                m_Water = new VertexArrayObject();
                m_Water.BindElementsArrayBuffer(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);
            }
        

            // Create Bunny
            {
                var bunnyVerts = ModelLoader.LoadObj("Resources/Models/bunny.obj");
                m_Bunny = new VertexArrayObject();
                m_Bunny.BindElementsArrayBuffer(bunnyVerts.Verts, bunnyVerts.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_BunnyModel = new BunnyModel(m_terrainShder.ShaderProgramId, m_Bunny.VertexArrayObjectId, m_Bunny.VertexCount);
            }

            // Create Cube
            {
                var verts = Geometry.CreateCube();
                m_Cube = new VertexArrayObject();
                m_Cube.BindArrayBuffer(verts, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);

                // Create Cube Positions
                {
                    Positions = new Vector3[10];
                    Positions[0] = new Vector3(  -2.0f,  0.0f,   0.0f);
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
                m_SkyBoxVAO = new VertexArrayObject();
                m_SkyBoxVAO.BindArrayBuffer(verts, VertexPosition.Stride, VertexPosition.AttributeLengths, VertexPosition.AttributeOffsets);

                // Load Skybox Texture
                m_CubeMap = TextureCubeMap.CreateFromFile("Resources/Textures/Skybox/front.png",
                                                            "Resources/Textures/Skybox/back.png",
                                                            "Resources/Textures/Skybox/bottom.png",
                                                            "Resources/Textures/Skybox/top.png",
                                                            "Resources/Textures/Skybox/left.png",
                                                            "Resources/Textures/Skybox/right.png");

                m_SkyBox = new SkyBox(m_SkyboxShader.ShaderProgramId, m_SkyBoxVAO.VertexArrayObjectId, m_SkyBoxVAO.VertexCount, m_CubeMap.TextureId);
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

            m_RenderQueue = new RenderQueue(this.GraphicsDevice);
        }

        public override void RenderScene()
        {
            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);

            m_RenderQueue.Render(m_SkyBox, camera);
            m_RenderQueue.Render(m_BunnyModel, camera);


            // Render Terrain
            {
                // TODO Toogle with tab
                if (this.IsWireframeMode)
                {
                    GraphicsDevice.FillMode(OpenGL.Mode.GL_LINE);
                }

                m_terrainShder.SetUniform("model", camera.WorldMatrix);
                
                GraphicsDevice.UseVertexArrayObject(m_Terrain.VertexArrayObjectId);
                GraphicsDevice.DrawElements(PrimitiveType.TriangleList, m_Terrain.VertexCount, DrawElementsType.UnsignedInt, 0);

                if (this.IsWireframeMode)
                {
                    GraphicsDevice.FillMode(OpenGL.Mode.GL_FILL);
                }
            }

            // Render Water
            {
                GraphicsDevice.Enable(OpenGL.Enable.GL_BLEND);
                GraphicsDevice.BlendFunc(OpenGL.BlendFunc.GL_SRC_ALPHA, OpenGL.BlendFunc.GL_ONE_MINUS_SRC_ALPHA);

                // TODO Set water shader
                GraphicsDevice.UseVertexArrayObject(m_Water.VertexArrayObjectId);
                GraphicsDevice.DrawElements(PrimitiveType.TriangleList, m_Water.VertexCount, DrawElementsType.UnsignedInt, 0);

                GraphicsDevice.Disable(OpenGL.Enable.GL_BLEND);
            }


            // Render Cubes
            {
                GraphicsDevice.UseShaderProgram(m_QuadProgram.ShaderProgramId);
                m_QuadProgram.SetUniform("texture1", 0);
                m_QuadProgram.SetUniform("texture2", 1);
                m_QuadProgram.SetUniform("model", camera.WorldMatrix);
                m_QuadProgram.SetUniform("view", camera.ViewMatrix);
                m_QuadProgram.SetUniform("proj", camera.ProjMatrix);
            
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

                    m_QuadProgram.SetUniform("model", model);

                    GraphicsDevice.DrawArrays(PrimitiveType.TriangleList, 0, 36);
                }
            }
        }

        public override void Render2D()
        {
            // THis just draws the text behind atm
            //GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);

            GraphicsDevice.BindTexture2D(m_Texture.TextureId, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(m_AwesomeFace.TextureId, OpenGL.TextureUnits.GL_TEXTURE1);

            GraphicsDevice.Enable(OpenGL.Enable.GL_BLEND);
            GraphicsDevice.BlendFunc(OpenGL.BlendFunc.GL_SRC_ALPHA, OpenGL.BlendFunc.GL_ONE_MINUS_SRC_ALPHA);

            /*
            m_QuadBatch.Start();

            m_QuadBatch.DrawText("Bunny!", Vector2.Zero, m_FontAriel, PresetColors.White);
            m_QuadBatch.DrawText("Bunny!", new Vector2(10,80), m_FontAriel, PresetColors.White);
            m_QuadBatch.DrawText("Bunny!", new Vector2(20, 160), m_FontAriel, PresetColors.White);

            m_QuadBatch.Commit();
            */

            GraphicsDevice.Disable(OpenGL.Enable.GL_BLEND);

            // Unbind textures
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE0);
            GraphicsDevice.BindTexture2D(0, OpenGL.TextureUnits.GL_TEXTURE1);
        }


        public override void Update()
        {
            base.Update();
            camera.Update();
            m_BunnyModel.Update(GameTime.ElapsedMilliseconds);

            rot += (float)(GameTime.ElapsedSeconds * 15.0f);

            if (this.Input.WasPressed(KeyCode.KEYCODE_TAB))
            {
                IsWireframeMode = !IsWireframeMode;
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_W))
            {
                if (this.Input.IsDown(KeyCode.KEYCODE_LSHIFT))
                {
                    camera.MoveForward(0.4f);
                }
                else
                {
                    camera.MoveForward(0.05f);
                }
            }

            if (this.Input.IsDown(KeyCode.KEYCODE_S))
            {
                if (this.Input.IsDown(KeyCode.KEYCODE_LSHIFT))
                {
                    camera.MoveForward(-0.4f);
                }
                else
                {
                    camera.MoveForward(-0.05f);
                }
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

    }
}
