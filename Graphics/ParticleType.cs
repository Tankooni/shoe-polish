/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/7/2013
 * Time: 3:13 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Graphics;

namespace Punk.Graphics
{
	/// <summary>
	/// Template used to define a particle type used by the Emitter class. Instead
	/// of creating this object yourself, fetch one with Emitter's NewType() function.
	/// </summary>
	public class ParticleType
	{	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the particle type.</param>
		/// <param name="frames">Array of frame indices to animate through.</param>
		/// <param name="source">Source image.</param>
		/// <param name="frameWidth">Frame width.</param>
		/// <param name="frameHeight">Frame height.</param>
		public ParticleType(string name, int[] frames, Texture source, int frameWidth, int frameHeight)
		{
			_name = name;
			_source = source;
			_width = source.Size.X;
			_frame = new IntRect(0, 0, frameWidth, frameHeight);
			_frames = frames;
			_frameCount = frames.Length;
		}
		
		/// <summary>
		/// Defines the motion range for this particle type.
		/// </summary>
		/// <param name="angle">Launch Direction.</param>
		/// <param name="distance">Distance to travel.</param>
		/// <param name="duration">Particle duration.</param>
		/// <param name="angleRange">Random amount to add to the particle's direction.</param>
		/// <param name="distanceRange">Random amount to add to the particle's distance.</param>
		/// <param name="durationRange">Random amount to add to the particle's duration.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>This ParticleType object.</returns>
		public ParticleType SetMotion(float angle, float distance, float duration, float angleRange = 0, float distanceRange = 0, float durationRange = 0, Emitter.Easer ease = null)
		{
			_angle = angle * FP.RAD;
			_distance = distance;
			_duration = duration;
			_angleRange = angleRange * FP.RAD;
			_distanceRange = distanceRange;
			_durationRange = durationRange;
			_ease = ease;
			return this;
		}
		
		/// <summary>
		/// Defines the motion range for this particle type based on the vector.
		/// </summary>
		/// <param name="x">X distance to move.</param>
		/// <param name="y">Y distance to move.</param>
		/// <param name="duration">Particle duration.</param>
		/// <param name="durationRange">Random amount to add to the particle's duration.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>This ParticleType object.</returns>
		public ParticleType SetMotionVector(float x, float y, float duration, float durationRange = 0, Emitter.Easer ease = null)
		{
			_angle = (float) Math.Atan2(y, x);
			_angleRange = 0;
			_duration = duration;
			_durationRange = durationRange;
			_ease = ease;
			return this;
		}
		
		/// <summary>
		/// Sets the gravity range of this particle type.
		/// </summary>
		/// <param name="gravity">Gravity amount to affect to the particle y velocity.</param>
		/// <param name="gravityRange">Random amount to add to the particle's gravity.</param>
		/// <returns>This ParticleType object.</returns>
		public ParticleType SetGravity(float gravity = 0, float gravityRange = 0)
		{
			_gravity = gravity;
			_gravityRange = gravityRange;
			return this;
		}
		
		/// <summary>
		/// Sets the alpha range of this particle type.
		/// </summary>
		/// <param name="start">The starting alpha.</param>
		/// <param name="finish">The finish alpha.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>This ParticleType object.</returns>
		public ParticleType SetAlpha(float start = 1, float finish = 0, Emitter.Easer ease = null)
		{
			start = start < 0 ? 0 : (start > 1 ? 1 : start);
			finish = finish < 0 ? 0 : (finish > 1 ? 1 : finish);
			_alpha = start;
			_alphaRange = finish - start;
			_alphaEase = ease;
			return this;
		}
		
		/// <summary>
		/// Sets the color range of this particle type.
		/// </summary>
		/// <param name="start">The starting color.</param>
		/// <param name="finish">The finish color.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>This ParticleType object.</returns>
		public ParticleType SetColor(Color start, Color finish, Emitter.Easer ease = null)
		{
			uint s = FP.HexColor(start);
			uint f = FP.HexColor(finish);
			
			s &= 0xFFFFFF;
			f &= 0xFFFFFF;
			_red = (s >> 16 & 0xFF) / 255.0f;
			_green = (s >> 8 & 0xFF) / 255.0f;
			_blue = (s & 0xFF) / 255.0f;
			_redRange = (f >> 16 & 0xFF) / 255.0f - _red;
			_greenRange = (f >> 8 & 0xFF) / 255.0f - _green;
			_blueRange = (f & 0xFF) / 255.0f - _blue;
			
			_colorEase = ease;
			return this;
		}
		
		// Particle information.
		internal string _name;
		internal Texture _source;
		internal uint _width;
		internal IntRect _frame;
		internal int[] _frames;
		internal int _frameCount;
		
		// Motion information.
		internal float _angle;
		internal float _angleRange;
		internal float _distance;
		internal float _distanceRange;
		internal float _duration;
		internal float _durationRange;
		internal Emitter.Easer _ease;
		
		// Gravity information.
		internal float _gravity = 0;
		internal float _gravityRange = 0;
		
		// Alpha information.
		internal float _alpha = 1;
		internal float _alphaRange = 0;
		internal Emitter.Easer _alphaEase;
		
		// Color information.
		internal float _red = 1;
		internal float _redRange = 0;
		internal float _green = 1;
		internal float _greenRange = 0;
		internal float _blue = 1;
		internal float _blueRange = 0;
		internal Emitter.Easer _colorEase;
		
//		// Buffer information.
//		internal _buffer:BitmapData;
//		internal _bufferRect:Rectangle;
	}
}
