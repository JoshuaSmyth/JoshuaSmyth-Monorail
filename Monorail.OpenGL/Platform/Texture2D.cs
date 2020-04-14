using OpenGL;
using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Monorail.Platform
{
    // TODO Make a resource manager
    // TODO Implement .Dispose()
    public class Texture2D
    {
        private int m_TextureId;
        public int TextureId {  get { return m_TextureId; } }

        public static Texture2D CreateFromFile(string filename, bool generateMipmaps=false)
        {
            using (TracedStopwatch.Start("Loading Texture"))
            {
                IntPtr intPtr = SDL2.SDL_image.IMG_Load(filename);
                var surface = Marshal.PtrToStructure<SDL.SDL_Surface>(intPtr);

                var pixelFormat = Marshal.PtrToStructure<SDL.SDL_PixelFormat>(surface.format);

                var mode = OpenGL.PixelFormat.Rgba;
                if (pixelFormat.BytesPerPixel == 3)
                {
                    mode = OpenGL.PixelFormat.Rgb;
                }
                if (pixelFormat.BytesPerPixel == 1)
                {
                    mode = OpenGL.PixelFormat.Red;
                }

                var rv = new Texture2D();

                // TODO Pass in wraping styles
                GlBindings.GenTextures(1, out rv.m_TextureId);
                GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureId);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_REPEAT);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_REPEAT);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

                var internalFormat = OpenGL.PixelInternalFormat.Rgba;
                if (mode == PixelFormat.Luminance)
                {
                    internalFormat = PixelInternalFormat.Red;
                }

                GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, internalFormat, surface.w, surface.h, 0, mode, PixelType.UnsignedByte, surface.pixels);

                if (generateMipmaps)
                {
                    GlBindings.GenerateMipmap(TextureType.GL_TEXTURE_2D);
                }

                SDL2.SDL.SDL_free(intPtr);

                return rv;
            }
        }
    }
}
