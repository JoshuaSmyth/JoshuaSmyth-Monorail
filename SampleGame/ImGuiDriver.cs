using ImGuiNET;
using Monorail;
using Monorail.Graphics;
using Monorail.Platform;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SampleGame
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexImGui : IInterleavedVertex
    {
        public Vector2 Position;

        public Vector2 Texture;

        public uint Color;

        public static int Stride = 20;

        public static int[] AttributeLengths { get { return new[] { 2, 2, 1 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0, 8, 12 }; } }
    }

    public class ImGuiDriver
    {
        private VertexArrayObject m_VertexArrayObject;
        private VertexImGui[] m_Verts = new VertexImGui[65536];
        private ushort[] m_Indicies16 = new ushort[65536];
        private uint[] m_Indicies32 = new uint[65536];

        private IntPtr? _fontTextureId;

        private ResourceManager m_ResourceManager;

        private ShaderProgram m_ShaderProgram;
        private Texture2D m_Texture;
        private VertexBuffer<VertexImGui> m_VertexBuffer;
        private IndexBuffer m_IndexBuffer;

        private IPlatformGraphicsDevice m_GraphicsDevice;

        string fragShaderCode = @"#version 330 core
                               uniform sampler2D FontTexture;

                               in vec4 color;
                               in vec2 texCoord;

                               out vec4 outputColor;

                               void main()
                               {
                                    outputColor = color * texture(FontTexture, texCoord);
                               }";

        string vertShaderCode = @"#version 330 core
                               uniform mat4 projection_matrix;

                               in vec2 in_position;
                               in vec2 in_texCoord;
                               in vec4 in_color;

                               out vec4 color;
                               out vec2 texCoord;

                               void main()
                               {
                                   gl_Position = projection_matrix * vec4(in_position, 0, 1);
                                   color = in_color;
                                   texCoord = in_texCoord;
                               }";

        public void Initalise(IPlatformGraphicsDevice graphicsDevice, ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;
            m_GraphicsDevice = graphicsDevice;

            // Load Context
            {
                IntPtr context = ImGui.CreateContext();
                ImGui.SetCurrentContext(context);
            }

            // Load Fonts
            {
                var fonts = ImGui.GetIO().Fonts;
                ImGui.GetIO().Fonts.AddFontDefault();
            }

            // Load Graphics Resources
            {
                m_ShaderProgram = m_ResourceManager.LoadShaderFromString(this.vertShaderCode, this.fragShaderCode);
                m_VertexArrayObject = m_ResourceManager.LoadVAO<VertexImGui>(m_Verts, m_Indicies32, VertexImGui.Stride, VertexImGui.AttributeLengths, VertexImGui.AttributeOffsets, OpenGL.BufferUsageHint.GL_DYNAMIC_DRAW );

                SetKeyMappings();
                BuildFontAtlas();
                CreateTexture();
            }
        }

        private void CreateTexture()
        {
            // TODO?
        }

        public virtual unsafe void BuildFontAtlas()
        {
            IntPtr pixels = IntPtr.Zero;

            // Get font texture from ImGui
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixelData, out int width, out int height, out int bytesPerPixel);

            // Create and register the texture
            m_Texture = m_ResourceManager.CreateTexture2d(pixels, width, height, bytesPerPixel);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (_fontTextureId.HasValue)
            {
                throw new NotImplementedException("Implement font rebuild");
                //UnbindTexture(_fontTextureId.Value);
            }

            _fontTextureId = (IntPtr) m_Texture.TextureId;

            io.Fonts.SetTexID(_fontTextureId.Value);
            io.Fonts.ClearTexData();
        }

        public void Begin()
        {
            

            UpdateIO();

            ImGui.NewFrame();
        }

        protected virtual void UpdateIO()
        {
            // TODO Update Input

            var io = ImGui.GetIO();
            io.DeltaTime = (float)GameTime.ElapsedSeconds;
            
            // TODO Read height and width from the graphics device
            io.DisplaySize = new System.Numerics.Vector2(1280, 800);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);


            // TODO Update Input
            {
                /*
                var mouse = Mouse.GetState();
                var keyboard = Keyboard.GetState();

                for (int i = 0; i < _keys.Count; i++)
                {
                    io.KeysDown[_keys[i]] = keyboard.IsKeyDown((Keys)_keys[i]);
                }

                io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
                io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
                io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
                io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

                io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);

                io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
                io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
                io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

                var scrollDelta = mouse.ScrollWheelValue - _scrollWheelValue;
                io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
                _scrollWheelValue = mouse.ScrollWheelValue;
                */
            }
        }

        public void Submit()
        {
            ImGui.Render();
            unsafe { RenderDrawData(ImGui.GetDrawData()); }
        }

        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            UpdateBuffers(drawData);

            RenderCommandLists(drawData);
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            // Can we fit the verticies in the current vertex buffer?
            if (drawData.TotalVtxCount > m_Verts.Length)
            {
                // TODO resize vertex buffer
                // Or multiple buffers
                // Or multiple draw calls
                throw new Exception("TODO need a vertex buffer");
                return;
            }

            // Can we fit the indicies in the current index buffer
            if (drawData.TotalIdxCount > m_Indicies16.Length)
            {
                // TODO resize index buffer
                // Or multiple buffers
                // Or multiple draw calls
                throw new Exception("TODO need an index buffer");
                return;
            }


            int vertCount = 0;
            int indexCount = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &m_Verts[vertCount * 20])
                fixed (void* idxDstPtr = &m_Indicies16[indexCount * sizeof(ushort)])
                {
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, m_Verts.Length, cmdList.VtxBuffer.Size * VertexImGui.Stride);
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, m_Indicies16.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vertCount += cmdList.VtxBuffer.Size;
                indexCount += cmdList.IdxBuffer.Size;
            }

            // This is a bit of a hack but lets copy the 16bit indicies into 32bit buffers
            // This is a slow path was we don't have 16bit VAOs yet.
            for(int i=0;i<indexCount;i++)
            {
                m_Indicies32[i] = m_Indicies16[i];
            }
       
            m_VertexArrayObject.UpdateVertexData<VertexImGui>(m_Verts, 0, vertCount);
            m_VertexArrayObject.UpdateIndexData32(m_Indicies32);

            return;
        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            // TODO Need othomatrix
            var m_Ortho = Monorail.Mathlib.Matrix4.CreateOrthographic(1280, 800, 0.1f, 100);

            m_GraphicsDevice.BindVertexArrayObject(m_VertexArrayObject.VaoId);

            var drawcount = 0;
            int vtxOffset = 0;
            int idxOffset = 0;
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];
                    var texId = drawCmd.TextureId;


//                    OpenGL.GlBindings.Enable(OpenGL.Enable.GL_SCISSOR_TEST);
  //                  OpenGL.GlBindings.glScissor((int)drawCmd.ClipRect.X, (int)drawCmd.ClipRect.Y, (uint)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X), (uint)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y));


                    /*
       
                    */


                    /*
                     *  (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y
                     */

                    // TODO If textureId not loaded?
                    //   m_GraphicsDevice.ScissorRect();

                    //  m_ShaderProgram.SetUniform("projection_matrix", m_Ortho);
                    m_GraphicsDevice.BindShaderProgram(m_ShaderProgram.ShaderProgramId);

                    m_ShaderProgram.SetUniform("projection_matrix", m_Ortho);

                    m_GraphicsDevice.BindVertexArrayObject(m_VertexArrayObject.VaoId);
                    m_GraphicsDevice.BindTexture(m_Texture.TextureId, OpenGL.TextureType.GL_TEXTURE_2D);

                    m_GraphicsDevice.DrawElements(PrimitiveType.TriangleList, cmdList.VtxBuffer.Size, DrawElementsType.UnsignedInt, idxOffset);


                    drawcount++;

                    idxOffset += (int)drawCmd.ElemCount;
                }
                vtxOffset += cmdList.VtxBuffer.Size;
            }

       //     OpenGL.GlBindings.Disable(OpenGL.Enable.GL_SCISSOR_TEST);

        }

        private static void SetKeyMappings()
        {
            // Surely there is more than just this?

            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.KEYCODE_TAB;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.KEYCODE_LEFT;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.KEYCODE_RIGHT;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.KEYCODE_UP;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.KEYCODE_DOWN;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.KEYCODE_PAGEUP;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.KEYCODE_PAGEDOWN;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.KEYCODE_HOME;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.KEYCODE_END;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.KEYCODE_DELETE;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.KEYCODE_BACKSPACE;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.KEYCODE_KP_ENTER;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyCode.KEYCODE_ESCAPE;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.KEYCODE_A;
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.KEYCODE_C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.KEYCODE_V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.KEYCODE_X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)KeyCode.KEYCODE_Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.KEYCODE_Z;
        }


        // Persisted IMGUI State
        bool _showDemoWindow;
        bool _showAnotherWindow;
        bool _showMemoryEditor;
        int _counter;
        int _dragInt;
        float _f;
        private static uint s_tab_bar_flags = (uint)ImGuiTabBarFlags.Reorderable;
        static bool[] s_opened = { true, true, true, true }; // Persistent user state

        public void Draw()
        {
            {
                ImGui.Text("Hello, world!");                                        // Display some text (you can use a format string too)
                ImGui.SliderFloat("float", ref _f, 0, 1, _f.ToString("0.000"), 1);  // Edit 1 float using a slider from 0.0f to 1.0f    
                //ImGui.ColorEdit3("clear color", ref _clearColor);                   // Edit 3 floats representing a color

                ImGui.Text($"Mouse position: {ImGui.GetMousePos()}");

                ImGui.Checkbox("Demo Window", ref _showDemoWindow);                 // Edit bools storing our windows open/close state
                ImGui.Checkbox("Another Window", ref _showAnotherWindow);
                ImGui.Checkbox("Memory Editor", ref _showMemoryEditor);
                if (ImGui.Button("Button"))                                         // Buttons return true when clicked (NB: most widgets return true when edited/activated)
                    _counter++;
                ImGui.SameLine(0, -1);
                ImGui.Text($"counter = {_counter}");

                ImGui.DragInt("Draggable Int", ref _dragInt);

                float framerate = ImGui.GetIO().Framerate;
                ImGui.Text($"Application average {1000.0f / framerate:0.##} ms/frame ({framerate:0.#} FPS)");
            }

            // 2. Show another simple window. In most cases you will use an explicit Begin/End pair to name your windows.
            if (_showAnotherWindow)
            {
                ImGui.Begin("Another Window", ref _showAnotherWindow);
                ImGui.Text("Hello from another window!");
                if (ImGui.Button("Close Me"))
                    _showAnotherWindow = false;
                ImGui.End();
            }

            // 3. Show the ImGui demo window. Most of the sample code is in ImGui.ShowDemoWindow(). Read its code to learn more about Dear ImGui!
            if (_showDemoWindow)
            {
                // Normally user code doesn't need/want to call this because positions are saved in .ini file anyway.
                // Here we just want to make the demo initial state a bit more friendly!
                ImGui.SetNextWindowPos(new Vector2(650, 20), ImGuiCond.FirstUseEver);
                ImGui.ShowDemoWindow(ref _showDemoWindow);
            }

            if (ImGui.TreeNode("Tabs"))
            {
                if (ImGui.TreeNode("Basic"))
                {
                    ImGuiTabBarFlags tab_bar_flags = ImGuiTabBarFlags.None;
                    if (ImGui.BeginTabBar("MyTabBar", tab_bar_flags))
                    {
                        if (ImGui.BeginTabItem("Avocado"))
                        {
                            ImGui.Text("This is the Avocado tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Broccoli"))
                        {
                            ImGui.Text("This is the Broccoli tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        if (ImGui.BeginTabItem("Cucumber"))
                        {
                            ImGui.Text("This is the Cucumber tab!\nblah blah blah blah blah");
                            ImGui.EndTabItem();
                        }
                        ImGui.EndTabBar();
                    }
                    ImGui.Separator();
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Advanced & Close Button"))
                {
                    // Expose a couple of the available flags. In most cases you may just call BeginTabBar() with no flags (0).
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_Reorderable", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.Reorderable);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_AutoSelectNewTabs", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.AutoSelectNewTabs);
                    ImGui.CheckboxFlags("ImGuiTabBarFlags_NoCloseWithMiddleMouseButton", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.NoCloseWithMiddleMouseButton);
                    if ((s_tab_bar_flags & (uint)ImGuiTabBarFlags.FittingPolicyMask) == 0)
                        s_tab_bar_flags |= (uint)ImGuiTabBarFlags.FittingPolicyDefault;
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyResizeDown", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.FittingPolicyResizeDown))
                        s_tab_bar_flags &= ~((uint)ImGuiTabBarFlags.FittingPolicyMask ^ (uint)ImGuiTabBarFlags.FittingPolicyResizeDown);
                    if (ImGui.CheckboxFlags("ImGuiTabBarFlags_FittingPolicyScroll", ref s_tab_bar_flags, (uint)ImGuiTabBarFlags.FittingPolicyScroll))
                        s_tab_bar_flags &= ~((uint)ImGuiTabBarFlags.FittingPolicyMask ^ (uint)ImGuiTabBarFlags.FittingPolicyScroll);

                    // Tab Bar
                    string[] names = { "Artichoke", "Beetroot", "Celery", "Daikon" };

                    for (int n = 0; n < s_opened.Length; n++)
                    {
                        if (n > 0) { ImGui.SameLine(); }
                        ImGui.Checkbox(names[n], ref s_opened[n]);
                    }

                    // Passing a bool* to BeginTabItem() is similar to passing one to Begin(): the underlying bool will be set to false when the tab is closed.
                    if (ImGui.BeginTabBar("MyTabBar", (ImGuiTabBarFlags)s_tab_bar_flags))
                    {
                        for (int n = 0; n < s_opened.Length; n++)
                            if (s_opened[n] && ImGui.BeginTabItem(names[n], ref s_opened[n]))
                            {
                                ImGui.Text($"This is the {names[n]} tab!");
                                if ((n & 1) != 0)
                                    ImGui.Text("I am an odd tab.");
                                ImGui.EndTabItem();
                            }
                        ImGui.EndTabBar();
                    }
                    ImGui.Separator();
                    ImGui.TreePop();
                }
                ImGui.TreePop();
            }

        }
    }
}
