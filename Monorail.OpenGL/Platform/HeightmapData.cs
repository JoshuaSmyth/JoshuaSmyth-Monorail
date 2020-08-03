using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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

}
