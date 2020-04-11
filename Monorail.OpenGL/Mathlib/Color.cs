using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail.Mathlib
{
    public struct Color
    {
        public float R, G, B, A;
        
        public Color(float r, float g, float b, float a)
        {
            R = r; G = g; B = b; A = a;
        }

        public Color(float r, float g, float b)
        {
            R = r; G = g; B = b; A = 1.0f;
        }

        public Color(byte r, byte g, byte b)
        {
            R = r / 255.0f; G = g / 255.0f; B = b / 255.0f; A = 1.0f;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(R, G, B);
        }

        public Vector4 ToVector4()
        {
            return new Vector4(R, G, B, A);
        }
    }
}
