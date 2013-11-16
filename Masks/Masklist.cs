/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/4/2013
 * Time: 12:18 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Punk.Masks
{
	/// <summary>
	/// A Mask that can contain multiple Masks of one or various types.
	/// </summary>
	public class Masklist : Hitbox
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="masks">Masks to add to the list.</param>
		public Masklist(params Mask[] masks)
		{
			_masks = new List<Mask>();
			
			foreach (Mask m in masks)
			{
				Add(m);
			}
		}
		
		/// <summary>
		/// Collide against a mask.
		/// </summary>
		/// <param name="mask">The mask to collide against.</param>
		/// <returns>Whether this returns with the Mask.</returns>
		public override bool Collide(Mask mask)
		{
			foreach (Mask m in _masks)
			{
				if (m.Collide(mask))
				{
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Collide against a Masklist.
		/// </summary>
		/// <param name="other">The Masklist to collide with.</param>
		/// <returns>Whether this returns with the Masklist.</returns>
		protected override bool CollideMasklist(Mask other)
		{
			//	C# is stupid. This really shouldn't be necessary.
			Debug.Assert(other is Masklist);
			var list = other as Masklist;
			
			foreach (Mask a in _masks)
			{
				foreach (Mask b in list._masks)
				{
					if (a.Collide(b))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Adds a Mask to the list.
		/// </summary>
		/// <param name="mask">The Mask to add.</param>
		/// <returns>The added Mask.</returns>
		public Mask Add(Mask mask)
		{
			_masks.Add(mask);
			mask.List = this;
			mask.Parent = Parent;
			Update();
			
			return mask;
		}
		
		/// <summary>
		/// Removes a Mask from the list.
		/// </summary>
		/// <param name="mask">The Mask to remove.</param>
		/// <returns>The removed Mask.</returns>
		public Mask Remove(Mask mask)
		{
			if (_masks.Contains(mask))
			{
				mask.List = null;
				mask.Parent = null;
				_masks.Remove(mask);
				Update();
			}
			
			return mask;
		}
		
		/// <summary>
		/// Removes the Mask at the index.
		/// </summary>
		/// <param name="index">The Mask index.</param>
		public void RemoveAt(int index)
		{
			index %= _masks.Count;
			Mask m = _masks[index];
			m.List = null;
			_masks.RemoveAt(index);
			Update();
		}
		
		/// <summary>
		/// Removes all Masks from the list.
		/// </summary>
		public void RemoveAll()
		{
			foreach (Mask m in _masks)
			{
				m.List = null;
			}
			
			_masks.Clear();
			Update();
		}
		
		/// <summary>
		/// Gets a Mask from the list.
		/// </summary>
		/// <param name="index">The Mask index.</param>
		/// <returns>The Mask at the index.</returns>
		public Mask GetMask(int index = 0)
		{
			return _masks[index % _masks.Count];
		}
		
		/// <summary>
		/// Assigns the mask to the parent.
		/// </summary>
		/// <param name="parent"></param>
		public override void AssignTo(Entity parent)
		{
			foreach (Mask m in _masks)
			{
				m.AssignTo(parent);
			}
			
			base.AssignTo(parent);
			
		}
		
		/// <summary>
		/// Updates the parent's bounds for this mask.
		/// </summary>
		public override void Update()
		{
			int t = 0, l = 0, r = 0, b = 0, i = Count;
			Hitbox h;
			
			while (i -- != 0)
			{
				if ((h = _masks[i] as Hitbox) != null)
				{
					if (h._x < l) l = h._x;
					if (h._y < t) t = h._y;
					if (h._x + h._width > r) r = h._x + h._width;
					if (h._y + h._height > b) b = h._y + h._height;
				}
			}
			
			// update hitbox bounds
			_x = l;
			_y = t;
			_width = r - l;
			_height = b - t;
			
			base.Update();
		}
		
		/// <summary>
		/// Used to render debug information in console.
		/// </summary>
		public override void RenderDebug(SFML.Graphics.VertexArray vertexArray)
		{
			foreach (Mask m in _masks)
			{
				m.RenderDebug(vertexArray);
			}
		}
		
		/// <summary>
		/// Amount of Masks in the list.
		/// </summary>
		public int Count
		{
			get
			{
				return _masks.Count;
			}
		}
		
		/// <summary>
		/// All masks contained by this.
		/// </summary>
		private List<Mask> _masks;
	}
}
