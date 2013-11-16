/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/13/2013
 * Time: 12:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Window;

namespace Punk.Utils
{
	/// <summary>
	/// Description of Axis.
	/// </summary>
	public class Axis
	{
		public float X { get; internal set;}
		public float Y { get; internal set;}
		
		public Axis()
		{
		}
		
		internal void Update(float x, float y, float deadzone, float length = 0)
		{
			X = Math.Abs(x) > deadzone ? x / 100f: 0.0f;
			Y = Math.Abs(y) > deadzone ? y / 100f : 0.0f;
			
			if (length > 0)
			{
				Normalize(length);
			}
		}
		
		internal void Normalize(float length)
		{
			var vec = new Vector2f(X, Y);
			vec = vec.Normalized(length);
			X = vec.X;
			Y = vec.Y;
		}
	}
}
