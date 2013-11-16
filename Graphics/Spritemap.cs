/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/17/2013
 * Time: 8:09 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;

namespace Punk.Graphics
{
	/// <summary>
	/// Animated Image. Can have multiple animations,
	/// which draw frames from the provided source image to the screen.
	/// </summary>
	public class Spritemap : Image
	{
		/// <summary>
		/// Callback type for when animations complete.
		/// </summary>
		public delegate void OnComplete();
		
		/// <summary>
		/// If the animation has stopped.
		/// </summary>
		public bool Complete = true;
		
		/// <summary>
		/// Optional callback function for animation end.
		/// </summary>
		public OnComplete Callback;
		
		/// <summary>
		/// Animation speed factor, alter this to speed up/slow down all animations.
		/// </summary>
		public float Rate = 1;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="texture">Source image.</param>
		/// <param name="frameWidth">Frame width.</param>
		/// <param name="frameHeight">Frame height.</param>
		/// <param name="complete">Optional callback function for animation end.</param>
		public Spritemap(Texture texture, int frameWidth = 0, int frameHeight = 0, OnComplete complete = null) :
		base(texture, new IntRect(0, 0, frameWidth, frameHeight))
		{
			_anims = new Dictionary<string, Anim>();
			
			var rect = new IntRect(0, 0, frameWidth, frameHeight);
			if (frameWidth == 0)
			{
				rect.Width = (int) _source.Size.X;
			}
			
			if (frameHeight == 0)
			{
				rect.Height = (int) _source.Size.Y;
			}
			
			_width = (int) _source.Size.X;
			_height = (int) _source.Size.Y;
			
			Columns = (int) Math.Ceiling((float) _width / ClipRect.Width);
			Rows = (int) Math.Ceiling((float) _height / ClipRect.Height);
			
			FrameCount = Columns * Rows;
			Callback = complete;
			
			_sourceRectSize = _sourceRect = rect;
			
			Active = true;
		}
		
		/// <summary>
		/// Updates the spritemap's buffer.
		/// </summary>
		private void UpdateBuffer()
		{
			int left = _sourceRectSize.Left;
			int top = _sourceRectSize.Top;
			int height = _sourceRectSize.Height;
			int width = _sourceRectSize.Width;
			
			if (_flipX)
			{
				left = width + left + width * (_frame % Columns);
				width = -width;
			}
			else
			{
				left = width * (_frame % Columns);
			}
			
			if (_flipY)
			{
				top = height + top + height * (int) (_frame / Columns);
				height = -height;
			}
			else
			{
				top = height * (int) (_frame / Columns);
			}
			
			_sourceRect = new IntRect(left, top, width, height);
		}
		
		protected override void UpdateTextureRect()
		{
			//	It's empty! OH NOES
			//	This is on purpose.
		}
		
		/// <summary>
		/// Updates the animation.
		/// </summary>
		public override void Update()
		{
			base.Update();
			
			if (_anim != null && !Complete)
			{
				float timeAdd = _anim.FrameRate * Rate;
				if (! FP.TimeInFrames)
				{
					timeAdd *= FP.Elapsed;
				}
				
				Timer += timeAdd;
				
				if (Timer >= 1)
				{
					while (Timer >= 1)
					{
						Timer --;
						_index ++;
						
						if (_index == _anim.FrameCount)
						{
							if (_anim.Loop)
							{
								_index = 0;
								
								if (Callback != null)
								{
									Callback();
								}
							}
							else
							{
								_index = _anim.FrameCount - 1;
								Complete = true;
								if (Callback != null)
								{
									Callback();
								}
								
								break;
							}
						}
					}
					
					if (_anim != null)
					{
						_frame = (int) (_anim.Frames[_index]);
					}
					
					UpdateBuffer();
				}
			}
		}
		
		/// <summary>
		/// Add an Animation.
		/// </summary>
		/// <param name="name">Name of the animation.</param>
		/// <param name="frames">Array of frame indices to animate through.</param>
		/// <param name="frameRate">Animation speed.</param>
		/// <param name="loop">If the animation should loop.</param>
		/// <returns>A new Anim object for the animation.</returns>
		public Anim Add(string name, int[] frames, float frameRate = 0, bool loop = true)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				frames[i] %= FrameCount;
				if (frames[i] < 0)
				{
					frames[i] += FrameCount;
				}
			}
			
			var result = new Anim(name, frames, frameRate, loop);
			result.Parent = this;
			_anims.Add(name, result);
			
			return result;
		}
		
		/// <summary>
		/// Plays an animation.
		/// </summary>
		/// <param name="name">Name of the animation to play.</param>
		/// <param name="reset">If the animation should force-restart if it is already playing.</param>
		/// <param name="frame">Frame of the animation to start from, if restarted.</param>
		/// <returns>Anim object representing the played animation.</returns>
		public Anim Play(string name, bool reset = false, int frame = 0)
		{
			if (!reset && _anim != null && _anim.Name == name)
			{
				return _anim;
			}
			
			if (!_anims.ContainsKey(name))
			{
				_frame = _index = 0;
				Complete = true;
				UpdateBuffer();
				return null;
			}
			
			_anim = _anims[name];
			
			_index = 0;
			Timer = 0;
			_frame = (int) _anim.Frames[frame % _anim.FrameCount];
			Complete = false;
			UpdateBuffer();
			return _anim;
		}
		
		/// <summary>
		/// Gets the frame index based on the column and row of the source image.
		/// </summary>
		/// <param name="column">Frame column.</param>
		/// <param name="row">Frame row.</param>
		/// <returns>Frame index.</returns>
		public int GetFrame(int column, int row)
		{
			return (row % Rows) * Columns + (column % Columns);
		}
		
		/// <summary>
		/// Sets the current display frame based on the column and row of the source image.
		/// When you set the frame, any animations playing will be stopped to force the frame.
		/// </summary>
		/// <param name="column">Frame column.</param>
		/// <param name="row">Frame row.</param>
		public void SetFrame(int column, int row)
		{
			_anim = null;
			int frame = (row % Rows) * Columns + (column % Columns);
			
			if (_frame == frame)
			{
				return;
			}
			
			_frame = frame;
			Timer = 0;
			UpdateBuffer();
		}
		
		/// <summary>
		/// Assigns the Spritemap to a random frame.
		/// </summary>
		public void RandFrame()
		{
			_frame = (int) FP.Rand((uint) FrameCount);
		}
		
		/// <summary>
		/// Sets the frame to the frame index of an animation.
		/// </summary>
		/// <param name="name">Animation to draw the frame frame.</param>
		/// <param name="index">Index of the frame of the animation to set to.</param>
		public void SetAnimFrame(string name, int index)
		{
			var frames = _anims[name].Frames;
			index %= frames.Length;
			
			if (index < 0)
			{
				index += frames.Length;
			}
			
			_frame = frames[index];
		}
		
		/// <summary>
		/// The current frame index. When you set this, any
		/// animations playing will be stopped to force the frame.
		/// </summary>
		public int Frame
		{
			get
			{
				return _frame;
			}
			
			set
			{
				_anim = null;
				value %= FrameCount;
				if (value < 0) value = FrameCount + value;
				if (_frame == value) return;
				_frame = value;
				Timer = 0;
				UpdateBuffer();
			}
		}
		
		/// <summary>
		/// Current index of the playing animation.
		/// </summary>
		public int Index
		{
			get
			{
				return _index;
			}
			
			set
			{
				if (_anim == null)
				{
					return;
				}
				
				value %= _anim.FrameCount;
				if (_index == value) return;
				_index = value;
				_frame = _anim.Frames[_index];
				Timer = 0;
				UpdateBuffer();
			}
		}
		
		/// <summary>
		/// The amount of frames in the Spritemap.
		/// </summary>
		public int FrameCount { get; private set; }
		
		/// <summary>
		/// Columns in the Spritemap.
		/// </summary>
		public int Columns { get; private set; }
		
		/// <summary>
		/// Rows in the Spritemap.
		/// </summary>
		public int Rows { get; private set; }
		
		/// <summary>
		/// The currently playing animation.
		/// </summary>
		public string CurrentAnim
		{
			get
			{
				return _anim == null ? _anim.Name : "";
			}
		}
		
		// Spritemap information.
		
		/// <summary>
		/// The width of the Spritemap.
		/// </summary>
		protected int _width;
		
		/// <summary>
		/// The height of the Spritemap.
		/// </summary>
		protected int _height;
		
		private Dictionary<string, Anim> _anims;
		private Anim _anim;
		
		private int _index;
		
		/// <summary>
		/// The current animation frame.
		/// </summary>
		protected int _frame;
		
		private float Timer = 0;
	}
}
