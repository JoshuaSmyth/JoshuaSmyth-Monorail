using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Monorail
{
    class Program
    {
        [DllImport("Monorail.Native.Graphics.dll")]
        private static extern int init();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello. World! From CSharp");

            init();

            Console.WriteLine("Launched Native Window!");


            while (true)
            {
                Console.ReadKey();
            }

        }
    }
}
