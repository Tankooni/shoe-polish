using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using Punk.Masks;
using Punk;

namespace Testing
{
    public class BaseShip : Entity
    {
        public Image ShipCenter;

        public List<PartBase> shipParts;

        public int mass, forwardThrust, leftThrust, rightThrust;

        public BaseShip()
        {
            shipParts = new List<PartBase>();
        }

        public override void Added()
        {
            base.Added();

            Type = "BaseShip";
            ShipCenter = Image.CreateRect(64, 64, FP.Color(0xffffff));

            ShipCenter.CenterOO();

            X = FP.HalfWidth;
            Y = FP.HalfHeight;
        }
    }
    public class EmptyShip : BaseShip
    {
        private Vector2f Velocity;
        private int created;
        public EmptyShip()
        {
            shipParts = new List<PartBase>();
            Velocity = new Vector2f(FP.Rand(2), FP.Rand(2));
            created = 0;
        }

        public override void Added()
        {
            base.Added();
            World.Add(new Part(this, 1, 0, 0));
            Type = "EmptyShip";
            ShipCenter = Image.CreateRect(64, 64, FP.Color(0xffffff));

            ShipCenter.CenterOO();

            //for (int i = 0; i < FP.Rand(8); i++)
            //{
            //    for (int j = 1; j < FP.Rand(8); j++)
            //    {
            //        World.Add(new Part(this, Convert.ToInt32(FP.Rand(3)) + 1, i, j));
            //    }
            //}

            X = FP.Rand(SpaceWorld.WorldLength);
            Y = FP.Rand(SpaceWorld.WorldHeight);
        }

        public override void Update()
        {
            base.Update();

            if (created == 2)
            {
                for (int i = 0; i < shipParts.Count; i++)
                {
                    if (shipParts[i] as EmptyPart != null)
                    {
                        World.Add(new Part(this, (int)(FP.Rand(2) + 1), shipParts[i].curCol, shipParts[i].curRow));
                    }
                }
                created = 3;
            }
            else if ( created < 2)
            {
                created++;
            }

            //X += Velocity.X;
            //Y += Velocity.Y;
            FP.Log(X, Y);
        }
    }
    public class Ship : BaseShip
    {
        public Ship()
        {
            shipParts = new List<PartBase>();
        }

        public override void Added()
        {
            base.Added();

            Type = "Ship";
            ShipCenter = Image.CreateRect(64, 64, FP.Color(0xffffff));

            ShipCenter.CenterOO();

            X = FP.HalfWidth;
            Y = FP.HalfHeight;
        }

        public override void Update()
        {
            base.Update();

            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                X += (float)Math.Cos(FP.RAD * ShipCenter.Angle) * 5;
                Y += (float)Math.Sin(FP.RAD * ShipCenter.Angle) * 5;
            }
            else
            {
                ClearTweens();
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                X -= (float)Math.Cos(FP.RAD * ShipCenter.Angle) * 5;
                Y -= (float)Math.Sin(FP.RAD * ShipCenter.Angle) * 5;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                ShipCenter.Angle += 2;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                ShipCenter.Angle -= 2;
            }
            SetHitboxTo(ShipCenter);
            CenterOrigin();
            FP.Camera.X = X;
            FP.Camera.Y = Y;
        }
    }

}
