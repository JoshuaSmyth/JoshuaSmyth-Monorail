using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail
{
    public static class GameTime
    {
        public static long ElapsedMilliseconds { internal set; get; }

        public static double ElapsedSeconds { internal set; get; }

        public static double TotalSeconds { internal set; get; }    // TODO:(Joshua) FIXME: Probably shouldn't be a double
    }
}
