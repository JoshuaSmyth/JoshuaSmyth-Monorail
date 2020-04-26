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

        public Cube()
        {

        }

        public override void OnApplyUniforms(RenderQueue renderQueue, GameCamera camera)
        {
            //throw new NotImplementedException();
        }
    }
}
