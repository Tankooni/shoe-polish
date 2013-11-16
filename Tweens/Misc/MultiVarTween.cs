/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/14/2013
 * Time: 4:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using Fasterflect;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Tweens multiple numeric public properties of an Object simultaneously.
	/// </summary>
	public class MultiVarTween : Tween
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public MultiVarTween(OnComplete complete = null, uint type = Tweener.PERSIST) : base(0, type, complete)
		{
			_vars = new List<VarTweenInfo>();
			_range = new List<float>();
			_start = new List<float>();
		}
		
		/// <summary>
		/// The duration of the tween.
		/// If this is set after calling Tween(), existing tweens will be canceled.
		/// </summary>
		void StopAll()
		{
			//	FIXME:	Maybe this should use Cancel() ?
			_vars.Clear();
			_range.Clear();
			_start.Clear();
		}
		
		/// <summary>
		/// Tweens multiple numeric properties
		/// </summary>
		/// <param name="obj">The object containing the properties. </param>
		/// <param name="values">
		/// An anonymous type object containing key/value pairs of properties and target values.
		/// Example: new { X = 100, Y = 50 }
		/// </param>
		/// <param name="duration">Duration of the tween. </param>
		/// <param name="ease">Optional Easer function.</param>
		/// <param name="delay"></param>
		public void Tween(object obj, object values, float duration, Easer ease = null, float delay = 0)
		{
			_object = obj;
			
			foreach (PropertyInfo property in values.GetType().GetProperties())
			{
				var info = new VarTweenInfo(_object, property.Name);
				var to = new VarTweenInfo(values, property.Name, VarTweenInfo.Options.Read);
			
				float start = info.Value;
				float range = to.Value - start;
				
				_vars.Add(info);
				_start.Add(start);
				_range.Add(range);
			}
			
			//	FIXME:	delay does nothing!
			_target = duration;
			_ease = ease;
			
			Start();
		}
		
		/// <summary>
		/// Update the tween.
		/// </summary>
		public override void Update()
		{
			base.Update();

			int i = _vars.Count;			
			while (i --> 0)
			{
				_vars[i].Value = _start[i] + _range[i] * _t;
			}
		}
		
		private List<VarTweenInfo> _vars;
		private List<float> _start;
		private List<float> _range;
		private object _object;
	}
}
