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

        public int mass, forwardThrust, leftThrust, rightThrust;

        public Ship()
        {
            shipParts = new List<PartBase>();
        }

        public override void Added()
        {
            base.Added();
            //var AddPart1 = new Part(1);
            //var AddPart2 = new Part(1);
            //var AddPart3 = new Part(1);
            //var AddPart4 = new Part(1);
            //var AddPart5 = new Part(1);
            //var AddPart6 = new Part(1);
            //var AddPart7 = new Part(1);
            //var AddPart8 = new Part(1);
            //var AddPart9 = new Part(1);


            Type = "Ship";
            ShipCenter = Image.CreateRect(64, 64, FP.Color(0xffffff));
            //MainShip1 = AddPart1.GetImage();

            //MainShip2 = AddPart2.GetImage();
            //MainShip3 = AddPart3.GetImage();
            //MainShip4 = AddPart4.GetImage();
            //MainShip5 = AddPart5.GetImage();
            //MainShip6 = AddPart6.GetImage();
            //MainShip7 = AddPart7.GetImage();
            //MainShip8 = AddPart8.GetImage();
            //MainShip9 = AddPart9.GetImage();

            //Thruster = Image.CreateCircle(16, FP.Color(0xFFFF00));
            //Thrusters.Add(Thruster);
            //Thruster = Image.CreateCircle(16, FP.Color(0xFFFF00));
            //Thrusters.Add(Thruster);

            //AddGraphic(ShipCenter);
            //AddGraphic(MainShip1);
            //ShipArray[1, 2] = MainShip1;
            //AddGraphic(MainShip2);
            //ShipArray[1, 3] = MainShip2;
            //AddGraphic(MainShip3);
            //ShipArray[1, 4] = MainShip3;
            //AddGraphic(MainShip4);
            //ShipArray[2, 2] = MainShip4;
            //AddGraphic(MainShip5);
            //ShipArray[2, 3] = MainShip5;
            //AddGraphic(MainShip6);
            //ShipArray[2, 4] = MainShip6;
            //AddGraphic(MainShip7);
            //ShipArray[3, 2] = MainShip7;
            //AddGraphic(MainShip8);
            //ShipArray[3, 3] = MainShip8;
            //AddGraphic(MainShip9);
            //ShipArray[3, 4] = MainShip9;

            ////var masklist = new Masklist();
            ////masklist.Add(new Hitbox(Convert.ToInt32(MainShip1.Width), Convert.ToInt32(MainShip1.Height), -Convert.ToInt32(MainShip1.Width), -Convert.ToInt32(MainShip1.Height)));
            ////masklist.Add(new Hitbox(Convert.ToInt32(MainShip2.Width), Convert.ToInt32(MainShip2.Height), -Convert.ToInt32(MainShip2.X), -Convert.ToInt32(MainShip2.Height)));
            ////masklist.Add(new Hitbox(Convert.ToInt32(MainShip3.Width), Convert.ToInt32(MainShip3.Height), -Convert.ToInt32(MainShip3.X), -Convert.ToInt32(MainShip3.Y)));
            //////masklist.Add(new Hitbox(Convert.ToInt32(MainShip4.Width), Convert.ToInt32(MainShip4.Height), -Convert.ToInt32(MainShip4.Width), -Convert.ToInt32(MainShip4.Y)));
            ////Mask = masklist;

            // Thruster 1
            //Thrusters[0].OriginX = Thrusters[0].Width + MainShip1.Width + MainShip1.Width / 2;
            //Thrusters[0].OriginY = Thrusters[0].Height + MainShip1.Height / 2;
            //AddGraphic(Thrusters[0]);
            //ShipArray[0, 2] = Thrusters[0];
            //// Thruster 1
            //Thrusters[1].OriginX = Thrusters[1].Width + MainShip3.Width + MainShip3.Width / 2;
            //Thrusters[1].OriginY = -MainShip3.Height / 2;
            //AddGraphic(Thrusters[1]);
            //ShipArray[0, 3] = Thrusters[1];

            ShipCenter.CenterOO();



            ////Top left Frame
            //MainShip1.OriginX = MainShip1.Width + MainShip1.Width / 2;
            //MainShip1.OriginY = MainShip1.Height + MainShip1.Height / 2;

            ////Middle Left Frame
            //MainShip2.OriginX = MainShip2.Width + MainShip2.Width / 2;
            //MainShip2.OriginY = MainShip2.Height / 2;

            ////Bottom left Frame
            //MainShip3.OriginX = MainShip3.Width + MainShip3.Width / 2;
            //MainShip3.OriginY = -MainShip3.Height +  MainShip3.Height / 2;
            //////////////////////////////////////////////////////////////////////////////
            ////Top Middle Frame
            //MainShip4.OriginX = MainShip4.Width / 2;
            //MainShip4.OriginY = MainShip4.Height + MainShip4.Height / 2;


            ////Middle Middle Frame
            //MainShip5.OriginX = MainShip5.Width / 2;
            //MainShip5.OriginY = MainShip5.Height / 2;

            ////Bottom Middle Frame
            //MainShip6.OriginX = MainShip6.Width / 2;
            //MainShip6.OriginY = -MainShip6.Height + MainShip6.Height / 2;
            /////////////////////////////////////////////////////////////////////////////////
            ////Top Right Frame
            //MainShip7.OriginX = -MainShip7.Width + MainShip7.Width / 2;
            //MainShip7.OriginY = MainShip7.Height + MainShip7.Height / 2;

            ////Middle Right Frame
            //MainShip8.OriginX = -MainShip8.Width + MainShip8.Width / 2;
            //MainShip8.OriginY = MainShip8.Height / 2;

            ////Bottom Right Frame
            //MainShip9.OriginX = -MainShip9.Width + MainShip9.Width / 2;
            //MainShip9.OriginY = -MainShip9.Height + MainShip9.Height / 2;



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

                //var tween1 = new VarTween(null, LOOPING);
                //tween1.Tween(Thrusters[0], "Scale", Thrusters[0].Scale - 0.1f, 0.1f, Ease.ElasticOut);
                //AddTween(tween1, true);

                //var tween2 = new VarTween(null, LOOPING);
                //tween2.Tween(Thrusters[1], "Scale", Thrusters[1].Scale - 0.1f, 0.1f, Ease.ElasticOut);
                //AddTween(tween2, true);

            }
            else
            {
                //Thrusters[0].Scale = 1;
                //Thrusters[1].Scale = 1;
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
