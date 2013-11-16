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
        protected int size = 20;
        protected int sizeMod = 20;
        protected int posSizeMod = 20;
        private Sfx AsteroidHit = new Sfx(Library.GetBuffer("Collision.Wav"));

        public Asteroid(float x, float y, uint right, uint bottom)
            :base(x, y, right, bottom)
        {
            vel.X = FP.Rand(100) * FP.Choose(1, -1);
            vel.Y = FP.Rand(100) * FP.Choose(1, -1);
            rot = FP.Random * (float)90 * FP.Choose(1,-1);
            health += (((Height = Width = (sizeMod = (int)FP.Rand((uint)posSizeMod)) + size) - size) / posSizeMod) * health;
            var AsteroidImage = new Image(Library.GetTexture("Asteroid.png"));
            Graphic = image = AsteroidImage;
            CenterOrigin();
            image.CenterOO();
            Type = "Asteroid";
        }

        public override bool MoveCollideX(Entity e)
        {
            return Collided(e);
        }
        public override bool MoveCollideY(Entity e)
        {
            return Collided(e);
        }
        public bool Collided(Entity e)
        {
            AsteroidHit.Pitch = FP.Random*.5f+ .5f;
            AsteroidHit.Play();

            health -= (e as Testing.Part).health;
            if(health <= 0)
            {
                (e as Testing.Part).DoDamage(10 + sizeMod / posSizeMod);
                FP.Log((e as Testing.Part).health);
                return false;
            }
            
            return true;

        }
        public override void Update()
        {
            
            base.Update();
            MoveBy(vel.X * FP.Elapsed, vel.Y * FP.Elapsed, "Part");
            image.Angle += rot * FP.Elapsed;
        }
    }
}
