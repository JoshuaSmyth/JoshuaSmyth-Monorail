using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail.Mathlib
{
    public struct Rect
    {
        int X { get; set;  }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
