/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/13/2013
 * Time: 4:12 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using Fasterflect;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Tweens a numeric public property of an Object.
	/// </summary>
	public class VarTween : Tween
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public VarTween(OnComplete complete = null, uint type = Tweener.PERSIST) : base(0, type, complete)
		{
		}
		
		/// <summary>
		/// Tweens a numeric property. If this method has already been called, the values will be overwritten.
		/// </summary>
		/// <param name="obj">The object containing the property.</param>
		/// <param name="property">The name of the property.</param>
		/// <param name="to">Value to tween to.</param>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="ease">Optional easer function.</param>
		public void Tween(object obj, string property, float to, float duration, Easer ease = null)
		{
			_info = new VarTweenInfo(obj, property);
			
			_start = _info.Value;
			_range = to - _start;
			_target = duration;
			_ease = ease;
			
			Start();
		}
		
		/// <summary>
		/// Updates the tween.
		/// </summary>
		public override void Update()
		{
			base.Update();
			_info.Value = _start + _range * _t;
		}
		
		private VarTweenInfo _info;
		private float _start;
		private float _range;
	}
}
