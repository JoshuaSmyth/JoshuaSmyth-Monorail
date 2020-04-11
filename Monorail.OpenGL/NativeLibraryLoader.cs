using System;
using System.Runtime.InteropServices;

namespace Monorail
{
    public static class NativeLibraryLoader
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
        
        private static bool _dllDirectorySet = false;
        
        public static void SetNativeLibaryFolder()
        {
            if (!_dllDirectorySet)
            {
                string executingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (Environment.Is64BitProcess)
                {
                    NativeLibraryLoader.SetDllDirectory(System.IO.Path.Combine(executingDirectory, "lib/x64"));
                }
                else
                {
                    NativeLibraryLoader.SetDllDirectory(System.IO.Path.Combine(executingDirectory, "lib/x86"));
                }
                
                _dllDirectorySet = true;
            }
        }
    }
}
