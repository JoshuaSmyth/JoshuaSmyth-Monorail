using Monorail.Graphics;
using Monorail.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame.GameObjects
{
    public class Terrain : RenderableObject
    {
        
        public Terrain(int shaderProgramId, int vertexArrayObjectId, int vertexCount)
        {
            this.ShaderId = shaderProgramId;
            this.VaoId = vertexArrayObjectId;
            this.VertexCount = vertexCount;

            this.HasIndexBuffer = true;
            this.DepthBufferEnabled = true;
        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            renderQueue.SetUniform(this.ShaderId, "model", camera.WorldMatrix);
        }
    }
}
