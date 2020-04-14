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
                var items = text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

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
                var faces = new Vector3[faceCount];

                // Bunny.obj
                // # vertex count = 2503
                // # face count = 4968
                var fIndex = 0;
                var vIndex = 0;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].StartsWith("nv"))
                    {
                        // Skip for now
                    }
                    else if (items[i].StartsWith("v"))
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
                        faces[fIndex].X = uint.Parse(vsplit[1], CultureInfo.InvariantCulture);
                        faces[fIndex].Y = uint.Parse(vsplit[2], CultureInfo.InvariantCulture);
                        faces[fIndex].Z = uint.Parse(vsplit[3], CultureInfo.InvariantCulture);

                        fIndex++;
                    }
                }

                rv.Verts = new VertexPositionColorTexture[faceCount*3];
                for (int i = 0; i < faceCount; i+=3)
                {
                    var face = faces[i/3];                    
                    rv.Verts[i].Position = verts[(int)face.X];
                    rv.Verts[i].Color = new Vector3(1, 1, 1);
                    rv.Verts[i].Texture = new Vector2(1, 1);
                    
                    rv.Verts[i + 1].Position = verts[(int)face.Y];
                    rv.Verts[i + 1].Color = new Vector3(1, 1, 1);
                    rv.Verts[i + 1].Texture = new Vector2(1, 1);
                    
                    rv.Verts[i + 2].Position = verts[(int)face.Z];
                    rv.Verts[i + 2].Color = new Vector3(1, 1, 1);
                    rv.Verts[i + 2].Texture = new Vector2(1, 1);
                }

                rv.Indicies = null;
                return rv;
            }
        }
    }
}
