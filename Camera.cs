/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/11/2013
 * Time: 6:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Window;

namespace Punk
{
	/// <summary>
	/// Camera class. Allows rotation, scale and positioning of the render view.
	/// Also supports viewports for split-screen, minimaps, etc.
	/// </summary>
	public class Camera
	{
		public Camera()
		{
			X = FP.HalfWidth;
			Y = FP.HalfHeight;
			Zoom = 1;
		}
		/// <summary>
		/// Position of the camera.
		/// </summary>
		public float X, Y;
		
		/// <summary>
		/// Rotation of the camera.
		/// </summary>
		public float Angle;
		
		/// <summary>
		/// Zoom of the camera.
		/// </summary>
		public float Zoom
		{
			get { return _zoom; }
			set
			{
				_zoom = Math.Max(value, 0.0001f);
			}
		}
		
		internal Vector2f TopRight
		{
			get
			{
				return new Vector2f(Right, Top);
			}
		}
		
		internal float Right
		{
			get
			{
				return X - (Zoom * FP.HalfWidth);
			}
		}
		
		internal float Top
		{
			get
			{
				return Y - (Zoom * FP.HalfHeight);
			}
		}
		
		private float _zoom;
	}
}
