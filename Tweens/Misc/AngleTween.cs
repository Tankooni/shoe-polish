/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/21/2013
 * Time: 9:23 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Tweens from one angle to another.
	/// </summary>
	public class AngleTween : Tween
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public AngleTween(OnComplete complete = null, uint type = Tweener.PERSIST) : 
		base(0, type, complete, null)
		{
		}
		
		/// <summary>
		/// Tweens the value from one angle to another.
		/// </summary>
		/// <param name="fromAngle">Start angle.</param>
		/// <param name="toAngle">End angle.</param>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="ease">Optional easer function.</param>
		public void Tween(float fromAngle, float toAngle, float duration, Easer ease = null)
		{
			_start = Angle = fromAngle;
			float d = toAngle - Angle;
			float a = Math.Abs(d);
			
			if (a > 181)
			{
				_range = (360 - a) * (d > 0 ? -1 : 1);
			}
			else if (a < 179)
			{
				_range = d;
			}
			else
			{
				_range = FP.Choose(180, -180);
			}
			
			_target = duration;
			_ease = ease;
			Start();
		}
		
		/// <summary>
		/// Updates the Tween.
		/// </summary>
		public override void Update()
		{
			base.Update();
			Angle = (_start + _range * _t) % 360;
			
			if (Angle < 0)
			{
				Angle += 360;
			}
		}
		
		/// <summary>
		/// The tweened angle.
		/// </summary>
		public float Angle;
		
		// Tween information.
		private float _start;
		private float _range;
	}
}
