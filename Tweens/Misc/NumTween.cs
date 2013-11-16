/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/21/2013
 * Time: 7:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Tweens a numeric value.
	/// </summary>
	public class NumTween : Tween
	{
		/// <summary>
		/// The current value.
		/// </summary>
		public float Value = 0;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public NumTween(OnComplete complete = null, uint type = PERSIST) :
		base(0, type, complete, null)
		{
		}
		
		/// <summary>
		/// Tweens the value from one value to another.
		/// </summary>
		/// <param name="fromValue">Start value.</param>
		/// <param name="toValue">End value.</param>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="ease">Optional easer function.</param>
		public void Tween(float fromValue, float toValue, float duration, Easer ease = null)
		{
			_start = Value = fromValue;
			_range = toValue - Value;
			_target = duration;
			_ease = ease;
			Start();
		}
		
		/// <summary>
		/// Updates the Tween
		/// </summary>
		public override void Update()
		{
			base.Update();
			Value = _start + _range * _t;
		}
		
		// Tween information.
		private float _start;
		private float _range;
	}
}
