using Monorail.Mathlib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monorail.Graphics
{
    public class Model
    {
        // TODO A model is made up of several meshes
        public VertexPositionColorTexture[] Verts;
        public uint[] Indicies;
    }

    public class ModelLoader
    {
        // TODO Refactor into a resources loader/manager class

        public static Model LoadObj(string fileName)
        {
            using (TracedStopwatch.Start("Load obj:" + fileName))
            {
                var rv = new Model();
                var text = File.ReadAllText(fileName);
                var items = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                int vertCount = 0;
                int faceCount = 0;

                // Need to know num verts/faces
                for (int i = 0; i < items.Length; i++)
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

                var verts = new Vector3[vertCount];
                var faces = new uint[faceCount * 3];

                // Bunny.obj
                // # vertex count = 2503
                // # face count = 4968
                var fIndex = 0;
                var vIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].StartsWith("v"))
                    {
                        var vsplit = items[i].Split(new char[] { ' ' });
                        verts[vIndex].X = float.Parse(vsplit[1], CultureInfo.InvariantCulture);
                        verts[vIndex].Y = float.Parse(vsplit[2], CultureInfo.InvariantCulture);
                        verts[vIndex].Z = float.Parse(vsplit[3], CultureInfo.InvariantCulture);

                        vIndex++;
                    }
                    else if (items[i].StartsWith("f"))
                    {
                        var vsplit = items[i].Split(new char[] { ' ' });
                        faces[fIndex + 0] = uint.Parse(vsplit[1], CultureInfo.InvariantCulture);
                        faces[fIndex + 1] = uint.Parse(vsplit[2], CultureInfo.InvariantCulture);
                        faces[fIndex + 2] = uint.Parse(vsplit[3], CultureInfo.InvariantCulture);

                        fIndex += 3;
                    }
                }

                rv.Verts = new VertexPositionColorTexture[vertCount];
                for (int i = 0; i < vertCount; i++)
                {
                    rv.Verts[i].Position = verts[i];
                    rv.Verts[i].Color = new Vector3(1, 1, 1);
                }

                rv.Indicies = faces;
                return rv;
            }
        }
    }
}
