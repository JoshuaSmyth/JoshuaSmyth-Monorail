using System;

namespace Monorail.Graphics
{
    [Flags]
    public enum ClearBufferMask
    {
        DepthBufferBit = 0x00000100,
        StencilBufferBit = 0x00000400,
        ColorBufferBit = 0x00004000,
    }

    public enum PrimitiveType
    {
        Lines = 0x0001,
        LineStrip = 0x0003,
        TriangleList = 0x0004,
        TriangleStrip = 0x0005,
    }

    public enum DrawElementsType
    {
        UnsignedShort = 0x1403,
        UnsignedInt = 0x1405,
    }
}
