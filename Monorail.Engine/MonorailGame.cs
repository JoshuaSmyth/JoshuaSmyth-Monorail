using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Monorail
{
    public class MonorailGame
    {
        // Note:(Joshua) We need to copy all the dependancies of this dll as well as
        //               the dll itself otherwise it will fail to load.

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void ProgressCallback(int value);

        [DllImport("Monorail.Native.Graphics.dll", CallingConvention= CallingConvention.Cdecl)]
        private static extern int Init(int a);

        [DllImport("Monorail.Native.Graphics.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Update();

        [DllImport("Monorail.Native.Graphics.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Render();

        [DllImport("Monorail.Native.Graphics.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void RegisterCallback(ProgressCallback callback);

        private bool IsRunning = true;

        public virtual void Update(float dt)
        {
            if (IsRunning)
            {
                Update();
            }
        }

        public virtual void Render(float dt)
        {
            if (IsRunning)
            {
                Render();
            }
        }

        public void Start()
        {
            Console.WriteLine("Hello. World! From CSharp");

            ProgressCallback callback =
            (value) =>
            {
                // TODO Process Input
                // This is a callback from C++
                Console.WriteLine(value);
                if(value == 256)
                {
                    IsRunning = false;
                }
            };
            

            Init(1234);
            RegisterCallback(callback);

            Console.WriteLine("Launched Native Window!");

            while (IsRunning)
            {
                // TODO Get ticks from SDL

                Update(16); // TODO Pass ms as float

                Render(16);
            }
        }
    }
}
