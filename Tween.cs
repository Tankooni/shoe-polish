/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 10:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk
{
	/// <summary>
	/// Base class for all Tween objects, can be added to any Core-extended classes.
	/// </summary>
	public class Tween
	{
		/// <summary>
		/// Callback type for when the tween completes.
		/// </summary>
		public delegate void OnComplete();
		
		/// <summary>
		/// Mathematical easer function type.
		/// Can be used to add effects to the tween motion.
		/// </summary>
		public delegate float Easer(float t);
		
		/// <summary>
		/// Persistent Tween type, will stop when it finishes.
		/// </summary>
		public const uint PERSIST = 0;
		
		///	<summary>
		/// Looping Tween type, will restart immediately when it finishes.
		/// </summary>
		public const uint LOOPING = 1;
		
		/// <summary>
		/// Oneshot Tween type, will stop and remove itself from its core container when it finishes.
		/// </summary>
		public const uint ONESHOT = 2;
		
		/// <summary>
		/// If the Tweener should update.
		/// </summary>
		public bool Active = false;
		
		/// <summary>
		/// Tween completion callback
		/// </summary>
		public OnComplete Complete;
		
		/// <summary>
		/// Constructor. Specify basic information about the Tween.
		/// </summary>
		/// <param name="duration">Duration of the tween (in seconds or frames).</param>
		/// <param name="type">Tween type, one of Tween.PERSIST (default), Tween.LOOPING, or Tween.ONESHOT.</param>
		/// <param name="complete">Optional callback for when the Tween completes.</param>
		/// <param name="ease">Optional easer function to apply to the Tweened value.`</param>
		public Tween(float duration = 0, uint type = 0, OnComplete complete = null, Easer ease = null)
		{
			_target = duration;
			_type = type;
			Complete = complete;
			_ease = ease;
		}
		
		/// <summary>
		/// Updates the Tween, called by World.
		/// </summary>
		public virtual void Update()
		{
			_time += FP.TimeInFrames ? 1 : FP.Elapsed;
			_t = _time / _target;
			
			if (_time >= _target)
			{
				_t = 1;
				_finish = true;
			}
			
			if (_ease != null)
			{
				_t = _ease(_t);
			}
		}
		
		/// <summary>
		/// Starts the Tween, or restarts it if it's currently running.
		/// </summary>
		public virtual void Start()
		{
			_time = 0;
			if (_target == 0)
			{
				Active = false;
				return;
			}
			
			Active = true;
		}
		
		/// <summary>
		/// Immediately stops the Tween and removes it from its Tweener without calling the complete callback.
		/// </summary>
		public void Cancel()
		{
			Active = false;
			if (_parent != null)
			{
				_parent.RemoveTween(this);
			}
		}
		
		/// <summary>
		/// Called when the Tween completes.
		/// </summary>
		internal void Finish()
		{
			switch (_type)
			{
				case PERSIST:
					_time = _target;
					Active = false;
					break;
				case LOOPING:
					_time %= _target;
					_t = _time / _target;
					if (_ease != null) _t = _ease(_t);
					Start();
					break;
				case ONESHOT:
					_time = _target;
					Active = false;
					_parent.RemoveTween(this);
					break;
			}
			
			_finish = false;
			
			if (Complete != null)
			{
				Complete();
			}
		}
		
		/// <summary>
		/// The completion percentage of the Tween.
		/// </summary>
		public float Percent
		{
			get
			{
				return _time / _target;
			}
			
			set
			{
				_time = _target * value;
			}
		}
		
		/// <summary>
		/// The current time scale of the Tween (after easer has been applied).
		/// </summary>
		public float Scale
		{
			get
			{
				return _t;
			}
		}
		
		#region Tween information
		
		/// <summary>
		/// The type of tween (Oneshot, Persist or Looping)
		/// </summary>
		protected uint _type;
		
		/// <summary>
		/// The easer function to apply, if any.
		/// </summary>
		protected Easer _ease;
		
		/// <summary>
		/// The Tween's update percentage.
		/// </summary>
		protected float _t = 0;
		#endregion
		
		#region Timing information.
		
		/// <summary>
		/// How long the Tween has been running since it last restarted.
		/// </summary>
		protected float _time;
		
		/// <summary>
		/// The duration of the Tween.
		/// </summary>
		protected float _target;
		#endregion
		
		#region List information.
		internal bool _finish;
		internal Tweener _parent;
		internal Tween _prev;
		internal Tween _next;
		#endregion
	}
}
