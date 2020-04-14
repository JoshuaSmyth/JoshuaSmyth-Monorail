using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monorail
{
    public class TracedStopwatch : IDisposable
    {
        public Stopwatch Stopwatch;
        public string Name;
        
        public static TracedStopwatch Start(string name)
        {
            var rv = new TracedStopwatch();
            rv.Name = name;
            rv.Stopwatch = Stopwatch.StartNew();
            return rv;
        }

        public void Dispose()
        {
            Console.WriteLine(String.Format("{0}: {1}ms", Name, Stopwatch.ElapsedMilliseconds));
        }
    }
}
