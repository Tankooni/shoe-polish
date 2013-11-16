/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/7/2013
 * Time: 3:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk.Graphics
{
	/// <summary>
	/// Description of Particle.
	/// </summary>
	public class Particle
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Particle()
		{
		}
		
		// Particle information.
		internal ParticleType _type;
		internal float _time;
		internal float _duration;
		
		// Motion information.
		internal float _x;
		internal float _y;
		internal float _moveX;
		internal float _moveY;
		
		// Gravity information.
		internal float _gravity;
		
		// List information.
		internal Particle _prev;
		internal Particle _next;
		
	}
}
