using Monorail.Graphics;
using Monorail.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGame
{
    public class SkyBox : RenderableObject
    {
        public SkyBox(int shaderId, int vaoId, int vertexCount, uint cubeMapId)
        {
            HasIndexBuffer = false;

            ShaderId = shaderId;
            VaoId = vaoId;
            VertexCount = vertexCount;
            CubemapTextureId = cubeMapId;

            // TODO
            TextureIdA = 0;
            TextureIdB = 0;
            DepthBufferEnabled = false;
        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            renderQueue.SetUniform(ShaderId, "view", camera.GetLookAtMatrix());
            renderQueue.SetUniform(ShaderId, "proj", camera.ProjMatrix);
            renderQueue.SetUniform(ShaderId, "skybox", 0);
        }
    }
}
