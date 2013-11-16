/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 8:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk
{
	/// <summary>
	/// Updateable Tween container.
	/// </summary>
	public class Tweener
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Tweener()
		{
		}
		
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
		public bool Active = true;

		/// <summary>
		/// If the Tweener should clear on removal. For Entities, this is when they are
		/// removed from a World, and for World this is when the active World is switched.
		/// </summary>
		public bool AutoClear = false;
		
		/// <summary>
		/// Updates the Tween container
		/// </summary>
		public virtual void Update()
		{
		}
		
		/// <summary>
		/// Adds a new Tween.
		/// </summary>
		/// <param name="t">The Tween to add.</param>
		/// <param name="start">If the Tween should call start() immediately.</param>
		/// <returns>The added Tween.</returns>
		public Tween AddTween(Tween t, bool start = false)
		{
			if (t._parent != null)
			{
				throw new Exception("Cannot add a Tween object more than once.");
			}
			
			t._parent = this;
			t._next = _tween;
			if (_tween != null)
			{
				_tween._prev = t;
			}
			
			_tween = t;
			
			if (start)
			{
				_tween.Start();
			}
			
			return t;
		}
		
		/// <summary>
		/// The Tween to remove.
		/// </summary>
		/// <param name="t">The Tween to remove.</param>
		/// <returns>The removed Tween</returns>
		public Tween RemoveTween(Tween t)
		{
			if (!ReferenceEquals(t._parent, this))
			{
				throw new Exception("Core object does not contain Tween.");
			}
			
			if (t._next != null)
			{
				t._next._prev = t._prev;
			}
			
			if (t._prev != null)
			{
				t._prev._next = t._next;
			}
			else
			{
				_tween = t._next;
			}
			
			t._next = t._prev = null;
			t._parent = null;
			t.Active = false;
			return t;
		}
		
		/// <summary>
		///	Removes all Tweens.
		/// </summary>
		public void ClearTweens()
		{
			Tween t= _tween;
			Tween n;
			
			while (t != null)
			{
				n = t._next;
				RemoveTween(t);
				t = n;
			}
		}
		
		/// <summary>
		/// Updates all contained tweens.
		/// </summary>
		public void UpdateTweens()
		{
			Tween t = _tween;
			Tween n;
			
			while (t != null)
			{
				n = t._next;
				if (t.Active)
				{
					t.Update();
					if (t._finish)
					{
						t.Finish();
					}
				}
				t = n;
			}
		}
		
		#region List
		internal Tween _tween;
		#endregion
	}
}
