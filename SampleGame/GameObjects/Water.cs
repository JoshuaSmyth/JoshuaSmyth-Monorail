using Monorail.Graphics;
using Monorail.Mathlib;
using Monorail.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame.GameObjects
{
    public class Water : RenderableObject
    {
        public Water(int shaderProgramId, int vertexArrayObjectId, int vertexCount)
        {
            // There are some really dumb defaults
            // So we should seperate the data in the interface and the methods of the interface
            // and provide a RenderPropertiesBase class!
            this.ShaderId = shaderProgramId;
            this.VaoId = vertexArrayObjectId;
            this.VertexCount = vertexCount;

            this.HasIndexBuffer = true;
            this.BlendMode = BlendModes.Alpha;
            this.DepthBufferEnabled = true; //?
            this.CubemapTextureId = 0;

        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            renderQueue.SetUniform(this.ShaderId, "model", Matrix4.Identity);
            renderQueue.SetUniform(this.ShaderId, "view", camera.ViewMatrix);
            renderQueue.SetUniform(this.ShaderId, "proj", camera.ProjMatrix);
        }
    }
}
