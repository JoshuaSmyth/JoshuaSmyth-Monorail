using Monorail.Graphics;
using Monorail.Mathlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monorail.Platform
{
    public static class Geometry
    {
        public static VertexPosition[] CreateSkyBox()
        {
            var rv = new VertexPosition[36];

            rv[0].Position = new Vector3(-1.0f, 1.0f, -1.0f);
            rv[1].Position = new Vector3(-1.0f, -1.0f, -1.0f);
            rv[2].Position = new Vector3(1.0f, -1.0f, -1.0f);
            rv[3].Position = new Vector3(1.0f, -1.0f, -1.0f);
            rv[4].Position = new Vector3(1.0f, 1.0f, -1.0f);
            rv[5].Position = new Vector3(-1.0f, 1.0f, -1.0f);

            rv[6].Position = new Vector3(-1.0f, -1.0f, 1.0f);
            rv[7].Position = new Vector3(-1.0f, -1.0f, -1.0f);
            rv[8].Position = new Vector3(-1.0f, 1.0f, -1.0f);
            rv[9].Position = new Vector3(-1.0f, 1.0f, -1.0f);
            rv[10].Position = new Vector3(-1.0f, 1.0f, 1.0f);
            rv[11].Position = new Vector3(-1.0f, -1.0f, 1.0f);

            rv[12].Position = new Vector3(1.0f, -1.0f, -1.0f);
            rv[13].Position = new Vector3(1.0f, -1.0f, 1.0f);
            rv[14].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[15].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[16].Position = new Vector3(1.0f, 1.0f, -1.0f);
            rv[17].Position = new Vector3(1.0f, -1.0f, -1.0f);

            rv[18].Position = new Vector3(-1.0f, -1.0f, 1.0f);
            rv[19].Position = new Vector3(-1.0f, 1.0f, 1.0f);
            rv[20].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[21].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[22].Position = new Vector3(1.0f, -1.0f, 1.0f);
            rv[23].Position = new Vector3(-1.0f, -1.0f, 1.0f);

            rv[24].Position = new Vector3(-1.0f, 1.0f, -1.0f);
            rv[25].Position = new Vector3(1.0f, 1.0f, -1.0f);
            rv[26].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[27].Position = new Vector3(1.0f, 1.0f, 1.0f);
            rv[28].Position = new Vector3(-1.0f, 1.0f, 1.0f);
            rv[29].Position = new Vector3(-1.0f, 1.0f, -1.0f);

            rv[30].Position = new Vector3(-1.0f, -1.0f, -1.0f);
            rv[31].Position = new Vector3(-1.0f, -1.0f, 1.0f);
            rv[32].Position = new Vector3(1.0f, -1.0f, -1.0f);
            rv[33].Position = new Vector3(1.0f, -1.0f, -1.0f);
            rv[34].Position = new Vector3(-1.0f, -1.0f, 1.0f);
            rv[35].Position = new Vector3(1.0f, -1.0f, 1.0f);

            return rv;
        }


        public static VertexPositionColorTexture[] CreateCube()
        {
            var rv = new VertexPositionColorTexture[36];

            for (int i = 0; i < 36; i++)
            {
                rv[i].Color = new Vector3(1.0f, 1.0f, 1.0f);
            }

            rv[0].Position = new Vector3(-0.5f, -0.5f, -0.5f);
            rv[1].Position = new Vector3(0.5f, -0.5f, -0.5f);
            rv[2].Position = new Vector3(0.5f, 0.5f, -0.5f);
            rv[3].Position = new Vector3(0.5f, 0.5f, -0.5f);
            rv[4].Position = new Vector3(-0.5f, 0.5f, -0.5f);
            rv[5].Position = new Vector3(-0.5f, -0.5f, -0.5f);

            rv[6].Position = new Vector3(-0.5f, -0.5f, 0.5f);
            rv[7].Position = new Vector3(0.5f, -0.5f, 0.5f);
            rv[8].Position = new Vector3(0.5f, 0.5f, 0.5f);
            rv[9].Position = new Vector3(0.5f, 0.5f, 0.5f);
            rv[10].Position = new Vector3(-0.5f, 0.5f, 0.5f);
            rv[11].Position = new Vector3(-0.5f, -0.5f, 0.5f);

            rv[12].Position = new Vector3(-0.5f, 0.5f, 0.5f);
            rv[13].Position = new Vector3(-0.5f, 0.5f, -0.5f);
            rv[14].Position = new Vector3(-0.5f, -0.5f, -0.5f);
            rv[15].Position = new Vector3(-0.5f, -0.5f, -0.5f);
            rv[16].Position = new Vector3(-0.5f, -0.5f, 0.5f);
            rv[17].Position = new Vector3(-0.5f, 0.5f, 0.5f);

            rv[18].Position = new Vector3(0.5f, 0.5f, 0.5f);
            rv[19].Position = new Vector3(0.5f, 0.5f, -0.5f);
            rv[20].Position = new Vector3(0.5f, -0.5f, -0.5f);
            rv[21].Position = new Vector3(0.5f, -0.5f, -0.5f);
            rv[22].Position = new Vector3(0.5f, -0.5f, 0.5f);
            rv[23].Position = new Vector3(0.5f, 0.5f, 0.5f);

            rv[24].Position = new Vector3(-0.5f, -0.5f, -0.5f);
            rv[25].Position = new Vector3(0.5f, -0.5f, -0.5f);
            rv[26].Position = new Vector3(0.5f, -0.5f, 0.5f);
            rv[27].Position = new Vector3(0.5f, -0.5f, 0.5f);
            rv[28].Position = new Vector3(-0.5f, -0.5f, 0.5f);
            rv[29].Position = new Vector3(-0.5f, -0.5f, -0.5f);

            rv[30].Position = new Vector3(-0.5f, 0.5f, -0.5f);
            rv[31].Position = new Vector3(0.5f, 0.5f, -0.5f);
            rv[32].Position = new Vector3(0.5f, 0.5f, 0.5f);
            rv[33].Position = new Vector3(0.5f, 0.5f, 0.5f);
            rv[34].Position = new Vector3(-0.5f, 0.5f, 0.5f);
            rv[35].Position = new Vector3(-0.5f, 0.5f, -0.5f);

            for (int j = 0; j < 6; j++)
            {
                rv[j * 6 + 0].Texture = new Vector2(0.0f, 0.0f);
                rv[j * 6 + 1].Texture = new Vector2(1.0f, 0.0f);
                rv[j * 6 + 2].Texture = new Vector2(1.0f, 1.0f);
                rv[j * 6 + 3].Texture = new Vector2(1.0f, 1.0f);
                rv[j * 6 + 4].Texture = new Vector2(0.0f, 1.0f);
                rv[j * 6 + 5].Texture = new Vector2(0.0f, 0.0f);
            }

            return rv;
        }

        public static uint[] CreateIndexedQuadIndicies()
        {
            return new uint[] { 0, 1, 3, 1, 2, 3 };
        }

        public static VertexPositionColorTextureNormal[] CreateIndexedQuadVerts(float scale=1.0f)
        {
            var rv = new VertexPositionColorTextureNormal[4];
            rv[0].Position = new Vector3(0.5f * scale, 0, 0.5f * scale);     // Top Right
            rv[1].Position = new Vector3(0.5f * scale, 0, -0.5f * scale);    // Bottom Right
            rv[2].Position = new Vector3(-0.5f * scale, 0, -0.5f * scale);   // Bottom Left
            rv[3].Position = new Vector3(-0.5f * scale, 0, 0.5f * scale);    // Top Left

            rv[0].Color = new Vector4(1f, 1f, 0.0f, 1.0f);
            rv[1].Color = new Vector4(0.5f, 1f, 0.0f, 1.0f);
            rv[2].Color = new Vector4(1f, 1f, 0.0f, 1.0f);
            rv[3].Color = new Vector4(1f, 0.5f, 0.0f, 1.0f);

            rv[0].Texture = new Vector2(1f * scale, 1f * scale);
            rv[1].Texture = new Vector2(1f * scale, 0f);
            rv[2].Texture = new Vector2(0f, 0f);
            rv[3].Texture = new Vector2(0f, 1f * scale);

            rv[0].Normal = new Vector3(0.0f, 1f, 0.0f);
            rv[1].Normal = new Vector3(0.0f, 1f, 0.0f);
            rv[2].Normal = new Vector3(0.0f, 1f, 0.0f);
            rv[3].Normal = new Vector3(0.0f, 1f, 0.0f);

            return rv;
        }
    }
}
