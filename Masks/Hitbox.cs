/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/21/2013
 * Time: 3:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using Punk.Masks;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Masks
{
	/// <summary>
	/// Uses parent's hitbox to determine collision. This class is used
	/// internally by FlashPunk, you don't need to use this class becaus
	/// this is the default behaviour of Entities without a Mask object.
	/// </summary>
	public class Hitbox : Mask
	{
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">Width of the hitbox.</param>
		/// <param name="height">Width of the hitbox.</param>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public Hitbox(int width = 1, int height = 1, int x = 0, int y = 0) 
		{
			_width = width;
			_height = height;
			_x = x;
			_y = y;
			
			_check[typeof(Mask)] = CollideMask;
			_check[typeof(Hitbox)] = CollideHitbox;
		}
		
		/// <summary>
		/// Collides against an Entity.
		/// </summary>
		/// <param name="other">The Mask to collide agains.</param>
		/// <returns>Whether this collides with the Mask.</returns>
		protected override bool CollideMask(Mask other)
		{
			return Parent.X + _x + _width > other.Parent.X - other.Parent.OriginX
				&& Parent.Y + _y + _height > other.Parent.Y - other.Parent.OriginY
				&& Parent.X + _x < other.Parent.X - other.Parent.OriginX + other.Parent.Width
				&& Parent.Y + _y < other.Parent.Y - other.Parent.OriginY + other.Parent.Height;
		}
		
		/// <summary>
		/// Collides against a Hitbox.
		/// </summary>
		/// <param name="other">The Hitbox to collide against.</param>
		/// <returns>Whether this collides with the Hitbox.</returns>
		protected virtual bool CollideHitbox(Mask other)
		{
			//	dumb dumb dumb
			var hitbox = (other as Hitbox);
			Debug.Assert(hitbox != null);
			
			return Parent.X + _x + _width > hitbox.Parent.X + hitbox._x
				&& Parent.Y + _y + _height > hitbox.Parent.Y + hitbox._y
				&& Parent.X + _x < hitbox.Parent.X + hitbox._x + hitbox._width
				&& Parent.Y + _y < hitbox.Parent.Y + hitbox._y + hitbox._height;
		}
		
		/// <summary>
		/// X offset.
		/// </summary>
		public int X
		{
			get
			{
				return _x;
			}
			
			set
			{
				if (_x == value) return;
				_x = value;
				Update();
			}
		}
		
		/// <summary>
		/// Y offset.
		/// </summary>
		public int Y
		{
			get
			{
				return _y;
			}
			
			set
			{
				if (_y == value) return;
				_y = value;
				Update();
			}
		}
		
		/// <summary>
		/// Width.
		/// </summary>
		public int Width
		{
			get
			{
				return _width;
			}
			
			set
			{
				if (_width == value) return;
				_width = value;
				Update();
			}
		}
		
		/// <summary>
		/// Height.
		/// </summary>
		public int Height
		{
			get
			{
				return _height;
			}
			
			set
			{
				if (_height == value) return;
				_height = value;
				Update();
			}
		}
		
		/// <summary>
		/// Updates the parent's bounds for this mask.
		/// </summary>
		public override void Update()
		{
			if (List != null)
			{
				// update parent list
				List.Update();
			}
			else if (Parent != null)
			{
				// update entity bounds
				Parent.OriginX = -_x;
				Parent.OriginY = -_y;
				Parent.Width = _width;
				Parent.Height = _height;
			}
		}
		
		internal int _width;
		internal int _height;
		internal int _x;
		internal int _y;

	}
}
