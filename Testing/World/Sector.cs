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
        public static uint SectorSize = 1024;

        public Sector()
        {
            //Graphic = background = Image.CreateRect(SectorSize, SectorSize, FP.Color(0x000000));
            Height = Width = (int)SectorSize;
            Graphic = background = new Image(Library.GetTexture("StarField1024.png"));
            //background.ScaleX = SectorSize / background.Width;
            //background.ScaleY = SectorSize / background.Height;
           
        }
        public override void Added()
        {
            base.Added();
            for (int x = 0; x < 5; x++ )
                World.Add(new Asteroid(FP.Rand(SectorSize) + X, FP.Rand(SectorSize) + Y, SpaceWorld.WorldLength, SpaceWorld.WorldHeight));
        }
        public override void Update()
        {
            base.Update();
            background.Color = FP.Color(FP.Rand(uint.MaxValue));
        }
    }
}
