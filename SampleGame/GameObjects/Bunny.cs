using Monorail;
using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame
{
    public class Bunny : RenderableObject
    {
        Matrix4 model;

        public Bunny(int shaderId, int vaoId, int vertexCount)
        {
            ShaderId = shaderId;
            VaoId = vaoId;
            VertexCount = vertexCount;

            HasIndexBuffer = true;
            CubemapTextureId = -1;
            TextureIdA = -1;
            TextureIdB = -1;
            DepthBufferEnabled = true;
        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            renderQueue.SetUniform(ShaderId, "model", model);
            renderQueue.SetUniform(ShaderId, "view", camera.ViewMatrix);
            renderQueue.SetUniform(ShaderId, "proj", camera.ProjMatrix);
            renderQueue.SetUniform(ShaderId, "lightdir", new Vector3(1, 1, 1));
            renderQueue.SetUniform(ShaderId, "viewpos", camera.Position);

        }

        float rot;
        public void Update(long elapsedMilliseconds)
        {
            rot += (float)(GameTime.ElapsedSeconds * 15.0f);
            var scalefactor = (float)(12.0f * Math.Sin(rot * 0.1f) * Math.Sin(rot * 0.1f) + 2);

            var rotAxis = new Vector3(0.0f, 1.0f, 0.0f);
            rotAxis = rotAxis.Normalize();
            var r = Matrix4.CreateRotation(rotAxis, MathHelper.ToRads(rot * 16));

            model = Matrix4.CreateScaling(new Vector3(scalefactor, scalefactor, scalefactor)) * r;
        }
    }
}
