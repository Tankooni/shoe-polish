using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;

namespace Punk
{
    class Asteroid : SpaceObject
    {
        protected Image image;
        public SFML.Window.Vector2f vel;
        public float rot;

        public Asteroid(float x, float y, uint right, uint bottom)
            :base(x, y, right, bottom)
        {
            vel.X = FP.Rand(100) * FP.Choose(1, -1);
            vel.Y = FP.Rand(100) * FP.Choose(1, -1);
            rot = FP.Random * (float)Math.PI * FP.Choose(1,-1);
            Height = Width = (int)FP.Rand(20) + 20;

            Graphic = image = Image.CreateRect((uint)Width, (uint)Height, FP.Color(FP.Rand(0xFFFFFF)));
            CenterOrigin();
            image.CenterOO();
        }

        public override void Update()
        {
            base.Update();
            MoveBy(vel.X * FP.Elapsed, vel.Y * FP.Elapsed);
            image.Angle += rot * FP.Elapsed;
        }
    }
}
