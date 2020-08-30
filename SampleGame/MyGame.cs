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
        ScreenSpaceQuad quad;

        RenderTarget renderTarget;

        float rot;
        bool IsWireframeMode;

        public override void Load()
        {
            m_ImGuiDriver = new ImGuiDriver();
            m_ImGuiDriver.Initalise(GraphicsDevice, m_ResourceManager);

            camera = new GameCamera(new Vector3(40,40,40), new Vector3(80,80,0), 55, -20);

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

                var terrainVAO = m_ResourceManager.LoadVAO(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_TerrainRenderObject = new Terrain(terrainShader.ShaderProgramId, terrainVAO.VaoId, terrainVAO.VertexCount);
            }

            // Create Water
            {
                var model = ModelLoader.CreatePlane(1024, 1024, 12.2f);

                var waterVAO = m_ResourceManager.LoadVAO(model.Verts, model.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_WaterRenderObject = new Water(waterShader.ShaderProgramId, waterVAO.VaoId, waterVAO.VertexCount);
            }
        
            // Create Bunny
            {
                var bunnyVerts = ModelLoader.LoadObj("Resources/Models/bunny.obj");

                var bunnyVAO = m_ResourceManager.LoadVAO(bunnyVerts.Verts, bunnyVerts.Indicies, VertexPositionColorTextureNormal.Stride, VertexPositionColorTextureNormal.AttributeLengths, VertexPositionColorTextureNormal.AttributeOffsets);

                m_BunnyRenderObject = new Bunny(terrainShader.ShaderProgramId, bunnyVAO.VaoId, bunnyVAO.VertexCount);
            }

            
            // Create Cube
            {
                Maploader mp = new Maploader();
                mp.Load();

                var verts = Geometry.CreateCube();

                var m_CubeVAO = m_ResourceManager.LoadNonIndexedVAO(verts, VertexPositionColorTexture.Stride, VertexPositionColorTexture.AttributeLengths, VertexPositionColorTexture.AttributeOffsets);

                m_Cube = new Cube[Maploader.width*Maploader.height];
                for(int i=0;i< Maploader.width * Maploader.height; i++)
                {
                    m_Cube[i] = new Cube(cubeShader.ShaderProgramId, m_CubeVAO.VaoId, m_CubeVAO.VertexCount);
                    m_Cube[i].i = i;
                    m_Cube[i].TextureIdA = m_AwesomeFace.TextureId;
                    m_Cube[i].TextureIdB = m_Texture.TextureId;
                }

                int k = 0;
                for (int i = 0; i < Maploader.width; i++)
                {
                    for (int j = 0; j < Maploader.height; j++)
                    {
                        var height = (float)mp.cubes[i, j];

                        height /= 2.0f;

                        if (height < 0)
                        {
                            height = -100; // Temp
                        }
                        height += 20;
                        m_Cube[k].Position = new Vector3(i, height, j);
                        k++;
                    }
                }
            }

            // Create Skyubox
            {
                var verts = Geometry.CreateSkyBoxVerticies();
                var skyboxVAO = m_ResourceManager.LoadNonIndexedVAO(verts, VertexPosition.Stride, VertexPosition.AttributeLengths, VertexPosition.AttributeOffsets);


                var cubeMap = m_ResourceManager.LoadCubeMap("Skybox/front.png", "Skybox/back.png", "Skybox/bottom.png", "Skybox/top.png", "Skybox/left.png", "Skybox/right.png");

                m_SkyBoxRenderObject = new SkyBox(skyboxShader.ShaderProgramId, skyboxVAO.VaoId, skyboxVAO.VertexCount, cubeMap.TextureId);
            }

            // Load Font
            {
                m_FontAriel = m_ResourceManager.LoadTextureFont("ariel.fnt");
            }


            renderTarget = RenderTarget.Create(GameWindow.ScreenWidth, GameWindow.ScreenHeight); // TODO This should be the size of the screen

            quad = new ScreenSpaceQuad();
            quad.Create();
        }

        public override void RenderScene()
        {
            // Set Render Target
            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);

            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(PresetColors.CornFlowerBlue);
            GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);

            m_TerrainRenderObject.IsWireframe = IsWireframeMode;

            // Clear all buffers
            GraphicsDevice.ClearStencil(0);
            GraphicsDevice.Clear(new Vector4(0, 0, 0, 0), ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            m_RenderQueue.Render(m_SkyBoxRenderObject, camera);

            // TODO
            // Due to the depth planes being split we probably want to generate bounding spheres for each of our render objects so we can determine which depth plane
            // They are in.



            // Far Far
            if (true)
            {
                camera.NearPlane = 999f;
                camera.FarPLane = 10000f;
                camera.Update();


                // Render terrain
                // Overwrite the stencil (not strictly nessesary in the far far plane.
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO);
                m_RenderQueue.Render(m_TerrainRenderObject, camera);


                // TODO Write to stencil buffer only
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_REPLACE);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilMask(0xFF);
                GraphicsDevice.ColorMask(0, 0, 0, 0);
                    m_RenderQueue.Render(m_WaterRenderObject, camera);
                GraphicsDevice.Disable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.ColorMask(1, 1, 1, 1);


                // Clear depth buffer between splitting the rendering
                GraphicsDevice.Clear(new Vector4(1, 1, 1, 1), ClearBufferMask.DepthBufferBit);
            }


            // Far
            
            if (true)
            {
                // For opaque geometry overlapping the near plane with the far plane works best.
                // But for transparency you get more of an 'apha line'
                // Maybe in the next pass some kind of w-buffer could be used?

                camera.NearPlane = 99.5f; // Could we scale the error based on the angle of the camera?
                camera.FarPLane = 1000f;
                camera.Update();


                // Render terrain

                // Overwrite the stencil
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO);
                m_RenderQueue.Render(m_TerrainRenderObject, camera);


                // Want to pass the test, but zero it out when we pass
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO);

                foreach (var cube in m_Cube)
                {
                    m_RenderQueue.Render(cube, camera);
                }

                m_RenderQueue.Render(m_BunnyRenderObject, camera);


                // TODO Write to stencil buffer only
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_REPLACE);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilMask(0xFF);
                GraphicsDevice.ColorMask(0, 0, 0, 0);
                    m_RenderQueue.Render(m_WaterRenderObject, camera);
                GraphicsDevice.Disable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.ColorMask(1, 1, 1, 1);

                //m_RenderQueue.Render(m_WaterRenderObject, camera);

                // Clear depth buffer between splitting the rendering
                GraphicsDevice.Clear(new Vector4(1, 1, 1, 1), ClearBufferMask.DepthBufferBit);
            }
            

            // Near
            if (true)
            {
                camera.NearPlane = 0.01f;
                camera.FarPLane = 100f;
                camera.Update();


                m_RenderQueue.Render(m_TerrainRenderObject, camera);

                // Want to pass the test, but zero it out when we pass
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO);

                foreach (var cube in m_Cube)
                {
                    m_RenderQueue.Render(cube, camera);
                }

             


                // TODO Write to stencil buffer only
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_KEEP, OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_REPLACE);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_ALWAYS, 1, 0xFF);
                GraphicsDevice.StencilMask(0xFF);
                GraphicsDevice.ColorMask(0, 0, 0, 0);
                m_RenderQueue.Render(m_WaterRenderObject, camera);
                GraphicsDevice.Disable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.ColorMask(1, 1, 1, 1);


                // Clear depth buffer between splitting the rendering
                GraphicsDevice.Clear(new Vector4(1, 1, 1, 1), ClearBufferMask.DepthBufferBit);
            }


            // Draw Large Transparencies
            if (true)
            {
                camera.NearPlane = 0.01f;
                camera.FarPLane = 10000f;
                camera.Update();

                // Disable Depth test
                GraphicsDevice.Disable(OpenGL.Enable.GL_DEPTH_TEST);

                // Draw water based on 
                GraphicsDevice.Enable(OpenGL.Enable.GL_STENCIL_TEST);
                GraphicsDevice.StencilMask(0xFF);
                GraphicsDevice.StencilOp(OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_ZERO, OpenGL.StencilEnum.GL_ZERO);
                GraphicsDevice.StencilFunc(OpenGL.CompareEnum.GL_EQUAL, 1, 0xFF);
                m_RenderQueue.Render(m_WaterRenderObject, camera);
                GraphicsDevice.Disable(OpenGL.Enable.GL_STENCIL_TEST);
            }

            // End Set Render Target
            GraphicsDevice.SetRenderTarget(null);


            // Render the render Target
            GraphicsDevice.Disable(OpenGL.Enable.GL_DEPTH_TEST);
            quad.Draw((uint)renderTarget.TextureColorBufferId);
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



            //// Render IMGUI
            /*
            m_ImGuiDriver.Begin();

            // TODO ImGUI calls here.
            m_ImGuiDriver.Draw();

            m_ImGuiDriver.Submit();
            */
        }


        public override void Update()
        {
            base.Update();
            camera.Update();
            m_BunnyRenderObject.Update(GameTime.ElapsedMilliseconds);

            rot = 0; //+= (float)(GameTime.ElapsedSeconds * 15.0f);


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
