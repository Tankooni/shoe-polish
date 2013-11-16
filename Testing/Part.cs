﻿using System;
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
    class Part:Entity
    {
        private Ship myShip;
        private Image aPart;
        private int PartNum;
        private int curRow;
        private int curCol;
        private float ChangeAng;
        private float Distance;
        private float angle;
        private bool Flying;
        private bool Dragging;
        private float LastRot;
        public Part(Entity e, int PartItem, int Col, int Row)
        {
            Flying = true;
            Dragging = false;
            Type = "Part";


            //ChangeAng = InAng;
            //ChangeX = InX;
            curCol = Col;
            curRow = Row;
            int A = (32 * Col) * (32 * Col);
            int B = (32 * Row) * (32 * Row);
            float C = (float)Math.Sqrt(A + B);
            Distance = C;
            angle = (float)Math.Atan2(Col, Row) * (180 / (float)Math.PI);
            PartNum = PartItem;
            myShip = e as Ship;
            if (PartItem == 0)
            {

                aPart = Image.CreateCircle(16, FP.Color(0xFFFF00));

                AddGraphic(aPart);
                SetHitboxTo(aPart);

            }
            else if (PartItem == 1)
            {
                
                aPart = Image.CreateRect(32, 32, FP.Color(0x00ffff));

                AddGraphic(aPart);
                SetHitboxTo(aPart);
                
            }
            else if (PartItem == 2)
            {
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;
                

                AddGraphic(aPart);
                SetHitboxTo(aPart);

            }

            

            X = 100;
            Y = 100;
        }


        public Part(int PartItem)
        {
            Flying = false;
            Dragging = false;
            Type = "Part";
            
            if (PartItem == 1)
            {
                aPart = Image.CreateRect(32, 32, FP.Color(0x00ffff));
                AddGraphic(aPart);
                
            }
        }

        public Image GetImage()
        {
            return aPart;
        }


        public override void Render()
        {
            base.Render();
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                //Draw.Line(new Vector2f(myShip.X, myShip.Y), new Vector2f((float)World.MouseX, (float)World.MouseY), FP.Color(0x7A0202));
            }
        }

        public override void Update()
        {
            base.Update();
            
            SetHitboxTo(aPart);
            CenterOrigin();
            aPart.CenterOO();
            if(Input.Pressed(Mouse.Button.Left))
            {
                if(World.CollidePoint("Part", World.MouseX, World.MouseY) != null)
                {
                    Dragging = true;
                }
            }
            else if (Input.Released(Mouse.Button.Left))
            {
                Dragging = false;
            }

            FP.AnchorTo(ref X, ref Y, myShip.X, myShip.Y, Distance, Distance);
            FP.RotateAround(ref X, ref Y, myShip.X, myShip.Y, myShip.ShipCenter.Angle + angle, false);

            if (PartNum == 2)
            {
                
                FP.Log(FP.Angle(X, Y, World.MouseX, World.MouseY));
                
                
                if ((myShip.ShipCenter.Angle + 45 > FP.Angle(X, Y, World.MouseX, World.MouseY)) && (myShip.ShipCenter.Angle - 45 < FP.Angle(X, Y, World.MouseX, World.MouseY)))
                {
                    //LastRot = FP.Angle(X, Y, World.MouseX, World.MouseY);
                    aPart.Angle = FP.Angle(X, Y, World.MouseX, World.MouseY);
                    if (Mouse.IsButtonPressed(Mouse.Button.Right))
                    {
                        
                        World.Add(new Bullet(X, Y, new Vector2f((float)Math.Cos(aPart.Angle * FP.RAD) * 5, (float)Math.Sin(aPart.Angle * FP.RAD) * 5)));
                    }
                    
                }
                else
                {
                    aPart.Angle = myShip.ShipCenter.Angle;
                }

            }
            else
            {
                aPart.Angle = myShip.ShipCenter.Angle;
                
            }

            if (Dragging)
            {
                X = World.MouseX;
                Y = World.MouseY;
                Flying = false;
            }
            if(Flying)
            {
                //X += FP.Rand(5);
                //Y += FP.Rand(5);
            }

            
        }
    }

    public class Bullet : Entity
    {
        private Vector2f Velocity;
        private Image BulletShot;
        public Bullet(float x, float y, Vector2f VelocityIn)
        {
            Graphic = BulletShot = Image.CreateCircle(2, FP.Color(0x4083FF));
            X = x;
            Y = y;
            Velocity = VelocityIn;
            BulletShot.CenterOO();
            SetHitboxTo(BulletShot);
            CenterOrigin();
        }

        public override void Update()
        {
            base.Update();
            X += Velocity.X * 2;
            Y += Velocity.Y * 2;
        }
    }

}