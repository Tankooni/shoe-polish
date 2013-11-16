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
    public abstract class PartBase : Entity
    {
        public int curRow;
        public int curCol;
        public BaseShip myShip;
        public float health = 100;
        public float totalHealth = 100;
        public int MyType;
        public float rotationOffSet;
        protected Sfx AsteroidHit = new Sfx(Library.GetBuffer("Collision.Wav"));

        public PartBase(Entity e, int PartItem, int Col, int Row)
        {
            Type = "PartBase";
            curCol = Col;
            curRow = Row;
            myShip = e as BaseShip;
            myShip = e as Ship;
            MyType = PartItem;
        }

        public PartBase(int PartItem)
        {

        }
        public PartBase(Entity e, int Col, int Row)
        {

        }

        public virtual bool DoDamage(float damage)
        {
            health -= damage;
            if (health < 0)
                return true;
            return false;
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
        private Sfx TurretSfx = new Sfx(Library.GetBuffer("gun.Wav"));
        private Sfx EquipSfx = new Sfx(Library.GetBuffer("Docking.Wav"));

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
            myShip = e as BaseShip;
            
            myShip.shipParts.Add(this);
            if (PartItem == 1)
            {
                EquipSfx.Play();
                Image Hull = new Image(Library.GetTexture("MetalPlating.png"));
                aPart = Hull;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
                totalHealth = health = 150;
            }
            else if (PartItem == 2)
            {
                EquipSfx.Play();
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;
                AddGraphic(aPart);
                SetHitboxTo(aPart);
                totalHealth = health = 100;
            }
            else if (PartItem == 3)
            {
                EquipSfx.Play();
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;

                totalHealth = health = 50;
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
                totalHealth = health = 150;
            }
            if (PartItem == 2)
            {
                Image Thruster = new Image(Library.GetTexture("Thruster.png"));
                aPart = Thruster;

                AddGraphic(aPart);
                SetHitboxTo(aPart);
                totalHealth = health = 100;
            }
            else if (PartItem == 3)
            {
                Image Turret = new Image(Library.GetTexture("Turret.png"));
                aPart = Turret;
                

                AddGraphic(aPart);
                SetHitboxTo(aPart);
                totalHealth = health = 50;
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
            {
                if (myShip != null)
                {
                    if (myShip as Ship == null)
                    {
                        List<PartBase> temp = myShip.shipParts;
                        World.Remove(myShip);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            if (temp[i].MyType != 0)
                            {
                                Part replacementPart = new Part(temp[i].MyType);
                                replacementPart.X = temp[i].X;
                                replacementPart.Y = temp[i].Y;
                                World.Add(replacementPart);
                            }

                            World.Remove(temp[i]);
                        }
                        World.Remove(myShip);
                    }
                    else
                    {
                        World.Add(new EmptyPart(myShip, curCol, curRow, curCol, curRow));
                        World.Remove(this);
                        if (myShip.shipParts.Count <= 0)
                        {
                            var Title = new Text("Game Over! Press R to Restart The Game.", 100, FP.HalfHeight - 200, 0, 0);
                            Title.Size = 100;
                            AddGraphic(Title);
                        }
                    }
                }
                else
                {
                    World.Remove(this);
                }
            }

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
                
                    //FP.Log(FP.Angle(X, Y, World.MouseX, World.MouseY));

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
                        if(Mouse.IsButtonPressed(Mouse.Button.Right))
                        {
                            TurretSfx.Volume = FP.Random * 20 + 20;
                            TurretSfx.Pitch = FP.Random*.3f +.2f;
                            TurretSfx.Play();

                            World.Add(new Bullet(X, Y, new Vector2f((float)Math.Cos(aPart.Angle * FP.RAD) * 5, (float)Math.Sin(aPart.Angle * FP.RAD) * 5), myShip));
                        }
                        else
                        {
                            //TurretSfx.Pitch = .01f; //+ 0.5f;
                        }
                    
                    }
                    else
                    {
                        //TurretSfx.Pitch = .01f; //+ 0.5f;
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
            //aPart.Color = FP.Color(0x000000);
            aPart.Color = new SFML.Graphics.Color((byte)(byte)(FP.Clamp<int>((int)((health / totalHealth) * 255), 0, 255)), (byte)(byte)(FP.Clamp<int>((int)((health / totalHealth) * 255), 0, 255)), (byte)(byte)(FP.Clamp<int>((int)((health / totalHealth) * 255), 0, 255)));

        }
    }

    public class Bullet : Punk.SpaceObject
    {
        private Vector2f Velocity;
        private Image BulletShot;
        BaseShip myShip;
        float life = 1;
        public Bullet(float x, float y, Vector2f VelocityIn, Entity ShipIn)
            : base(x, y, Punk.SpaceWorld.WorldLength, Punk.SpaceWorld.WorldHeight)
        {
            Type = "Bullet";
            Graphic = BulletShot = Image.CreateCircle(2, FP.Color(0x4083FF));
            X = x;
            Y = y;
            BulletShot.CenterOO();
            SetHitboxTo(BulletShot);
            CenterOrigin();
            myShip = ShipIn as Ship;
            Velocity = VelocityIn + myShip.Velocity;
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
            if (e is Part)
            {
                if ((e as Part).myShip == myShip)
                {
                    return false;
                }
                (e as Part).DoDamage(4);
            }
            else if (e is Punk.Asteroid)
                (e as Punk.Asteroid).DoDamage(4);
            World.Remove(this);
            return true;
            
        }

        public override void Update()
        {
            base.Update();
            MoveBy(Velocity.X * 2, Velocity.Y * 2, new string[] {"Asteroid", "Part"});
            if ((life -= FP.Elapsed) <= 0)
                World.Remove(this);
            
        }
    }

}

