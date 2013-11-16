using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using Testing;

namespace Punk
{
	class SpaceWorld : World
	{
		protected Sector[,] Sectors = new Sector[10, 10];
		public static uint WorldLength = Sector.SectorSize * 10;
		public static uint WorldHeight = Sector.SectorSize * 10;
        public Ship PlayerShip;
		public SpaceWorld()
		{
		}
			
		public override void Begin()
		{
			base.Begin();
			FP.Engine.ClearColor = FP.Color(0xff00ff);
			for (int x = 0; x < Sectors.GetLength(0) - 1; x++)
			{
				for (int y = 0; y < Sectors.GetLength(1) - 1; y++)
				{
					Sectors[x, y] = new SpaceSector();
					Sectors[x, y].X = x * Sectors[x, y].Width;
					Sectors[x, y].Y = y * Sectors[x, y].Height;
					Add(Sectors[x, y]);
				}
			}
			var e = Add(PlayerShip = new Ship());
			Add(new Part(e, 1, 0, 0));
			for (int i = 0; i < 5; i++)
			{
				Add(new Part(1));
				Add(new Part(2));
			}
			Add(new Part(e, 3, 0, 0));
			FP.Log(WorldHeight);
			//Input.ControllerConnected += (s, e) => Add(new JoystickGuy(e.JoystickId));
			//Input.Pressed(Mouse.Button.Left);
		}

		public override void Update()
		{
			//FP.Camera.Angle += (float)Math.PI / 5;
			base.Update();
			if (Input.Down(Keyboard.Key.Up))
			{
                PlayerShip.MoveTo(100, 100);
			}
			else if (Input.Down(Keyboard.Key.Down))
			{
				Camera.Y += 10;
			}

			if (Input.Down(Keyboard.Key.Left))
			{
				Camera.X -= 10;
			}
			else if (Input.Down(Keyboard.Key.Right))
			{
				Camera.X += 10;
			}
		}

		public override void End()
		{
			base.End();
		}
	}
}
