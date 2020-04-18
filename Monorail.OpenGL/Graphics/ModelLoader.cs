using Monorail.Mathlib;
using Monorail.Platform;
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
        public VertexPositionColorTextureNormal[] Verts;
        public uint[] Indicies;
    }

    public class ModelLoader
    {
        // TODO Refactor into a resources loader/manager class

        public static Model CreateTerrain(HeightMapData data)
        {
            var rv = new Model();
            var verts = Geometry.CreateIndexedQuadVerts(scale: 8.0f);
            uint[] indices = Geometry.CreateIndexedQuadIndicies();

            rv.Verts = verts;
            rv.Indicies = indices;

            return rv;
        }

        public static Model LoadObj(string fileName)
        {
            using (TracedStopwatch.Start("Load obj:" + fileName))
            {
                var rv = new Model();
                var text = File.ReadAllText(fileName);
                var items = text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);

                int vertCount = 0;
                int faceCount = 0;
                int normalCount = 0;

                // Need to know num verts/faces
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].StartsWith("vn"))
                    {
                        normalCount++;
                    }
                    else if (items[i].StartsWith("v"))
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
                    if (items[i].StartsWith("vn"))
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
                        faces[fIndex] = uint.Parse(vsplit[1], CultureInfo.InvariantCulture) - 1;
                        faces[fIndex+1] = uint.Parse(vsplit[2], CultureInfo.InvariantCulture) - 1;
                        faces[fIndex+2] = uint.Parse(vsplit[3], CultureInfo.InvariantCulture) - 1;

                        fIndex+=3;
                    }
                }

                // TODO Compute normals
             
                
                // Verticies

                rv.Verts = new VertexPositionColorTextureNormal[vertCount];
                for (int i = 0; i < vertCount; i++)
                {
                    var v1 = verts[i];
                    rv.Verts[i].Position = v1;
                    rv.Verts[i].Color = new Vector3(1, 1, 1);
                    rv.Verts[i].Texture = new Vector2(1, 1);
                }



                // Compute Face Normals
                for (int i = 0; i < faces.Length - 1; i+=3)
                {
                    var id1 = faces[i];
                    var id2 = faces[i+1];
                    var id3 = faces[i+2];

                    var v1 = verts[id1];
                    var v2 = verts[id2];
                    var v3 = verts[id3];

                    var e1 = v2 - v1;
                    var e2 = v3 - v1;

                    var n = Vector3.Cross(e1, e2);

                    // Smooth normals
                    rv.Verts[id1].Normal += n;
                    rv.Verts[id2].Normal += n;
                    rv.Verts[id3].Normal += n;

                    // Face Normals
                    /*
                        rv.Verts[id1].Normal += n;
                        rv.Verts[id2].Normal += n;
                        rv.Verts[id3].Normal += n;
                    */
                }

                for (int i=0;i<rv.Verts.Length;i++)
                {
                    rv.Verts[i].Normal = Vector3.Normalize(rv.Verts[i].Normal);
                }

                // Indicies
                rv.Indicies = faces;

                // If using no indicies
                // rv.Indicies

                return rv;
            }
        }
    }
}
