using Monorail.Platform;

namespace SampleGame
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // TODO:(Joshua) NEXT: Implement a camera
            // https://learnopengl.com/#!Getting-started/Camera

            using (var window = new GameWindow("Test", 1280, 720))
            {
                window.RunGame(new MySampleGame());
            }
        }
    }
}
