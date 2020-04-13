using Monorail.Audio;
using System;
using System.Diagnostics;

using SDL2;

namespace Monorail.Platform
{

    public class GameWindow : IDisposable
    {
        private static Int32 m_ScreenWidth;
        private static Int32 m_ScreenHeight;
        String m_WindowName;
        internal IntPtr WindowPtr;
        internal IntPtr OpenGLPtr;

        internal bool HasQuit;

        public static int ScreenHeight { get { return m_ScreenHeight; } }

        public static int ScreenWidth { get { return m_ScreenWidth; } }

        public static IPlatformGraphicsDevice GraphicsDevice { get; private set; }

        public static GameInput GameInput { get; private set; }

        public static AudioDevice AudioDevice { get; private set; } // TODO Create IAudioDevice

        internal static ShaderProgram QuadBatchShader { get; private set; } // TODO Createa a Shader database?

        public GameWindow(String windowName, Int32 screenWidth, Int32 screenHeight)
        {
            m_WindowName = windowName;
            m_ScreenWidth = screenWidth;
            m_ScreenHeight = screenHeight;
        }

        public void Dispose()
        {
            // TODO Tear down
        }

        public void RunGame(Game game)
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
            WindowPtr = SDL.SDL_CreateWindow(m_WindowName, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, ScreenWidth, ScreenHeight, SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL);
            OpenGLPtr = SDL.SDL_GL_CreateContext(WindowPtr);

            OpenGL.GlBindings.InitaliseOpenGLEntryPoints();

            

            var graphicsDevice = new OpenGLGraphicsDevice(OpenGLPtr);

            SetVSync(VSyncStatus.VSYNC_ENABLE);

            GraphicsDevice = graphicsDevice;
            AudioDevice = audioDevice;

            game.Init(audioDevice, graphicsDevice, this, GameInput);
            // TODO Embed these as resources
            QuadBatchShader = ShaderProgram.CreateFromFile("Resources/Shaders/Vertex/v.quadbatch.glsl", "Resources/Shaders/Fragment/f.quadbatch.glsl");
            game.Load();

            var stopwatch = new Stopwatch();
            var isRunning = true;
            while (isRunning)
            {
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

                // TODO Multimedia Timer

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

        public enum VSyncStatus
        {
            VSYNC_DISABLE = 0,
            VSYNC_ENABLE = 1
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
