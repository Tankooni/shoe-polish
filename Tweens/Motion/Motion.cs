/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/28/2013
 * Time: 12:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk.Tweens.Misc;

namespace Punk.Tweens.Motion
{
	/// <summary>
	/// Description of Motion.
	/// </summary>
	public class Motion : Tween
	{
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		/// <param name="ease">Optional easer function.</param>
		protected Motion(float duration, OnComplete complete = null, uint type = 0, Easer ease = null) :
		base(duration, type, complete, ease)
		{
		}
		
		/// <summary>
		/// Current X position of the tween
		/// </summary>
		public float X
		{
			get { return _x; }
			set
			{
				_x = value;
				if (_object != null)
					_xinfo.Value = _x;
			}
		}
		
		/// <summary>
		/// Current Y position of the tween
		/// </summary>
		public float Y
		{
			get { return _y; }
			set
			{
				_y = value;
				if (_object != null)
					_yinfo.Value = _y;
			}
		}
		
		/// <summary>
		/// Target object for the tween. Must have an X and a Y property.
		/// </summary>
		public object Object
		{
			get { return _object; }
			set
			{
				_object = value;
				if (_object != null)
				{
					_xinfo = new VarTweenInfo(_object, "X");
					_yinfo = new VarTweenInfo(_object, "Y");
					
					_xinfo.Value = _x;
					_yinfo.Value = _y;
				}
			}
		}
		
		protected float _x;
		protected float _y;
		
		internal VarTweenInfo _xinfo;
		internal VarTweenInfo _yinfo;
			
		protected object _object;
		
	}
}