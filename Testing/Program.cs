using System;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using Testing;

namespace Punk
{
	class Game : Engine
	{
		public static void Main(string[] args)
		{
			var game = new Game();
		}

        private class myShip : Entity
        {
            //private Image Ship;

            //private Image LeftWing;
            //private Image RightWing;
            //private Image[] Turrets;
            //private Image[] Thrusters;

            //public myShip()
            //{
            //    Ship = Image.CreateRect(50, 100, FP.Color(0x00000));
            //    LeftWing = Image.CreateRect(50, 20, FP.Color(0x00ff00));
            //    RightWing = Image.CreateRect(50, 20, FP.Color(0x00ff00));
            //    AddGraphic(Ship);
            //    AddGraphic(LeftWing);
            //    AddGraphic(RightWing);
            //    Ship.CenterOO();

            //    LeftWing.OriginX = (LeftWing.Width) + (Ship.Width / 2);
            //    LeftWing.OriginY = LeftWing.Height / 2;

            //    RightWing.OriginX = (RightWing.X) - (Ship.Width / 2);
            //    RightWing.OriginY = RightWing.Height / 2;
                
            //    X = FP.HalfWidth;
            //    Y = FP.HalfHeight;
            //    //LeftWing.X = Ship.X - 25;
            //    //LeftWing.Y = Ship.Y + 25;
            //    //RightWing.X = Ship.X + 25;
            //    //RightWing.Y = Ship.Y + 50;
            //    FP.Log(Ship.X, Ship.Y);
                
            //}

            //public override void Update()
            //{
            //    base.Update();

            //    if (Mouse.IsButtonPressed(Mouse.Button.Left))
            //    {
                    
            //    }

            //    if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            //    {
            //        Y += 2;
            //    }

            //    if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            //    {
            //        Ship.Angle += 2;
            //    }

               
            //    LeftWing.Angle = RightWing.Angle = Ship.Angle;
            //}
        }
		
		private class JoystickGuy : Entity
		{
            //private Controller controller;
            //private uint id;
            //private static float speed = 3;
			
            //private Image image;
			
			public JoystickGuy(uint joyID)
			{
                //Graphic = image = Image.CreateRect(100, 100, FP.Color(FP.Rand(uint.MaxValue)));
                //X = (int) FP.Rand(FP.Width);
                //Y = (int) FP.Rand(FP.Height);
				
                //image.CenterOO();
                //SetHitboxTo(image);

                //CenterOrigin();
				
                //controller = new Controller(joyID);
                //controller.Disconnected += delegate { World.Remove(this); };

                //Type = "joyguy";

                //AddResponse("A", OnEnemyA);

                //id = joyID;
			}

            private void OnEnemyA(params object[] ars)
            {
                //ClearTweens();
                ////image.Scale = 1.3f;
                //var tween = new VarTween(null, ONESHOT);
                //tween.Tween(this, "Y", Y+100, 5, Ease.ElasticOut);
                //AddTween(tween, true);
            }
			
			public override void Update()
			{
				base.Update();
				
                //if (controller.Pressed(Controller.Button.A))
                //{
                //    World.BroadcastMessage("A");
                //}
                //X = Input.MouseX;
                //Y = Input.MouseY;
                //MoveBy(speed * controller.DPad.X, speed * controller.DPad.Y, Type);
			}

           
		}
		
		private class MyWorld : World
		{
			public MyWorld()
			{
			}
			
			public override void Begin()
			{
				base.Begin();
				
				FP.Engine.ClearColor = FP.Color(0x000000);
                //Add(new JoystickGuy(0));
                
                var e = Add(new Ship());
                Add(new Part(e, 1, 0, 0));
                Add(new Part(e, 1, 1, 1));
                Add(new Part(e, 1, 1, 0));
                Add(new Part(e, 1, 1, -1));
                Add(new Part(e, 1, 0, -1));
                Add(new Part(e, 1, -1, -1));
                Add(new Part(e, 1, -1, 0));
                Add(new Part(e, 1, -1, 1));
                Add(new Part(e, 1, 0, 1));

                Add(new Part(e, 0, -1, -2));
                Add(new Part(e, 0, 0, -2));
                Add(new Part(e, 0, 1, -2));
				//Input.ControllerConnected += (s, e) => Add(new JoystickGuy(e.JoystickId));
                
			}
			
		}
		
		public Game() : 
		base(1280, 960, 60)
		{
		}
		
		public override void Init()
		{
			base.Init();
			
			FP.World = new MyWorld();
			FP.Screen.GainedFocus += delegate { Library.Reload(); };
            
			FP.Console.Enable();
		}
	}
	
	
}