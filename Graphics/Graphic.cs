/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/3/2013
 * Time: 4:22 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Graphics;

namespace Punk.Graphics
{
	/// <summary>
	/// Base class for all graphical types that can be drawn by Entity.
	/// </summary>
	public class Graphic
	{
		/// <summary>
		/// Callback for when the Graphic is assigned to an entity.
		/// </summary>
		public delegate void OnAssign();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public Graphic()
		{
		}
		
		/// <summary>
		/// If the graphic should update.
		/// </summary>
		public bool Active;
		
		/// <summary>
		/// If the graphic should render.
		/// </summary>
		public bool Visible = true;
		
		/// <summary>
		/// If the graphic should render.
		/// </summary>
		public float X = 0;
		
		/// <summary>
		/// Y offset.
		/// </summary>
		public float Y = 0;
		
		/// <summary>
		/// X scrollfactor, effects how much the camera offsets the drawn graphic.
		/// Can be used for parallax effect, eg. Set to 0 to follow the camera,
		/// 0.5 to move at half-speed of the camera, or 1 (default) to stay still.
		/// </summary>
		public float ScrollX = 1;
		
		/// <summary>
		/// Y scrollfactor, effects how much the camera offsets the drawn graphic.
		/// Can be used for parallax effect, eg. Set to 0 to follow the camera,
		/// 0.5 to move at half-speed of the camera, or 1 (default) to stay still.
		/// </summary>
		public float ScrollY = 1;
		
		/// <summary>
		/// If the graphic should render at its position relative to its parent Entity's position.
		/// </summary>
		public bool Relative = true;
		
		/// <summary>
		/// An optional shader to draw the image with
		/// </summary>
		public Shader Shader;
		
		/// <summary>
		/// Updates the graphic.
		/// </summary>
		public virtual void Update()
		{
		}
		
		/// <summary>
		/// Renders the graphic to the screen buffer.
		/// </summary>
		public virtual void Render(float x, float y, Camera camera)
		{
		}
		
		/// <summary>
		/// Callback for when the graphic is assigned to an Entity
		/// </summary>
		protected OnAssign Assign
		{
			get
			{
				return _assign;
			}
			
			set
			{
				_assign = value;
			}
		}
		
		#region Graphic information
		internal OnAssign _assign;
		#endregion
	}
}
