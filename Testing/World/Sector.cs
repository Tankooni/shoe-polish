using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;

namespace Punk
{
    class Sector : Entity
    {
        protected Image background;
        public static uint SectorSize = 1000;

        public Sector()
        {
            Graphic = background = Image.CreateRect(SectorSize, SectorSize, FP.Color(FP.Rand(uint.MaxValue)));
            Height = Width = (int)SectorSize;
           
        }
        public override void Added()
        {
            base.Added();
            for (int x = 0; x < 1; x++ )
                World.Add(new Asteroid(FP.Rand(SectorSize) + X, FP.Rand(SectorSize) + Y, SpaceWorld.WorldLength, SpaceWorld.WorldHeight));
        }
    }
}
