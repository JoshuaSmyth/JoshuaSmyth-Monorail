using ImGuiNET;
using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using SampleGame.GameObjects;

namespace SampleGame
{
    public class MySampleGame : Game
    {
        ImGuiDriver m_ImGuiDriver;

        // Render Objects
        SkyBox m_SkyBoxRenderObject;
        Bunny m_BunnyRenderObject;
        Terrain m_TerrainRenderObject;
        Water m_WaterRenderObject;

        Cube[] m_Cube;

        Texture2D m_Texture;
        Texture2D m_AwesomeFace;
        TextureFont m_FontAriel;
        QuadBatch m_QuadBatch;

        // TODO Implement multiple cameras
        GameCamera camera;


        float rot;
        bool IsWireframeMode;

        public override void Load()
        {
            m_ImGuiDriver = new ImGuiDriver();
            m_ImGuiDriver.Initalise(m_ResourceManager);

            camera = new GameCamera(new Vector3(0,1,-3), new Vector3(0,1,0), 90, 0);

            m_QuadBatch = new QuadBatch();


            var cubeShader = m_ResourceManager.LoadShaderFromFile("v.cube.glsl", "f.cube.glsl");
            var skyboxShader = m_ResourceManager.LoadShaderFromFile("v.skybox.glsl", "f.skybox.glsl");
            var terrainShader = m_ResourceManager.LoadShaderFromFile("v.terrain.glsl", "f.terrain.glsl");
            var waterShader = m_ResourceManager.LoadShaderFromFile("v.water.glsl", "f.water.glsl");


            // Load Textures
            {
                m_AwesomeFace = m_ResourceManager.LoadTexture2d("awesomeface.png");
                m_Texture = m_ResourceManager.LoadTexture2d("texture1.png");
                //m_Default = m_ResourceManager.LoadTexture2d("default.png");
            }


            // Load Terrain
            {
                var heightMapData = HeightMapData.LoadHeightmapData("Resources/Textures/Heightmaps/1.png");
                var model = ModelLoader.CreateTerrain(heightMapData);

                // Memory leak this has to come from somewhere...
                //var terrainVAO = new VertexArrayObject();
                var terrainVAO = m_ResourceManager.LoadVAO(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);
                //terrainVAO.BindElementsArrayBuffer(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_TerrainRenderObject = new Terrain(terrainShader.ShaderProgramId, terrainVAO.VaoId, terrainVAO.VertexCount);
            }

            // Create Water
            {
                var model = ModelLoader.CreatePlane(512, 512, 12.2f);

                // Memory Leak
                //var waterVAO = new VertexArrayObject();
                var waterVAO = m_ResourceManager.LoadVAO(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_WaterRenderObject = new Water(waterShader.ShaderProgramId, waterVAO.VaoId, waterVAO.VertexCount);
            }
        
            // Create Bunny
            {
                var bunnyVerts = ModelLoader.LoadObj("Resources/Models/bunny.obj");

                // Memory leak this needs to be cleaned up
                //var bunnyVAO = new VertexArrayObject();
                var bunnyVAO = m_ResourceManager.LoadVAO(bunnyVerts.Verts, bunnyVerts.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_BunnyRenderObject = new Bunny(terrainShader.ShaderProgramId, bunnyVAO.VaoId, bunnyVAO.VertexCount);
            }

            // Create Cube
            {
                var verts = Geometry.CreateCube();

                // memory leak
               // var m_CubeVAO = new VertexArrayObject();    // TODO Generate indicies
                var m_CubeVAO = m_ResourceManager.LoadVAO(verts, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);

                m_Cube = new Cube[10];
                for(int i=0;i<10;i++)
                {
                    m_Cube[i] = new Cube(cubeShader.ShaderProgramId, m_CubeVAO.VaoId, m_CubeVAO.VertexCount);
                    m_Cube[i].i = i;
                    m_Cube[i].TextureIdA = m_AwesomeFace.TextureId;
                    m_Cube[i].TextureIdB = m_Texture.TextureId;
                }

                // Create Cube Positions
                {
                    m_Cube[0].Position = new Vector3(  -2.0f,  0.0f,   0.0f);
                    m_Cube[1].Position = new Vector3(  2.0f,  5.0f, -15.0f);
                    m_Cube[2].Position = new Vector3( -1.5f,  2.2f,  -2.5f);
                    m_Cube[3].Position = new Vector3( -3.8f, -2.0f, -12.3f);
                    m_Cube[4].Position = new Vector3(  2.4f, -0.4f,  -3.5f);
                    m_Cube[5].Position = new Vector3( -1.7f,  3.0f,  -7.5f);
                    m_Cube[6].Position = new Vector3(  1.3f, -2.0f,  -2.5f);
                    m_Cube[7].Position = new Vector3(  1.5f,  2.0f,  -2.5f);
                    m_Cube[8].Position = new Vector3(  1.5f,  0.2f,  -1.5f);
                    m_Cube[9].Position = new Vector3( -1.3f,  1.0f,  -1.5f);
                }
            }

            // Create Skyubox
            {
                var verts = Geometry.CreateSkyBoxVerticies();
                var skyboxVAO = m_ResourceManager.LoadVAO(verts, VertexPosition.Stride, VertexPosition.AttributeLengths, VertexPosition.AttributeOffsets);


                var cubeMap = m_ResourceManager.LoadCubeMap("Skybox/front.png", "Skybox/back.png", "Skybox/bottom.png", "Skybox/top.png", "Skybox/left.png", "Skybox/right.png");

                m_SkyBoxRenderObject = new SkyBox(skyboxShader.ShaderProgramId, skyboxVAO.VaoId, skyboxVAO.VertexCount, cubeMap.TextureId);
            }

            // Load Font
            {
                m_FontAriel = m_ResourceManager.LoadTextureFont("ariel.fnt");
            }
        }

        public override void RenderScene()
        {
            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);

            m_TerrainRenderObject.IsWireframe = IsWireframeMode;

            /*
            OpenGL.GlBindings.Enable(OpenGL.Enable.GL_SCISSOR_TEST);
            OpenGL.GlBindings.glScissor(200, 200, 250, 250);
            */

            m_RenderQueue.Render(m_SkyBoxRenderObject, camera);
            m_RenderQueue.Render(m_BunnyRenderObject, camera);
            m_RenderQueue.Render(m_TerrainRenderObject, camera);

            foreach (var cube in m_Cube)
            {
                m_RenderQueue.Render(cube, camera);
            }

            m_RenderQueue.Render(m_WaterRenderObject, camera);
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


            // Render IMGUI
            m_ImGuiDriver.Begin();

            // TODO ImGUI calls here.
                m_ImGuiDriver.Draw();

            m_ImGuiDriver.End();
        }


        public override void Update()
        {
            base.Update();
            camera.Update();
            m_BunnyRenderObject.Update(GameTime.ElapsedMilliseconds);

            rot += (float)(GameTime.ElapsedSeconds * 15.0f);

            foreach (var cube in m_Cube)
            {
                cube.Update(rot);
            }

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
