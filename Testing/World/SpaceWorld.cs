﻿using System;
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
        public Sfx musics;
		public SpaceWorld()
		{
		}
			
		public override void Begin()
		{
			base.Begin();
            musics = new Sfx(Library.GetBuffer("BlackVortex.ogg"));
            musics.Pitch = .5f;
            musics.Loop();
            

			FP.Engine.ClearColor = FP.Color(0x000000);
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
                Part n = new Part(1);
                n.X = FP.Rand(800);
                n.Y = FP.Rand(800);
                Part nn = new Part(2);
                nn.X = FP.Rand(800);
                nn.Y = FP.Rand(800);
				Add(n);
				Add(nn);
			}
			Add(new Part(e, 3, 0, 0));
            for (int i = 0; i < 30; i++)
            {
                Add(new EmptyShip());
            }

			//FP.Log(WorldHeight);
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
			else if (Input.Pressed(Keyboard.Key.R))
			{
                FP.World = new SpaceWorld();
			}

			if (Input.Down(Keyboard.Key.Left))
			{
				
			}
			else if (Input.Down(Keyboard.Key.Right))
			{
				
			}
		}

		public override void End()
		{
			base.End();
		}
	}
}
