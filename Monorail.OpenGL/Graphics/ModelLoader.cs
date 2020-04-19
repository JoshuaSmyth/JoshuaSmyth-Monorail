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

        public static Model CreatePlane(int widthX, int widthZ, float height)
        {
            var rv = new Model();
            
            var verts = ModelLoader.CreatePlaneVerts(widthX, widthZ, height);

            uint[] indices = ModelLoader.CreateIndexedQuadIndicies(widthX, widthZ);

            rv.Verts = verts;
            rv.Indicies = indices;

            return rv;
        }

        public static Model CreateTerrain(HeightMapData data)
        {
            var rv = new Model();
            
            var width = data.Width;
            var height = data.Height;

            var verts = ModelLoader.CreateTerrainVerticies(data.Data, width, height);

            uint[] indices = ModelLoader.CreateIndexedQuadIndicies(width, height);

            rv.Verts = verts;
            rv.Indicies = indices;

            return rv;
        }

        private static uint[] CreateIndexedQuadIndicies(int width, int height)
        {
            var tileCount = width * height;
            var rv = new uint[tileCount * 6];

            var offset = 0;
            for(int i=0; i<tileCount*6; i+=6)
            {
                rv[i + 0] = (uint)(0 + offset);
                rv[i + 1] = (uint)(1 + offset);
                rv[i + 2] = (uint)(3 + offset);
                rv[i + 3] = (uint)(1 + offset);
                rv[i + 4] = (uint)(2 + offset);
                rv[i + 5] = (uint)(3 + offset);

                offset += 4;
            }

            return rv;
        }

        private static VertexPositionColorTextureNormal[] CreatePlaneVerts(int widthX, int widthZ, float height)
        {
            var scale = 1.0f;
            var vertexCount = 4 * widthX * widthZ;

            var rv = new VertexPositionColorTextureNormal[vertexCount];
            var tileWidthX = 1.0f;
            var tileWidthZ = 1.0f;

            var i = 0;
            var h = 0;
            for (int x = 0; x < widthX; x++)
                for (int z = 0; z < widthZ; z++)
                {
                    var dx = tileWidthX * x;
                    var dz = tileWidthZ * z;
                    
                    var yHeight = height;

                    rv[i + 0].Position = new Vector3(dx + 0.5f, yHeight, dz + 0.5f);     // Top Right
                    rv[i + 1].Position = new Vector3(dx + 0.5f, yHeight, dz + -0.5f);    // Bottom Right
                    rv[i + 2].Position = new Vector3(dx + -0.5f, yHeight, dz + -0.5f);   // Bottom Left
                    rv[i + 3].Position = new Vector3(dx + -0.5f, yHeight, dz + 0.5f);    // Top Left

                    rv[i + 0].Color = new Vector4(0f, 0f, 1.0f, 0.5f);
                    rv[i + 1].Color = new Vector4(0f, 0f, 1.0f, 0.5f);
                    rv[i + 2].Color = new Vector4(0f, 0f, 1.0f, 0.5f);
                    rv[i + 3].Color = new Vector4(0f, 0.0f, 1.0f, 0.5f);

                    rv[i + 0].Texture = new Vector2(1f, 1f);
                    rv[i + 1].Texture = new Vector2(1f, 0f);
                    rv[i + 2].Texture = new Vector2(0f, 0f);
                    rv[i + 3].Texture = new Vector2(0f, 1f);

                    
                    // Temp FIXME
                    rv[i + 0].Normal = new Vector3(0.0f, 0f, 1.0f);
                    rv[i + 1].Normal = new Vector3(0.0f, 0f, 1.0f);
                    rv[i + 2].Normal = new Vector3(0.0f, 0f, 1.0f);
                    rv[i + 3].Normal = new Vector3(0.0f, 0f, 1.0f);

                    i += 4;
                    h++;
                }

            return rv;
        }

        private static VertexPositionColorTextureNormal[] CreateTerrainVerticies(byte[] heights, int width, int height)
        {
            var vertexCount = 4 * width * height;

            var rv = new VertexPositionColorTextureNormal[vertexCount];
            var tileWidthX = 1.0f;
            var tileWidthZ = 1.0f;

            var tileCountX = width-1;
            var tileCountY = height - 1;

            var i = 0;
            for(int x=1; x < tileCountX; x++)
            for(int z=1; z < tileCountY; z++)
            {
                var dx = tileWidthX * x;
                var dz = tileWidthZ * z;

                    // TODO Work this out properly, SEEMS WE ARE FLIPPED A BIT

                var hIndex = x*width+z;


                    var heightA = (float)(heights[hIndex + width+1] / 256.0f) * 64.0f;
                    var heightB = (float)(heights[hIndex + width] / 256.0f) * 64.0f;
                    var heightC = (float)(heights[hIndex] / 256.0f) * 64.0f;
                    var heightD = (float)(heights[hIndex+1] / 256.0f) * 64.0f;
                    
                    rv[i + 0].Position = new Vector3(dx + 0.5f, heightA, dz + 0.5f);     // Top Right
                    
                    rv[i + 1].Position = new Vector3(dx + 0.5f, heightB, dz -0.5f);    // Bottom Right
                    rv[i + 2].Position = new Vector3(dx -0.5f, heightC, dz -0.5f);   // Bottom Left

                    rv[i + 3].Position = new Vector3(dx -0.5f, heightD, dz + 0.5f);    // Top Left

                rv[i + 0].Color = new Vector4(1f, 1f, 0.0f, 1.0f);
                rv[i + 1].Color = new Vector4(0.5f, 1f, 0.0f, 1.0f);
                rv[i + 2].Color = new Vector4(1f, 1f, 0.0f, 1.0f);
                rv[i + 3].Color = new Vector4(1f, 0.5f, 0.0f, 1.0f);

                rv[i + 0].Texture = new Vector2(1f, 1f);
                rv[i + 1].Texture = new Vector2(1f, 0f);
                rv[i + 2].Texture = new Vector2(0f, 0f);
                rv[i + 3].Texture = new Vector2(0f, 1f);

                // TODO Calculate normal correctly
                /*
                rv[i + 0].Normal = new Vector3(1.0f, 0f, 0.0f); // Red = topright
                rv[i + 1].Normal = new Vector3(0.0f, 1f, 0.0f); // Green = bottom right
                rv[i + 2].Normal = new Vector3(0.0f, 0f, 1.0f); // Blue = bottom left
                rv[i + 3].Normal = new Vector3(1.0f, 1f, 1.0f); // white = top left
                */
                i += 4;
            }


            // FaceA = 0, 1, 3
            // FaceB = 1, 2, 3

            // TODO Calculate Normals!
            for(int n=0; n<rv.Length-4; n++)
            {
                // Face A
                var v1 = rv[n + 0].Position;
                var v2 = rv[n + 1].Position;
                var v3 = rv[n + 3].Position;
                
                var e1 = v2 - v1;
                var e2 = v3 - v1;

                var normalA = Vector3.Cross(e1, e2);

                // Face B
                var v1b = rv[n + 1].Position;
                var v2b = rv[n + 2].Position;
                var v3b = rv[n + 3].Position;
                var e1b = v2b - v1b;
                var e2b = v3b - v1b;
                var normalB = Vector3.Cross(e1b, e2b);


                // Face normal B
                rv[n + 0].Normal += normalA;
                rv[n + 1].Normal += normalA + normalB;
                rv[n + 2].Normal += normalA + normalB;
                rv[n + 3].Normal += normalA + normalB;
            }


            for (int n = 0; n < rv.Length; n++)
            {
                rv[n].Normal = Vector3.Normalize(rv[n].Normal);
            }

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
                    rv.Verts[i].Color = new Vector4(1, 1, 1, 1);
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
