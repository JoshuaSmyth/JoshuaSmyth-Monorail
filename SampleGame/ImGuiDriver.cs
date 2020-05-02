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
        private byte[] _vertexData;
        private int _vertexBufferId;        // TODO Abstract this
        private int _vertexBufferSize;

        private byte[] _indexData;
        private int _indexBufferId;         // TODO Abstrat this
        private int _indexBufferSize;

        private IntPtr? _fontTextureId;

        private ResourceManager m_ResourceManager;

        private ShaderProgram m_ShaderProgram;
        private VertexBuffer<VertexImGui> m_VertexBuffer;
        private IndexBuffer m_IndexBuffer;

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
                               uniform ProjectionMatrixBuffer
                               {
                                   mat4 projection_matrix;
                               };

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

        public void Initalise(ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;

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

            // Load Shaders
            {
                m_ShaderProgram = m_ResourceManager.LoadShaderFromString(this.vertShaderCode, this.fragShaderCode);


            }

            // Load Default vertex and index buffers
            {
                // 65535
                throw new Exception("TODO Create index and vertex buffers via the resource manager!!!");
            }


            // TODO Add OpenGL ScissorRect
            // void glScissor(GLint x​, GLint y​, GLsizei width​, GLsizei height​);

            SetKeyMappings();
            BuildFontAtlas();
            CreateTexture();
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

            // Create and register the texture as an XNA texture
            var tex2d = Texture2D.Create(pixels, width, height, bytesPerPixel);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (_fontTextureId.HasValue)
            {
                throw new NotImplementedException("Implement font rebuild");
                //UnbindTexture(_fontTextureId.Value);
            }

            _fontTextureId = (IntPtr) tex2d.TextureId;

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

        public void End()
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
            if (drawData.TotalVtxCount > _vertexBufferSize)
            {
                // TODO resize vertex buffer
                //throw new Exception("TODO need a vertex buffer");
            }

            // Can we fit the indicies in the current index buffer
            if (drawData.TotalIdxCount > _indexBufferSize)
            {
                // TODO resize index buffer
                //throw new Exception("TODO need an index buffer");
            }

            return;
            // TODO I could probably use these pointers directly
            // Copy ImGui's vertices and indices to a set of managed byte arrays
            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &_vertexData[vtxOffset * 20])
                fixed (void* idxDstPtr = &_indexData[idxOffset * sizeof(ushort)])
                {
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, _vertexData.Length, cmdList.VtxBuffer.Size * VertexImGui.Stride);
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, _indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers

            
            //_vertexBuffer.SetData(_vertexData, 0, drawData.TotalVtxCount * VertexImGui.Stride);
            //_indexBuffer.SetData(_indexData, 0, drawData.TotalIdxCount * sizeof(ushort));

        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];
                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];
                    var texId = drawCmd.TextureId;
                }
            }
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
