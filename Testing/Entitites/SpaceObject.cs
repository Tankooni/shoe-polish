using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Punk
{
    public class SpaceObject : Entity
    {
        uint rightBound = 0;
        uint bottomBound = 0;
        public float health = 100;

        public SpaceObject(float x, float y, uint right, uint bottom)
            :base(x, y)
        {
            rightBound = right-Sector.SectorSize;
            bottomBound = bottom - Sector.SectorSize;
        }

        public override void Update()
        {
            base.Update();

            if (health <= 0)
                World.Remove(this);

            if (X > rightBound)
                X = 0;
            else if (X < 0)
                X = rightBound;
            if (Y > bottomBound)
                Y = 0;
            else if (Y < 0)
                Y = bottomBound;
        }

        public virtual void DoDamage(float damage)
        {
            health -= damage;
        }
    }
}
