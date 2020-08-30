using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame
{
    public class Maploader
    {
        public static int width = 16;
        public static int height = 32;

        public int[,] cubes;

        public void Load()
        {
            cubes = new int[width, height];

            var txt = File.ReadAllText("Resources/Maps/testmap1.txt");

            int k = 0;
            for(int i=0;i<height;i++)
            {
                for(int j=0;j< width; j++)
                {
                    var height = (int) txt[k + 1];

                    cubes[j, i] = height - 48;


                     k += 2;
                }

                // Skip new lines
                k += 2;
            }

        }
    }
}
