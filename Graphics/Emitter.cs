/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/7/2013
 * Time: 2:32 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Graphics
{
	/// <summary>
	/// Particle emitter used for emitting and rendering particle sprites.
	/// Good rendering performance with large amounts of particles.
	/// </summary>
	public class Emitter : Graphic
	{
		public delegate float Easer(float t);
		
		/// <summary>
		/// Constructor. Sets the source image to use for newly added particle types.
		/// </summary>
		/// <param name="source">Source texture.</param>
		/// <param name="frameWidth">Frame width.</param>
		/// <param name="frameHeight">Frame height.</param>
		public Emitter(Texture source, int frameWidth = 0, int frameHeight = 0)
		{
			SetSource(source, frameWidth, frameHeight);
			Active = true;
			
			_types = new Dictionary<string, ParticleType>();
			_vertexArray = new VertexArray(PrimitiveType.Quads, 1000);
		}
		
		/// <summary>
		/// Changes the source image to use for newly added particle types.
		/// </summary>
		/// <param name="source">Source image.</param>
		/// <param name="frameWidth">Frame width.</param>
		/// <param name="frameHeight">Frame height.</param>
		public void SetSource(Texture source, int frameWidth = 0, int frameHeight = 0)
		{
			if (source == null)
			{
				throw new Exception("Texture cannot be null!");
			}
			
			_source = source;
			_width = source.Size.X;
			_height = source.Size.Y;
			
			_frameWidth = frameWidth != 0 ? (uint) frameWidth : _width;
			_frameHeight = frameHeight != 0 ? (uint) frameHeight : _height;
			
			_frameCount = (uint)(_width / _frameWidth) * (uint)(_height / _frameHeight);
		}
		
		/// <summary>
		/// Update the graphic.
		/// </summary>
		public override void Update()
		{
			base.Update();
			
			// quit if there are no particles
			if (_particle == null)
			{
				return;
			}
			
			// particle info
			float e = FP.TimeInFrames ? 1 : FP.Elapsed;
			Particle p = _particle;
			Particle n = null;
			
			// loop through the particles
			while (p != null)
			{
				// update time scale
				p._time += e;
				
				// remove on time-out
				if (p._time >= p._duration)
				{
					if (p._next != null) p._next._prev = p._prev;
					if (p._prev != null) p._prev._next = p._next;
					else _particle = p._next;
					
					n = p._next;
					p._next = _cache;
					p._prev = null;
					_cache = p;
					p = n;
					_particleCount --;
					continue;
				}
				
				// get next particle
				p = p._next;
			}
		}
		
		/// <summary>
		/// Renders the particles.
		/// </summary>
		/// <param name="x">X position of the owning entity.</param>
		/// <param name="y">Y position of the owning entity.</param>
		public override void Render(float x, float y, Camera camera)
		{
			_vertexArray.Clear();
			
			// quit if there are no particles
			if (_particle == null) return;
			
			// get rendering position
//			_point.x = point.x + x - camera.x * ScrollX;
//			_point.y = point.y + y - camera.y * ScrollY;
			
			// particle info
			float t, td;
			Particle p = _particle;
			ParticleType type = null;
			
			Vector2f _p = new Vector2f();
			Vector2f _point = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
			
			var color = new Color();
			var states = new RenderStates(_source);
			
			// loop through the particles
			while (p != null)
			{
				// get time scale
				t = p._time / p._duration;
				
				// get particle type
				type = p._type;
				
				// get position
				td = (type._ease == null) ? t : type._ease(t);
				_p.X = _point.X + p._x + p._moveX * td;
				_p.Y = _point.Y + p._y + p._moveY * td;
				
				// stops particles from moving when gravity is enabled
				// and if emitter.Active = false (for game pausing for example)
				if (Active)
				{
					p._moveY += p._gravity * td;
				}
				
				// get frame
				float frameX = _frameWidth * type._frames[(int) FP.Clamp(td * type._frameCount, 0, type._frames.Length - 1)];
				float frameY = (int) (frameX / type._width) * _frameHeight;
				frameX %= (int) type._width;
				
				// get alpha
				float alphaT = (type._alphaEase == null) ? t : type._alphaEase(t);
				float a = type._alpha + type._alphaRange * alphaT;
				
				// get color
				td = (type._colorEase == null) ? t : type._colorEase(t);
				float r = type._red + type._redRange * td;
				float g = type._green + type._greenRange * td;
				float b = type._blue + type._blueRange * td;
				
				unchecked	//	screw you C#
				{
					color.R = (byte) (255 * r);
					color.G = (byte) (255 * g);
					color.B = (byte) (255 * b);
					color.A = (byte) (255 * a);
				}
				
				uint _x = _frameWidth;
				uint _y = _frameHeight;
				
				_vertexArray.Append(new Vertex(new Vector2f(_p.X, _p.Y), 			color, new Vector2f(frameX, frameY)));
				_vertexArray.Append(new Vertex(new Vector2f(_p.X + _x, _p.Y), 		color, new Vector2f(_x + frameX, frameY)));
				_vertexArray.Append(new Vertex(new Vector2f(_p.X + _x, _p.Y + _y), 	color, new Vector2f(_x + frameX, _y + frameY)));
				_vertexArray.Append(new Vertex(new Vector2f(_p.X, _p.Y + _y), 		color, new Vector2f(frameX, _y + frameY)));
				
				// get next particle
				p = p._next;
			}
			
			FP.Screen.Draw(_vertexArray, states);
		}
		
		/// <summary>
		/// Creates a new Particle type for this Emitter.
		/// </summary>
		/// <param name="name">Name of the particle type.</param>
		/// <param name="frames">Array of frame indices for the particles to animate.</param>
		/// <returns>A new ParticleType object.</returns>
		public ParticleType NewType(string name, int[] frames = null)
		{
			if (frames == null) frames = FP.Frames(0);
			if (_types.ContainsKey(name)) throw new Exception("Cannot add multiple particle types of the same name");
			
			var type = new ParticleType(name, frames, _source, (int) _frameWidth, (int) _frameHeight);
			_types.Add(name, type);
			
			return type;
		}
		
		/// <summary>
		/// Defines the motion range for a particle type.
		/// </summary>
		/// <param name="name">The particle type.</param>
		/// <param name="angle">Launch Direction.</param>
		/// <param name="distance">Distance to travel.</param>
		/// <param name="duration">Particle duration.</param>
		/// <param name="angleRange">Random amount to add to the particle's direction.</param>
		/// <param name="distanceRange">Random amount to add to the particle's distance.</param>
		/// <param name="durationRange">Random amount to add to the particle's duration.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>The ParticleType object being modified.</returns>
		public ParticleType SetMotion(string name, float angle, float distance, float duration, float angleRange = 0, float distanceRange = 0, float durationRange = 0, Easer ease = null)
		{
			return _types[name].SetMotion(angle, distance, duration, angleRange, distanceRange, durationRange, ease);
		}
		
		/// <summary>
		/// Sets the gravity range for a particle type.
		/// </summary>
		/// <param name="name">The particle type.</param>
		/// <param name="gravity">Gravity amount to affect to the particle y velocity.</param>
		/// <param name="gravityRange">Random amount to add to the particle's gravity.</param>
		/// <returns>The ParticleType object being modified.</returns>
		public ParticleType SetGravity(string name, float gravity = 0, float gravityRange = 0)
		{
			return _types[name].SetGravity(gravity, gravityRange);
		}
		
		/// <summary>
		/// Sets the alpha range of the particle type.
		/// </summary>
		/// <param name="name">The particle type.</param>
		/// <param name="start">The starting alpha.</param>
		/// <param name="finish">The finish alpha.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>The ParticleType object being modified.</returns>
		public ParticleType SetAlpha(string name, float start = 1, float finish = 0, Easer ease = null)
		{
			return _types[name].SetAlpha(start, finish, ease);
		}
		
		/// <summary>
		/// Sets the color range of the particle type.
		/// </summary>
		/// <param name="name">The particle type.</param>
		/// <param name="start">The starting color.</param>
		/// <param name="finish">The finish color.</param>
		/// <param name="ease">Optional easer function.</param>
		/// <returns>The ParticleType object being modified.</returns>
		public ParticleType SetColor(string name, Color start, Color finish, Easer ease = null)
		{
			return _types[name].SetColor(start, finish, ease);
		}
		
		/// <summary>
		/// Emits a particle.
		/// </summary>
		/// <param name="name">Particle type to emit.</param>
		/// <param name="x">X point to emit from.</param>
		/// <param name="y">Y point to emit from.</param>
		/// <returns></returns>
		public Particle Emit(string name, float x, float y)
		{
			if (!_types.ContainsKey(name)) throw new Exception("Particle type \"" + name + "\" does not exist.");
			Particle p = null;
			ParticleType type = _types[name];
			
			if (_cache != null)
			{
				p = _cache;
				_cache = p._next;
			}
			else 
				
				p = new Particle();
			p._next = _particle;
			p._prev = null;
			if (p._next != null) p._next._prev = p;
			
			p._type = type;
			p._time = 0;
			p._duration = type._duration + type._durationRange * FP.Random;
			float a = type._angle + type._angleRange * FP.Random;
			float d = type._distance + type._distanceRange * FP.Random;
			p._moveX = (float) Math.Cos(a) * d;
			p._moveY = (float) Math.Sin(a) * d;
			p._x = x;
			p._y = y;
			p._gravity = type._gravity + type._gravityRange * FP.Random;
			_particleCount ++;
			return (_particle = p);
		}
		
		/// <summary>
		/// Amount of currently existing particles.
		/// </summary>
		public uint ParticleCount
		{
			get
			{
				return _particleCount;
			}
		}
		
		// Particle information.
		private Dictionary<string, ParticleType> _types;
		private Particle _particle;
		private Particle _cache;
		private uint _particleCount;
		
		// Source information.
		private Texture _source;
		private uint _width;
		private uint _height;
		private uint _frameWidth;
		private uint _frameHeight;
		private uint _frameCount;
		
		private VertexArray _vertexArray;
	}
}
