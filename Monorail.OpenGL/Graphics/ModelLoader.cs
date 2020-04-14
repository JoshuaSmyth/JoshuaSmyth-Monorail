using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monorail.Graphics
{
    public class Model
    {
        // TODO A model is made up of several meshes
    }

    public class ModelLoader
    {
        // TODO Refactor into a resources loader/manager class

        public static VertexPositionColor[] LoadObj(string fileName)
        {
            var text = File.ReadAllText(fileName);
            var items = text.Split(new string[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            int vertCount = 0;
            int faceCount = 0;

            // Need to know num verts/faces
            for(int i=0; i<items.Length; i++)
            {
                if (items[i].StartsWith("v"))
                {
                    vertCount++;
                }
                else if (items[i].StartsWith("f"))
                {
                    faceCount++;
                }
            }

            // Bunny.obj
            // # vertex count = 2503
            // # face count = 4968

            // Need to know num faces


            return null;
        }
    }
}
