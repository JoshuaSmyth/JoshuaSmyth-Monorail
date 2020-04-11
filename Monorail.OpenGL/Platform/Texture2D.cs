using OpenGL;
using SDL2;
using System;
using System.Runtime.InteropServices;

namespace Monorail.Platform
{

    public class Texture2D
    {
        private int m_TextureId;
        public int TextureId {  get { return m_TextureId; } }

        public static Texture2D CreateFromFile(string filename, bool generateMipmaps=false)
        {
            IntPtr intPtr = SDL2.SDL_image.IMG_Load(filename);
            var surface = Marshal.PtrToStructure<SDL.SDL_Surface>(intPtr);

            var pixelFormat = Marshal.PtrToStructure<SDL.SDL_PixelFormat>(surface.format);

            var mode = OpenGL.PixelFormat.Rgba;
            if (pixelFormat.BytesPerPixel == 3)
            {
                mode = OpenGL.PixelFormat.Rgb;
            }

            var rv = new Texture2D();

            // TODO Pass in wraping styles
            GlBindings.GenTextures(1, out rv.m_TextureId);
            GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureId);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

            GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, OpenGL.PixelInternalFormat.Rgba, surface.w, surface.h, 0, mode, PixelType.UnsignedByte, surface.pixels);
       
            if (generateMipmaps)
            {
                GlBindings.GenerateMipmap(TextureType.GL_TEXTURE_2D);
            }

            SDL2.SDL.SDL_free(intPtr);

            return rv;

            // TODO Load from SDL Image
            /*
            var bm = new Bitmap(Image.FromFile(filename, true));
            var data = BitmapToByteArray(bm);

            var internalPixelFormat = PixelInternalFormat.Rgb;
            var pixelFormat = OpenGL.PixelFormat.Rgb;

            switch(bm.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    internalPixelFormat = PixelInternalFormat.Rgba;
                    pixelFormat = OpenGL.PixelFormat.Rgba;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    internalPixelFormat = PixelInternalFormat.Rgb;
                    pixelFormat = OpenGL.PixelFormat.Rgb;
                    break;
                default:
                    throw new NotImplementedException("Pixel format not supported");
            }


            

            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            var intPtr = handle.AddrOfPinnedObject();
            
            var rv = new Texture2D();

            // TODO Pass in wraping styles
            GlBindings.GenTextures(1, out rv.m_TextureId);
            GlBindings.BindTexture(TextureType.GL_TEXTURE_2D, rv.m_TextureId);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_S, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_WRAP_T, TextureAttributeValue.GL_REPEAT);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MIN_FILTER, TextureAttributeValue.GL_LINEAR);
            GlBindings.TexParameteri(TextureType.GL_TEXTURE_2D, TextureAttribute.GL_TEXTURE_MAG_FILTER, TextureAttributeValue.GL_LINEAR);

            GlBindings.TexImage2D(TextureType.GL_TEXTURE_2D, 0, internalPixelFormat, bm.Width, bm.Height, 0, pixelFormat, PixelType.UnsignedByte, intPtr);
            handle.Free();

            if (generateMipmaps) {
                GlBindings.GenerateMipmap(TextureType.GL_TEXTURE_2D);
            }
            bm.Dispose();
            return rv;
            */
        }
        /*
        public static byte[] BitmapToByteArray(Bitmap bitmap)
        {

            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }

        }
        */
    }
}
