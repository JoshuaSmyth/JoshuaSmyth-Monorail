using System;
using Monorail.Framework.Services.Async;
using Monorail.Framework.Services.ServiceLocation;

namespace Monorail.Platform
{
    public class Game
    {
        public IPlatformGraphicsDevice GraphicsDevice;
        public IPlatformAudioDevice Audio;
        public GameWindow Window;
        public Coroutines Coroutines;
        public GameInput Input;
        public ResourceManager m_ResourceManager;

        public Game()
        {
            Coroutines = new Coroutines();
            DependancyLocator.AddDepenancy(Coroutines);

            m_ResourceManager = new ResourceManager();
        }

        internal void Init(IPlatformAudioDevice audioDevice,
                           IPlatformGraphicsDevice graphicsDevice,
                           GameWindow window,
                           GameInput input)
        {
            Window = window;
            Audio = audioDevice;
            GraphicsDevice = graphicsDevice;
            Input = input;

            DependancyLocator.AddDepenancy(Window);
            DependancyLocator.AddDepenancy(Audio);
            DependancyLocator.AddDepenancy(GraphicsDevice);
            DependancyLocator.AddDepenancy(Input);
        }

        public virtual void Update()
        {

        }

        public virtual void RenderScene()
        {

        }

        public virtual void Render2D()
        {

        }

        public virtual void Load()
        {

        }

        internal void UpdateCoroutines()
        {
            Coroutines.Update((int)GameTime.ElapsedMilliseconds);
        }
    }
}
