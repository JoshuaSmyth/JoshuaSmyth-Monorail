using Monorail.Mathlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Monorail.Graphics
{
    public interface IInterleavedVertex
    {

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPosition : IInterleavedVertex
    {
        public Vector3 Position;

        public static int Stride = 12;

        public static int[] AttributeLengths { get { return new[] { 3 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0 }; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColor : IInterleavedVertex
    {
        public Vector3 Position;

        public Vector3 Color;

        public static int Stride = 24;
        
        public static int[] AttributeLengths { get { return new[] { 3, 3 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0, 12 }; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTexture : IInterleavedVertex
    {
        public Vector3 Position;

        public Vector3 Color;

        public Vector2 Texture;

        public static int Stride = 32;

        public static int[] AttributeLengths { get { return new[] { 3, 3, 2 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0, 12, 24 }; } }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTextureNormal : IInterleavedVertex
    {
        public Vector3 Position;

        public Vector3 Color;

        public Vector2 Texture;

        public Vector3 Normal;

        public static int Stride = 44;

        public static int[] AttributeLengths { get { return new[] { 3, 3, 2, 3 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0, 12, 24, 32 }; } }
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct DefaultQuadBatchVertex : IInterleavedVertex
    {
        public Vector3 Position;

        public Vector3 Color;

        public Vector2 Texture;

        public static int Stride = 32;

        public static int[] AttributeLengths { get { return new[] { 3, 3, 2 }; } }

        public static int[] AttributeOffsets { get { return new[] { 0, 12, 24 }; } }
    }
}
