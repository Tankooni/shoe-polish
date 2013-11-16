/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/21/2013
 * Time: 6:44 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// A simple alarm, useful for timed events, etc.
	/// </summary>
	public class Alarm : Tween
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="duration">Duration of the alarm.</param>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public Alarm(float duration, OnComplete complete = null, uint type = Tween.PERSIST) :
		base(duration, type, complete, null)
		{
		}
		
		/// <summary>
		/// Sets the alarm.
		/// </summary>
		/// <param name="duration">Duration of the alarm.</param>
		public void Reset(float duration)
		{
			_target = duration;
			Start();
		}
		
		/// <summary>
		/// How much time has passed since reset.
		/// </summary>
		public float Elapsed
		{
			get
			{
				return _time;
			}
		}
		
		/// <summary>
		/// Current alarm duration.
		/// </summary>
		public float Duration
		{
			get
			{
				return _target;
			}
		}
		
		/// <summary>
		/// Time remaining on the alarm.
		/// </summary>
		public float Remaining
		{
			get
			{
				return _target - _time;
			}
		}
	}
}
