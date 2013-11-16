using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using Punk;

namespace Testing.MainMenu
{
    class MenuWorld : World
    {
        Graphic Title = new Text("Black Shoe Polish", FP.HalfWidth, FP.HalfHeight, 0, 0);
        
        
        public MenuWorld()
        {
            var Title = new Text("Black Shoe Polish", 100, FP.HalfHeight - 200, 0, 0);
            Title.Size = 100;
            AddGraphic(Title);
            
            Add(new StartButton());
            
        }
    }

    class StartButton : Entity
    {
        private Image Button;
        public StartButton()
        {
            Type = "ButtonState";
            Button = Image.CreateRect(400, 200, FP.Color(0xFFFFFF));
            var Title = new Text("Start", 0, 0, 0, 0);
            Title.Size = 100;
            Title.Color = FP.Color(0x000000);
            var Instructions = new Text("Instructions: W,A,S,D Control, Right Mouse to Shoot, Left Mouse to Grab Floating Parts.", 0, 0, 0, 0);
            
            AddGraphic(Button);
            AddGraphic(Title);
            AddGraphic(Instructions);
            
            Instructions.CenterOO();
            Instructions.Size = 20;
            Instructions.Color = FP.Color(0xffffff);
            Instructions.X -= 450;
            Instructions.Y -= 200;
            Title.CenterOO();
            Title.X -= 125;
            Title.Y -= 75;
            SetHitboxTo(Button);
            CenterOrigin();
            Button.CenterOO();
            Button.CenterOrigin();
            X = FP.HalfWidth;
            Y = FP.HalfHeight +100;
            FP.Log(Instructions.X, Instructions.Y);
            
        }
        
        public override void Update()
        {
            base.Update();
            if (World.CollidePoint("ButtonState", Input.MouseX, Input.MouseY) != null)
            {
                Button.Scale = 1.2f;

                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    FP.World = new SpaceWorld();
                }
            }
            else
            {
               Button.Scale = 1f;
            }
            X = X;
            Y = Y;
            
        }

    }
}
