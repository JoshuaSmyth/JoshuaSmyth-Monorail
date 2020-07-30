using OpenGL;
using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Monorail.Platform
{

    public class HeightMapData
    {
        private int m_Width;
        private int m_Height;
        private byte[] m_Data;

        public int Width
        {
            get
            {
                return m_Width;
            }
        }

        public int Height
        {
            get
            {
                return m_Height;
            }
        }

        public byte[] Data
        {
            get
            {
                return m_Data;
            }
        }

        public void SetData(int width, int height, byte[] data)
        {
            m_Data = data;
            m_Width = width;
            m_Height = height;
        }

        public static HeightMapData LoadHeightmapData(string filename)
        {
            using (TracedStopwatch.Start("Loading Heightmap: " + filename))
            {
                var rv = new HeightMapData();

                IntPtr intPtr = SDL2.SDL_image.IMG_Load(filename);
                var surface = Marshal.PtrToStructure<SDL.SDL_Surface>(intPtr);
                var pixelFormat = Marshal.PtrToStructure<SDL.SDL_PixelFormat>(surface.format);
                if (pixelFormat.BytesPerPixel != 1)
                {
                    throw new Exception("Only 8bit textures support atm!");
                }

                var width = surface.w;
                var height = surface.h;
                var size = width * height;

                byte[] managedArray = new byte[size];
                Marshal.Copy(surface.pixels, managedArray, 0, size);

                rv.SetData(width, height, managedArray);
                
                return rv;
            }
        }
    }

    public class Texture2D
    {
        private int m_TextureId;
        public int TextureId {  get { return m_TextureId; } }


        internal static Texture2D Create(int width, int height, int bytesPerPixel)
        {
            byte[] bytearray = new byte[width * height * bytesPerPixel];

            // is there a faster way to create white?
            for(int i=0;i<bytearray.Length; i++)
            {
                bytearray[i] = 255;
            }

            GCHandle pinnedArray = GCHandle.Alloc(bytearray, GCHandleType.Pinned);
            IntPtr pixels = pinnedArray.AddrOfPinnedObject();
            
            var rv = new Texture2D();
            {

                GlBindings.GenTextures(1, out rv.m_TextureId);
                GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureId);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_REPEAT);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_REPEAT);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
                GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

                GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            }
            pinnedArray.Free();

            return rv;
        }

        internal static Texture2D Create(IntPtr pixels, int width, int height, int bytesPerPixel)
        {
            var rv = new Texture2D();

            GlBindings.GenTextures(1, out rv.m_TextureId);
            GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureId);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

            GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            return rv;
        }


        public void SetData(IntPtr pixels, int width, int height, int bytesPerPixel)
        {
            // TODO What if this texture is not bound?

            GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
        }

        internal static Texture2D CreateFromFile(string filename, bool generateMipmaps=false)
        {
            using (TracedStopwatch.Start("Loading Texture: " + filename))
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
