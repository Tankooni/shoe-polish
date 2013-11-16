/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 4:26 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Punk.Masks
{
	/// <summary>
	/// Base class for Entity collision masks.
	/// </summary>
	public class Mask
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Mask()
		{
			_check = new Dictionary<Type, Mask.CollideWithMask>();
			_class = GetType();
			
			_check[typeof(Mask)] = CollideMask;
			_check[typeof(Masklist)] = CollideMasklist;
		}
		
		/// <summary>
		/// The parent Entity of this mask.
		/// </summary>
		public Entity Parent;
		
		/// <summary>
		/// The parent Masklist of the mask.
		/// </summary>
		public Masklist List;
		
		/// <summary>
		/// Checks for collision with another Mask.
		/// </summary>
		/// <param name="mask">The other Mask to check against.</param>
		/// <returns>If the Masks overlap.</returns>
		public virtual bool Collide(Mask mask)
		{
			if (_check.ContainsKey(mask._class))
			{
				return _check[mask._class](mask);
			}
			
			if (_check.ContainsKey(_class))
			{
				return _check[_class](this);
			}
			
			return false;
		}
		
		/// <summary>
		/// Collide against a Mask.
		/// </summary>
		/// <param name="other">The Mask to collide with.</param>
		/// <returns>Whether this returns with the Mask.</returns>
		protected virtual bool CollideMask(Mask other)
		{
			return Parent.X - Parent.OriginX + Parent.Width > other.Parent.X - other.Parent.OriginX
				&& Parent.Y - Parent.OriginY + Parent.Height > other.Parent.Y - other.Parent.OriginY
				&& Parent.X - Parent.OriginX < other.Parent.X - other.Parent.OriginX + other.Parent.Width
				&& Parent.Y - Parent.OriginY < other.Parent.X - other.Parent.OriginY + other.Parent.Height;
		}
		
		/// <summary>
		/// Collide against a Masklist.
		/// C# is stupid, so you this method has to accept a Mask parameter and then check to see if it is a masklist.
		/// </summary>
		/// <param name="other">The Masklist to collide with.</param>
		/// <returns>Whether this returns with the Masklist.</returns>
		protected virtual bool CollideMasklist(Mask other)
		{
			Debug.Assert(other is Masklist);
			return other.Collide(this);
		}
		
		/// <summary>
		/// Assigns the mask to the parent.
		/// </summary>
		/// <param name="parent"></param>
		public virtual void AssignTo(Entity parent)
		{
			Parent = parent;
			if (List == null && parent != null)
			{
				Update();
			}
		}
		
		/// <summary>
		/// Updates the parent's bounds for this mask.
		/// </summary>
		public virtual void Update()
		{
		}
		
		/// <summary>
		/// Used to render debug information in console.
		/// </summary>
		/// <param name="vertexArray">The vertex array to add vertices to in order to display.</param>
		public virtual void RenderDebug(SFML.Graphics.VertexArray vertexArray)
		{
		}
		
		// Mask information.
		private Type _class;
		
		/// <summary>
		///	Callback type for checking mask collision.
		/// </summary>
		protected delegate bool CollideWithMask(Mask mask);
		
		/// <summary>
		/// Callback dictionary for setting up reactions between masks.
		/// </summary>
		protected Dictionary<Type, CollideWithMask> _check;
	}
}
