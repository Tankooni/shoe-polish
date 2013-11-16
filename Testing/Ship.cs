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

        protected Vector2f Velocity;
        uint rightBound = 0;
        uint bottomBound = 0;

        public float mass, forwardThrust, leftThrust, rightThrust, backThrust;

        public BaseShip()
        {
            shipParts = new List<PartBase>();
            Velocity = new Vector2f(0, 0);
            mass = forwardThrust = leftThrust = rightThrust = 0;
            rightBound = SpaceWorld.WorldLength - Sector.SectorSize;
            bottomBound = SpaceWorld.WorldHeight - Sector.SectorSize;
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

        public override void Update()
        {
            base.Update();
            if (X > rightBound)
                X = 0;
            else if (X < 0)
                X = rightBound;
            if (Y > bottomBound)
                Y = 0;
            else if (Y < 0)
                Y = bottomBound;
        }
    }
    public class EmptyShip : BaseShip
    {
        //private Vector2f Velocity;
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
            //FP.Log(X, Y);
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

            forwardThrust = 2;
            leftThrust = 0.12f;
            rightThrust = 0.12f;
            backThrust = 1;

            for (int i = 0; i < shipParts.Count; i++)
            {
                if (shipParts[i].MyType == 2)
                {
                    if (shipParts[i].rotationOffSet == 0)
                    {
                        forwardThrust += 10;
                    }
                    if (shipParts[i].rotationOffSet == -180)
                    {
                        backThrust += 5;
                    }
                    if (shipParts[i].rotationOffSet == -90)
                    {
                        leftThrust += 2;
                    }
                    if (shipParts[i].rotationOffSet == -270)
                    {
                        rightThrust += 2;
                    }
                }
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                Velocity.X += (float)Math.Cos(FP.RAD * ShipCenter.Angle) * forwardThrust * FP.Elapsed;
                Velocity.Y += (float)Math.Sin(FP.RAD * ShipCenter.Angle) * forwardThrust * FP.Elapsed;
            }
            else
            {
                ClearTweens();
            }
            X += Velocity.X;
            Y += Velocity.Y;
            Velocity.X *= 0.97f;
            Velocity.Y *= 0.97f;
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                Velocity.X -= (float)Math.Cos(FP.RAD * ShipCenter.Angle) * backThrust * FP.Elapsed;
                Velocity.Y -= (float)Math.Sin(FP.RAD * ShipCenter.Angle) * backThrust * FP.Elapsed;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                ShipCenter.Angle -= rightThrust;
            }

            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                ShipCenter.Angle += leftThrust;
            }
            //SetHitboxTo(ShipCenter);
            //CenterOrigin();
            FP.Camera.X = X;
            FP.Camera.Y = Y;
            FP.Camera.Angle = ShipCenter.Angle-90;
        }
    }
}
