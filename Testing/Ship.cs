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
    public class Ship : Entity
    {
        public Image ShipCenter;

        public List<PartBase> shipParts;

        private Vector2f Velocity;

        public float mass, forwardThrust, leftThrust, rightThrust, backThrust;

        public Ship()
        {
            shipParts = new List<PartBase>();
            Velocity = new Vector2f(0, 0);
            mass = forwardThrust = leftThrust = rightThrust = 0;
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

            forwardThrust = 0;
            leftThrust = 0.12f;
            rightThrust = 0.12f;
            backThrust = 0;

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
            SetHitboxTo(ShipCenter);
            CenterOrigin();
            FP.Camera.X = X;
            FP.Camera.Y = Y;
        }
    }
}
