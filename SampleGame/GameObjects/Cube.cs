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
    class Cube : RenderableObject
    {
        public Vector3 Position;

        public int i;

        private Matrix4 m_Model;
        
        public Cube(int shaderProgramId, int VaoId, int vertexCount)
        {
            this.ShaderId = shaderProgramId;
            this.VaoId = VaoId;
            this.VertexCount = vertexCount;

            this.HasIndexBuffer = false;
            this.CubemapTextureId = -1;
            this.DepthBufferEnabled = true;
        }

        public void Update(float rot)
        {
            var pos = Matrix4.CreateTranslation(Position);
            float angle = 20.0f * i;

            var rotAxis = new Vector3(1.0f, 0.0f, 1.0f);
            rotAxis = rotAxis.Normalize();
            m_Model = Matrix4.CreateRotation(rotAxis, MathHelper.ToRads(rot)) * pos;
        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            // TODO We are doing a lot of binds that do not need be be done everytime we
            // Render one of these cubes. So we should put some smarts into the render queue
            // at some stage to not needing to do all these rebinds.

            // Simple solution at some stage would be to create a renderObjectSet to inherit from

            // But also at some stage look at doing instanced rendering and dynamic/static batching...

            // This is once per set of this render object
            renderQueue.SetUniform(this.ShaderId, "texture1", 0);
            renderQueue.SetUniform(this.ShaderId, "texture2", 1);
            renderQueue.SetUniform(this.ShaderId, "view", camera.ViewMatrix);
            renderQueue.SetUniform(this.ShaderId, "proj", camera.ProjMatrix);

            // This is per instance of this render object
            renderQueue.SetUniform(this.ShaderId, "model", m_Model);
        }
    }
}
