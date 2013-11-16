/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/16/2013
 * Time: 6:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Graphics
{
	/// <summary>
	/// A simple non-transformed, non-animated graphic.
	/// </summary>
	public class Stamp : Graphic
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="texture">Source image.</param>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public Stamp(Texture texture, int x = 0, int y = 0)
		{
			Source = texture;
			
			X = x;
			Y = y;
		}
		
		/// <summary>
		/// Renders the graphic.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public override void Render(float x, float y, Camera camera)
		{
			var sprite = new Sprite(Source);
			sprite.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
			FP.Screen.Draw(sprite);
		}
		
		/// <summary>
		/// Width of the stamp.
		/// </summary>
		public uint Width
		{
			get
			{
				return Source.Size.X;
			}
		}
		
		/// <summary>
		/// Height of the stamp.
		/// </summary>
		public uint Height
		{
			get
			{
				return Source.Size.Y;
			}
		}
		
		/// <summary>
		/// Source texture.
		/// </summary>
		public Texture Source;
	}
}
