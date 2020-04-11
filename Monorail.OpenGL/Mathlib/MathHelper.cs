using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail.Mathlib
{
    public static class MathHelper
    {
        public static float ToDeg(double rads)
        {
            return (float) (rads * (180 / Math.PI));
        }

        public static float ToRads(double deg)
        {
            return (float) (deg * (Math.PI / 180));
        }
    }
}
