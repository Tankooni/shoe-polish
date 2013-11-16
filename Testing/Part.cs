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
    public class PartBase : Entity
    {
        public int curRow;
        public int curCol;
        public Ship myShip;

        public PartBase(Entity e, int PartItem, int Col, int Row)
        {
            Type = "PartBase";
            curCol = Col;
            curRow = Row;
            myShip = e as Ship;
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
        private Sfx TurretSfx = new Sfx(Library.GetBuffer("TurretFire.Wav"));
        private Sfx EquipSfx = new Sfx(Library.GetBuffer("Docking.Wav"));

        private int MyType;

        public Part(Entity e, int PartItem, int Col, int Row) : base(e, PartItem, Col, Row)
        {
            Flying = true;
            Dragging = false;
            Type = "Part";
            curCol = Col;
            curRow = Row;

            MyType = PartItem;
            Attached = true;

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
                EquipSfx.Play();
                Image Hull = new Image(Library.GetTexture("MetalPlating.png"));
                aPart = Hull;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
            }
            else if (PartItem == 2)
            {
                EquipSfx.Play();
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
            }
            else if (PartItem == 3)
            {
                EquipSfx.Play();
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;


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
            }
            if (PartItem == 2)
            {
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;

                AddGraphic(aPart);
                SetHitboxTo(aPart);
            }
            else if (PartItem == 3)
            {
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;
                

                AddGraphic(aPart);
                SetHitboxTo(aPart);

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
                        World.Add(new EmptyPart(myShip, curCol, curRow + 1));
                    }
                    if (right)
                    {
                        World.Add(new EmptyPart(myShip, curCol + 1, curRow));
                    }
                    if (bottom)
                    {
                        World.Add(new EmptyPart(myShip, curCol, curRow - 1));
                    }
                    if (left)
                    {
                        World.Add(new EmptyPart(myShip, curCol - 1, curRow));
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

                    if (myShip.ShipCenter.Angle + 2 > 360)
                    {
                        myShip.ShipCenter.Angle = (myShip.ShipCenter.Angle + 2) - 360;
                    }
                    if (myShip.ShipCenter.Angle - 2 <= 0)
                    {
                        myShip.ShipCenter.Angle = (myShip.ShipCenter.Angle - 2) + 360;
                    }
                
                
                    if ((myShip.ShipCenter.Angle + 45 > FP.Angle(X, Y, World.MouseX, World.MouseY)) && (myShip.ShipCenter.Angle - 45 < FP.Angle(X, Y, World.MouseX, World.MouseY)))
                    {
                        aPart.Angle = FP.Angle(X, Y, World.MouseX, World.MouseY);
                        if (Mouse.IsButtonPressed(Mouse.Button.Right))
                        {
                            //TurretSfx.Volume = 2f;
                            TurretSfx.Pitch = TurretSfx.Pitch + 0.1f;
                            TurretSfx.Play();
                            
                            World.Add(new Bullet(X, Y, new Vector2f((float)Math.Cos(aPart.Angle * FP.RAD) * 5, (float)Math.Sin(aPart.Angle * FP.RAD) * 5)));
                        }
                    
                    }
                    else
                    {
                        TurretSfx.Pitch = .01f; //+ 0.5f;
                        aPart.Angle = myShip.ShipCenter.Angle;
                    }

                }
                else
                {
                    aPart.Angle = myShip.ShipCenter.Angle;
                
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
                    World.Add(new Part(temp.myShip, MyType, temp.curCol, temp.curRow));
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

