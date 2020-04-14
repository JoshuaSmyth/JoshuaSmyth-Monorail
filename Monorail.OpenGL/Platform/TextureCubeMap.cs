using OpenGL;
using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Monorail.Platform
{
    public class TextureCubeMap
    {
        private int m_TextureId;
        public int TextureId { get { return m_TextureId; } }

        public static TextureCubeMap CreateFromFile(string front, string back, string bottom, string top, string left, string right, bool generateMipmaps = false)
        {
            using (TracedStopwatch.Start("Loading Cubemap: " + front))
            {
                var rv = new TextureCubeMap();

                // Generate the cube map
                GlBindings.GenTextures(1, out rv.m_TextureId);
                GlBindings.BindTexture(TextureType.GL_TEXTURE_CUBE_MAP, rv.m_TextureId);

                var items = new List<string>
            {
                right,
                left,
                top,
                bottom,
                front,
                back
            };
                var i = 0;
                foreach (var item in items)
                {
                    var fi = new FileInfo(item);
                    if (!fi.Exists)
                    {
                        throw new Exception("File not found");
                    }

                    // Load each image from disk
                    IntPtr intPtr = SDL2.SDL_image.IMG_Load(item);
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


                    var internalFormat = OpenGL.PixelInternalFormat.Rgba;
                    if (mode == PixelFormat.Luminance)
                    {
                        internalFormat = PixelInternalFormat.Red;
                    }

                    GlBindings.TexImage2D(TextureType.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, internalFormat, surface.w, surface.h, 0, mode, PixelType.UnsignedByte, surface.pixels);

                    // Free the managed resource
                    SDL2.SDL.SDL_free(intPtr);
                    i++;
                }

                // Texture Sampling and Filtering
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_CUBE_MAP, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_CLAMP_TO_EDGE);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_CUBE_MAP, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_CLAMP_TO_EDGE);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_CUBE_MAP, TextureAttribute.GL_TEXTURE_WRAP_R, TextureAttributeValue.GL_CLAMP_TO_EDGE);

                GlBindings.TexParameteri(TextureType.GL_TEXTURE_CUBE_MAP, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_CUBE_MAP, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

                return rv;
            }
        }
    }
}
