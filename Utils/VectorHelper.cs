/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 10:38 AM
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
	/// Helper class that adds some common math functions to SFML's Vector2f type.
	/// </summary>
	public static class VectorHelper
	{
		/// <summary>
		/// Computes the dot product of two vectors.
		/// </summary>
		/// <param name="vec">Self</param>
		/// <param name="other">The other vector to compute with.</param>
		/// <returns>The dot product of the two vectors.</returns>
		public static float Dot(this Vector2f vec, Vector2f other)
		{
			float result = 0;
			
            result += vec.X * other.X;
            result += vec.Y * other.Y;

            return result;
		}
		
		/// <summary>
		/// Get the length of a vector
		/// </summary>
		/// <param name="vec">Self</param>
		/// <returns>The length of the vector</returns>
		public static float Length(this Vector2f vec)
		{
			return (float) Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
		}
		
		/// <summary>
		/// Scales the line segment between (0,0) and the current point to a set length. 
		/// </summary>
		/// <param name="vec">Self</param>
		/// <param name="amount">The length to scale the vector to</param>
		/// <returns>The normalized vector</returns>
		public static Vector2f Normalized(this Vector2f vec, float amount = 1)
		{
			if (vec.IsZero())
			{
				return vec;
			}
			
			double val = 1.0 / vec.Length();
			var x = vec.X * val;
			var y = vec.Y * val;
			
			x *= amount;
			y *= amount;
			
			vec.X = (float) x;
			vec.Y = (float) y;
			
			return vec;
		}
		
		/// <summary>
		/// If the vector is a zero vector.
		/// </summary>
		/// <param name="vec">Self</param>
		/// <returns>Whether the vector is a zero vector.</returns>
		public static bool IsZero(this Vector2f vec)
		{
			return (vec.X == 0 && vec.Y == 0);
		}
	}
}
