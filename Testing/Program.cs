using System;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;

namespace Punk
{
	class Game : Engine
	{
		public static void Main(string[] args)
		{
			var game = new Game();
		}
		
		private class JoystickGuy : Entity
		{
			private Controller controller;
			private uint id;
			private static float speed = 3;
			
			private Image image;
			
			public JoystickGuy(uint joyID)
			{
				Graphic = image = Image.CreateRect(100, 100, FP.Color(FP.Rand(uint.MaxValue)));
				X = (int) FP.Rand(FP.Width);
				Y = (int) FP.Rand(FP.Height);
				
				image.CenterOO();
				SetHitboxTo(image);

				CenterOrigin();
				
				controller = new Controller(joyID);
				controller.Disconnected += delegate { World.Remove(this); };

				Type = "joyguy";

				AddResponse("A", OnEnemyA);

				id = joyID;
			}

			private void OnEnemyA(params object[] ars)
			{
				ClearTweens();
				//image.Scale = 1.3f;
				var tween = new VarTween(null, ONESHOT);
				tween.Tween(this, "Y", Y+100, 5, Ease.ElasticOut);
				AddTween(tween, true);
			}
			
			public override void Update()
			{
				base.Update();
				
				if (controller.Pressed(Controller.Button.A))
				{
					World.BroadcastMessage("A");
				}
				//X = Input.MouseX;
				//Y = Input.MouseY;
				//MoveTo(X, Y);

				//MoveBy(speed * controller.DPad.X, speed * controller.DPad.Y, Type);
			}

			public override bool MoveCollideX(Entity e)
			{
				e.X += 100;

				FP.Log("OW");
				return base.MoveCollideX(e);
			}
		}
		
		public Game() : 
		base(1280, 720, 60)
		{
		}
		
		public override void Init()
		{
			base.Init();
			
			FP.World = new SpaceWorld();
			FP.Screen.GainedFocus += delegate { Library.Reload(); };
			FP.Console.Enable();
		}
	}
	
	
}