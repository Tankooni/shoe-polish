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
            
        }

        private class JoystickGuy : Entity
        {
            
            public JoystickGuy(uint joyID)
            {
               
            }

           

            public override void Update()
            {
                base.Update();

               
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

                //Add(new Part(e, 1, -1, -1));
                //Add(new Part(e, 1, -1, 0));
                //Add(new Part(e, 1, -1, 1));
                //Add(new Part(e, 1, 0, 1));

                //Add(new Part(e, 0, -1, -2));
                //Add(new Part(e, 0, 0, -2));
                //Add(new Part(e, 0, 1, -2));
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

            FP.World = new SpaceWorld();
            FP.Screen.GainedFocus += delegate { Library.Reload(); };

            FP.Console.Enable();
        }
    }


}