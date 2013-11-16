/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/18/2013
 * Time: 7:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Punk.Graphics
{
	/// <summary>
	/// A Graphic that can contain multiple Graphics of one or various types.
	/// Useful for drawing sprites with multiple different parts, etc.
	/// </summary>
	public class Graphiclist : Graphic
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="graphics">Graphic objects to add to the list.</param>
		public Graphiclist(params Graphic[] graphics)
		{
			_graphics = new List<Graphic>();
			_camera = new Camera();
			
			foreach (Graphic g in graphics)
			{
				Add(g);
			}
		}
		
		/// <summary>
		/// Updates the graphics in the list.
		/// </summary>
		public override void Update()
		{
			base.Update();
			
			foreach (Graphic g in _graphics)
			{
				if (g.Active)
				{
					g.Update();
				}
			}
		}
		
		/// <summary>
		/// Get the first graphic of a type.
		/// </summary>
		/// <returns>The first graphic found, or null if none exist.</returns>
		public T First<T>() where T : Graphic
		{
			foreach (var graphic in _graphics)
			{
				if (graphic is T)
					return graphic as T;
			}
			
			return null;
		}
		
		/// <summary>
		/// Render the Graphic.
		/// </summary>
		/// <param name="x">X position of the owning Entity.</param>
		/// <param name="y">Y position of the owning Entity.</param>
		public override void Render(float x, float y, Camera camera)
		{
			x = X + x;
			y = Y + y;
			
			foreach (Graphic g in _graphics)
			{
				if (g.Visible)
				{
					if (g.Relative)
					{
						g.Render(x, y, camera);
					}
					else
					{
						g.Render(0, 0, camera);
					}
				}
			}
		}
		
		/// <summary>
		/// Adds the Graphic to the list.
		/// </summary>
		/// <param name="graphic">The Graphic to add.</param>
		/// <returns>The added Graphic.</returns>
		public Graphic Add(Graphic graphic)
		{
			_graphics.Add(graphic);
			if (!Active)
			{
				Active = graphic.Active;
			}
			
			return graphic;
		}
		
		/// <summary>
		/// Removes the Graphic from the list.
		/// </summary>
		/// <param name="graphic">The Graphic to remove.</param>
		/// <returns>The removed Graphic.</returns>
		public Graphic Remove(Graphic graphic)
		{
			_graphics.Remove(graphic);
			return graphic;
		}
		
		/// <summary>
		/// Removes the Graphic from the position in the list.
		/// </summary>
		/// <param name="index">Index to remove.</param>
		public void RemoveAt(int index = 0)
		{
			if (_graphics.Count == 0)
			{
				return;
			}
			
			index %= _graphics.Count;
			Remove(_graphics[index % _graphics.Count]);
			
			UpdateCheck();
		}
		
		/// <summary>
		/// Removes all Graphics from the list.
		/// </summary>
		public void RemoveAll()
		{
			_graphics.Clear();
			Active = false;
		}
		
		/// <summary>
		/// All Graphics in this list.
		/// </summary>
		public List<Graphic> Children
		{
			get
			{
				return _graphics;
			}
		}
		
		/// <summary>
		/// Amount of Graphics in this list.
		/// </summary>
		public int Count
		{
			get
			{
				return _graphics.Count;
			}
		}
		
		/// <summary>
		/// Check if the Graphiclist should update.
		/// </summary>
		private void UpdateCheck()
		{
			Active = false;
			foreach (Graphic g in _graphics)
			{
				if (g.Active)
				{
					Active = true;
					return;
				}
			}
		}
		
		private List<Graphic> _graphics;
		private Camera _camera;
	}
}
