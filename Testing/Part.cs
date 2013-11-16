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
    public class PartBase : Entity
    {
        public int curRow;
        public int curCol;
        public Ship myShip;
        public int MyType;
        public float rotationOffSet;
        public int health = 100;

        public PartBase(Entity e, int PartItem, int Col, int Row)
        {
            Type = "PartBase";
            curCol = Col;
            curRow = Row;
            myShip = e as Ship;
            MyType = PartItem;
        }
        public PartBase(int PartItem)
        {

        }
        public PartBase(Entity e, int Col, int Row)
        {

        }
    }
    public class Part : PartBase
    {
        private Image aPart;

        private float Distance;
        private float angle;
        private int PartNum;
        private bool Flying;
        private bool Dragging;
        private bool Attached;

        public Part(Entity e, int PartItem, int Col, int Row) : base(e, PartItem, Col, Row)
        {
            Flying = true;
            Dragging = false;
            Type = "Part";
            curCol = Col;
            curRow = Row;

            MyType = PartItem;
            Attached = true;

            rotationOffSet = 0;

            int A = (32 * Col) * (32 * Col);
            int B = (32 * Row) * (32 * Row);
            float C = (float)Math.Sqrt(A + B);
            Distance = C;
            angle = (float)Math.Atan2(Col, Row) * (180 / (float)Math.PI);
            PartNum = PartItem;
            myShip = e as Ship;
            myShip.shipParts.Add(this);
            if (PartItem == 1)
            {
                Image Hull = new Image(Library.GetTexture("MetalPlating.png"));
                aPart = Hull;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
                health = 200;
            }
            else if (PartItem == 2)
            {
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
                health = 100;
            }
            else if (PartItem == 3)
            {
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;

                health = 50;
                AddGraphic(aPart);
                SetHitboxTo(aPart);

            }



            X = 100;
            Y = 100;
        }
        public Part(int PartItem) : base(PartItem) 
        {
            Flying = true;
            Attached = false;
            Dragging = false;
            Type = "Part";
            MyType = PartItem;

            if (PartItem == 1)
            {
                Image Hull = new Image(Library.GetTexture("MetalPlating.png"));
                aPart = Hull;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
                health = 200;
            }
            if (PartItem == 2)
            {
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;

                AddGraphic(aPart);
                SetHitboxTo(aPart);
                health = 100;
            }
            else if (PartItem == 3)
            {
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;
                

                AddGraphic(aPart);
                SetHitboxTo(aPart);
                health = 50;
            }
        }
        public override void Added()
        {
            base.Added();
            
            bool top = true;
            bool left = true;
            bool right = true;
            bool bottom = true;

            if (Attached)
            {
                if (MyType == 1)
                {
                    for (int i = 0; i < myShip.shipParts.Count; i++)
                    {
                        if (myShip.shipParts[i].curCol == curCol && myShip.shipParts[i].curRow == curRow)
                        {
                            //World.Remove(myShip.shipParts[i]);
                        }
                        if (myShip.shipParts[i].curCol == curCol && myShip.shipParts[i].curRow == curRow + 1)
                        {
                            top = false;
                        }
                        if (myShip.shipParts[i].curCol == curCol + 1 && myShip.shipParts[i].curRow == curRow)
                        {
                            right = false;
                        }
                        if (myShip.shipParts[i].curCol == curCol && myShip.shipParts[i].curRow == curRow - 1)
                        {
                            bottom = false;
                        }
                        if (myShip.shipParts[i].curCol == curCol - 1 && myShip.shipParts[i].curRow == curRow)
                        {
                            left = false;
                        }
                    }
                    if (top)
                    {
                        World.Add(new EmptyPart(myShip, curCol, curRow + 1, curCol, curRow));
                    }
                    if (right)
                    {
                        World.Add(new EmptyPart(myShip, curCol + 1, curRow, curCol, curRow));
                    }
                    if (bottom)
                    {
                        World.Add(new EmptyPart(myShip, curCol, curRow - 1, curCol, curRow));
                    }
                    if (left)
                    {
                        World.Add(new EmptyPart(myShip, curCol - 1, curRow, curCol, curRow));
                    }
                }
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

            if (health <= 0)
                World.Remove(this);

            if (Attached)
            {
                SetHitboxTo(aPart);
                aPart.CenterOO();
                CenterOrigin();

                FP.AnchorTo(ref X, ref Y, myShip.X, myShip.Y, Distance, Distance);
                FP.RotateAround(ref X, ref Y, myShip.X, myShip.Y, myShip.ShipCenter.Angle + angle, false);

                aPart.Angle = myShip.ShipCenter.Angle;

                if (PartNum == 3)
                {
                
                    FP.Log(FP.Angle(X, Y, World.MouseX, World.MouseY));
                
                
                    if ((myShip.ShipCenter.Angle + 45 > FP.Angle(X, Y, World.MouseX, World.MouseY)) && (myShip.ShipCenter.Angle - 45 < FP.Angle(X, Y, World.MouseX, World.MouseY)))
                    {
                        //LastRot = FP.Angle(X, Y, World.MouseX, World.MouseY);
                        aPart.Angle = FP.Angle(X, Y, World.MouseX, World.MouseY);
                        if (Mouse.IsButtonPressed(Mouse.Button.Right))
                        {
                        
                            World.Add(new Bullet(X, Y, new Vector2f((float)Math.Cos(aPart.Angle * FP.RAD) * 5, (float)Math.Sin(aPart.Angle * FP.RAD) * 5), myShip));
                        }
                    
                    }
                    else
                    {
                        aPart.Angle = myShip.ShipCenter.Angle;
                    }

                }
                else
                {
                    aPart.Angle = myShip.ShipCenter.Angle + rotationOffSet;
                
                }
            }
            else
            {
                SetHitboxTo(aPart);
                aPart.CenterOO();
                CenterOrigin();
                EmptyPart temp = World.CollideRect("EmptyPart", X, Y, Width, Height) as EmptyPart;
                if (temp != null)
                {
                    float t = (float)Math.Atan2(temp.curRow - temp.parentRow, temp.curCol - temp.parentCol);
                    Part add = new Part(temp.myShip, MyType, temp.curCol, temp.curRow);
                    add.rotationOffSet = (t * FP.DEG) - 90;
                    add.rotationOffSet = (float)Math.Round(add.rotationOffSet);
                    if (add.rotationOffSet < 0.0002 && add.rotationOffSet > -0.0002)
                    {
                        add.rotationOffSet = 0;
                    }
                    World.Add(add);
                    World.Remove(temp);
                    World.Remove(this);
                }

                if (Input.Pressed(Mouse.Button.Left))
                {
                    if (World.CollidePoint("Part", World.MouseX, World.MouseY) == this)
                    {
                        Dragging = true;
                    }
                }
                else if (Input.Released(Mouse.Button.Left))
                {
                    Dragging = false;
                }

                if (Dragging)
                {
                    X = World.MouseX;
                    Y = World.MouseY;
                    Flying = false;
                }
                if (Flying)
                {
                    X += FP.Rand(2);
                    Y += FP.Rand(2);
                }
            }

        }
    }

    public class Bullet : Punk.SpaceObject
    {
        private Vector2f Velocity;
        private Image BulletShot;
        Ship myShip;
        float life = 1;
        public Bullet(float x, float y, Vector2f VelocityIn, Entity ShipIn)
            : base(x, y, Punk.SpaceWorld.WorldLength, Punk.SpaceWorld.WorldHeight)
        {
            Graphic = BulletShot = Image.CreateCircle(2, FP.Color(0x4083FF));
            X = x;
            Y = y;
            Velocity = VelocityIn;
            BulletShot.CenterOO();
            SetHitboxTo(BulletShot);
            CenterOrigin();
            myShip = ShipIn as Ship;
        }

        public override bool MoveCollideX(Entity e)
        {
            return base.MoveCollideX(e);
        }
        public override bool MoveCollideY(Entity e)
        {
            return base.MoveCollideY(e);
        }
        public bool Collided(Entity e)
        {
            if (e is PartBase)
            {
                if ((e as PartBase).myShip == myShip)
                {
                    FP.Log("Stop Hitting Yourself");
                    return false;
                }
            }
            return true;
             
        }

        public override void Update()
        {
            base.Update();
            if ((life -= FP.Elapsed) <= 0)
                World.Remove(this);
            MoveBy(Velocity.X * 2,  Velocity.Y * 2);
        }
    }

}

