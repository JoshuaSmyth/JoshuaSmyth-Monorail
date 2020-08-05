using Monorail.Audio;
using System;
using System.Diagnostics;

using SDL2;

namespace Monorail.Platform
{
    public enum VSyncStatus
    {
        VSYNC_DISABLE = 0,
        VSYNC_ENABLE = 1
    }

    public class GameWindow : IDisposable
    {
        private static Int32 m_ScreenWidth;
        private static Int32 m_ScreenHeight;

        String m_WindowName;

        internal IntPtr WindowPtr;
        internal IntPtr OpenGLPtr;
        public IntPtr Win32Ptr;

        internal bool HasQuit;

        public static int ScreenHeight { get { return m_ScreenHeight; } set { m_ScreenHeight = value; } }

        public static int ScreenWidth { get { return m_ScreenWidth; } set { m_ScreenWidth = value; } }

        public static IPlatformGraphicsDevice GraphicsDevice { get; private set; }

        public static GameInput GameInput { get; private set; }

        public static AudioDevice AudioDevice { get; private set; } // TODO Create IAudioDevice

        internal static ShaderProgram QuadBatchShader { get; private set; } // TODO Createa a Shader database?

        internal static ShaderProgram FullScreenShader { get; set; }

        public GameWindow(String windowName, Int32 screenWidth, Int32 screenHeight)
        {
            m_WindowName = windowName;
            m_ScreenWidth = screenWidth;
            m_ScreenHeight = screenHeight;

            using (TracedStopwatch.Start("Init Stopwatch"))
            {
                // This is here just to init the traced stopwatch
            }
        }

        public void Dispose()
        {
            // TODO Tear down
        }

        public void RunGame(Game game, bool borderless = false, Action a = null)
        {
            NativeLibraryLoader.SetNativeLibaryFolder();

            
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

            var imgFlags = SDL_image.IMG_InitFlags.IMG_INIT_PNG | SDL_image.IMG_InitFlags.IMG_INIT_JPG;
            if ((SDL_image.IMG_Init(imgFlags) > 0 & imgFlags > 0) == false)
            {
                Console.WriteLine("SDL_image could not initialize! SDL_image Error: {0}", SDL.SDL_GetError());
                return;
            }



            var cpus = SDL.SDL_GetCPUCount();
            Console.WriteLine("SYSTEM: CPU Count:" + cpus);

            var ramMb = SDL.SDL_GetSystemRAM();
            Console.WriteLine("SYSTEM: RAM MB:" + ramMb);

            // Init OpenAL
            var audioDevice = new AudioDevice();
            audioDevice.Initalise();

            GameInput = new GameInput();

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK,SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);

            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLEBUFFERS, 1);
            SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 8);

            // TODO Register the Audio device in the dependancy locator
            var windowStyle = /*SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS |*/ SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL;
            if (borderless == true)
            {
                windowStyle = windowStyle | SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS;
            }
            WindowPtr = SDL.SDL_CreateWindow(m_WindowName, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, ScreenWidth, ScreenHeight, windowStyle);
            OpenGLPtr = SDL.SDL_GL_CreateContext(WindowPtr);

            // Get the Win32 HWND from the SDL2 window
            SDL.SDL_SysWMinfo info = new SDL.SDL_SysWMinfo();
            SDL.SDL_GetWindowWMInfo(WindowPtr, ref info);
            Win32Ptr = info.info.win.window;

            OpenGL.GlBindings.InitaliseOpenGLEntryPoints();

            

            var graphicsDevice = new OpenGLGraphicsDevice(OpenGLPtr);

            SetVSync(VSyncStatus.VSYNC_ENABLE);

            GraphicsDevice = graphicsDevice;
            AudioDevice = audioDevice;

            game.Init(audioDevice, graphicsDevice, this, GameInput);

            // TODO Rename to OnLoadedWindow
            if (a != null)
            {
                a.Invoke();
            }

            // TODO Embed these as resources
            QuadBatchShader = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.quadbatch.glsl", "Resources/Shaders/Fragment/f.quadbatch.glsl");
            FullScreenShader = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.fullscreenquad.glsl", "Resources/Shaders/Fragment/f.fullscreenquad.glsl");
            using (TracedStopwatch.Start("Loading Game Resources"))
            {
                game.Load();
            }

            var stopwatch = new Stopwatch();
            var isRunning = true;
            while (isRunning)
            {
                // TODO Reset stats counters

                stopwatch.Restart();
                ProcessEvents();
                
                if (this.HasQuit)
                {
                    break;
                }
                game.Update();
                game.UpdateCoroutines();


                GraphicsDevice.Enable(OpenGL.Enable.GL_DEPTH_TEST);
                game.RenderScene();

                GraphicsDevice.Disable(OpenGL.Enable.GL_DEPTH_TEST);
                game.Render2D();

                // TODO Multimedia Timer or sleep for time...
                // TODO Stats for frame time and fps

                // Update window with OpenGL rendering
                SDL.SDL_GL_SwapWindow(WindowPtr);
                GameInput.Swap();

                stopwatch.Stop();
                GameTime.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                GameTime.ElapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                GameTime.TotalSeconds += GameTime.ElapsedSeconds;
            }

            // Unload Resources
            SDL.SDL_Quit();
        }



        // TODO Move to GraphicsDevice?
        private static void SetVSync(VSyncStatus vsyncstatus)
        {
            SDL.SDL_GL_SetSwapInterval((int)vsyncstatus);
        }

        private void ProcessEvents()
        {
            SDL.SDL_Event sdlEvent;
            while (SDL.SDL_PollEvent(out sdlEvent) != 0)
            {
                if (sdlEvent.type == SDL.SDL_EventType.SDL_QUIT)
                {
                    HasQuit = true;
                }

                if (sdlEvent.type == SDL.SDL_EventType.SDL_WINDOWEVENT)
                {
                    if (sdlEvent.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                    {
                        // TODO Respond to resize event properly
                        var width = sdlEvent.window.data1;
                        var height = sdlEvent.window.data2;

                        Console.WriteLine("Resize Event: width:" + width + "height:" + height);

                        GameWindow.ScreenWidth = width;
                        GameWindow.ScreenHeight = height;

                        GraphicsDevice.SetViewport(0, 0, (uint)width, (uint)height);
                    }
                }

                if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYDOWN)
                {
                    var value = (int)sdlEvent.key.keysym.scancode;
                    GameInput.SetKeyDown(value);

                    switch (sdlEvent.key.keysym.sym)
                    {
                        case SDL.SDL_Keycode.SDLK_ESCAPE:
                        {
                            HasQuit = true;
                        }
                        break;
                    }
                }

                if (sdlEvent.type == SDL.SDL_EventType.SDL_KEYUP)
                {
                    GameInput.SetKeyUp((int)sdlEvent.key.keysym.scancode);
                }
            }
        }
    }
}
