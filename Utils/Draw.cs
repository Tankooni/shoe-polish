/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/7/2013
 * Time: 9:07 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Utils
{
	
	/// <summary>
	/// Static class with access to miscellaneous drawing functions.
	/// These functions are not meant to replace Graphic components
	/// for Entities, but rather to help with testing and debugging.
	/// </summary>
	public static class Draw
	{
		internal static SFML.Graphics.VertexArray verts;
		
		static Draw()
		{
			verts = new VertexArray(SFML.Graphics.PrimitiveType.Lines);
		}
		
		internal static void Render()
		{
			FP.Screen.Draw(verts);
		}
		
		internal static void Update()
		{
			verts.Clear();
		}
	
		/// <summary>
		/// Draws a pixelated, non-antialiased line.
		/// </summary>
		/// <param name="x1">Starting x position.</param>
		/// <param name="y1">Starting y position.</param>
		/// <param name="x2">Ending x position.</param>
		/// <param name="y2">Ending y position.</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="alpha">Alpha of the line. Defaults to 1</param>
		public static void Line(float x1, float y1, float x2, float y2, Color color, float alpha = 1)
		{
			color.A = (byte) (255 * alpha);
			
			Vertex[] line = new Vertex[]
			{
				new Vertex(new Vector2f(x1, y1), color),
				new Vertex(new Vector2f(x2, y2), color)
			};
			
			FP.Screen.Draw(line, PrimitiveType.Lines);
		}
		
		
		/// <summary>
		/// Draws a pixelated, non-antialiased line.
		/// </summary>
		/// <param name="start">Starting position.</param>
		/// <param name="end">Ending position</param>
		/// <param name="color">Color of the line.</param>
		/// <param name="alpha">Alpha of the line. Defaults to 1</param>
		public static void Line(Vector2f start, Vector2f end, Color color, float alpha = 1)
		{
			Line(start.X, start.Y, end.X, end.Y, color, alpha);
		}
		
		//	TODO:	everything else
		
		
		
		
		public static void Rect(float x, float y, int width, int height, SFML.Graphics.Color color)
		{
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x, y), color));
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x + width, y), color));
			
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x + width, y), color));
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x + width, y + height), color));
			
			
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x + width, y + height), color));
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x, y + height), color));
			
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x, y + height), color));
			verts.Append(new SFML.Graphics.Vertex(new Vector2f(x, y), color));
		}
	}
}
