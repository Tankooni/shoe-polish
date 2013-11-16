/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/2/2013
 * Time: 8:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Resources;
using SFML;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using Punk;
using Punk.Utils;

namespace Punk
{
	/// <summary>
	/// Main game class. Manages the game loop.
	/// </summary>
	public class Engine
	{
		private delegate void MainLoop();
		
		/// <summary>
		/// If the game should stop updating/rendering.
		/// </summary>
		public bool Paused;
		
		/// <summary>
		/// Cap on the elapsed time (default at 30 FPS). Raise this to allow for lower framerates (eg. 1 / 10).
		/// </summary>
		public static float MaxElapsed = 0.0333f;
		
		/// <summary>
		/// The max amount of frames that can be skipped in fixed framerate mode.
		/// </summary>
		public uint MaxFrameSkip = 5;
		
		/// <summary>
		/// The amount of milliseconds between ticks in fixed framerate mode.
		/// </summary>
		public static float SkipRate = 4;
		
		/// <summary>
		/// The screen refresh color. Defaults to black.
		/// </summary>
		public Color ClearColor = Color.Black;
		
		/// <summary>
		/// Engine constructor
		/// </summary>
		/// <param name="width">The width of your game.</param>
		/// <param name="height">The height of your game.</param>
		/// <param name="framerate">The game framerate, in frames per second (default 60).</param>
		public Engine(uint width, uint height, float framerate = 60)
		{
			//	Engine properties
			FP.Width = width;
			FP.Height = height;
			FP.Framerate = framerate;
			FP.Fixed = true;
			FP.TimeInFrames = false;
			
			//	Globals
			FP.Tweener = new Tweener();
			FP.Engine = this;
			FP.Screen = new RenderWindow(new VideoMode(FP.Width, FP.Height), "");
			FP._world = new World();
			FP.Camera = new Camera();
			FP.Console = new Punk.Debugging.Console();
			CheckWorld();
			
			FP.Screen.Clear();
			
			//	Random
			FP.RandomizeSeed();
			
			//	Timing
			FP.Timer= new Stopwatch();
			FP.Timer.Restart();
			FP._time = (uint) FP.Timer.ElapsedMilliseconds;
			_frameList = new List<uint>();
			FP.Timescale = 1;
			
			//	Window events
			FP.Screen.Closed += new EventHandler(OnClosed);
			FP.Screen.GainedFocus += new EventHandler(OnFocusGained);
			FP.Screen.LostFocus += new EventHandler(OnFocusLost);
			
			Library.LoadEmbeddedAssets();
			
			Input.Init();
			
			//	FIXME:	Currently only FixedFramerateLoop is hooked up.
			//	FIXME:	Framerate independent loops may not be possible without timer callbacks like Flash has.
			
			_rate = 1000.0f / FP.Framerate;
			
			Init();
			
			if (FP.Fixed)
			{
				_skip = _rate * (MaxFrameSkip + 1);
				_last = _prev = FP.GetTimer();
				
				while (FP.Screen.IsOpen())
				{
					FP.Screen.DispatchEvents();					
					FP.Screen.SetTitle(Math.Round(1000.0f / (FP._gameTime)).ToString());

					
					FixedFramerateLoop();
				}
			}
			else
			{
				_last = FP.GetTimer();
				
				while (FP.Screen.IsOpen())
				{
					FP.Screen.DispatchEvents();
					
					FramerateIndependantLoop();
				}
			}
		}
		
		#region Loops
		private void FixedFramerateLoop()
		{
			// update timer
			_time = FP.GetTimer();
			_delta += (_time - _last);
			_last = _time;
			
			// quit if a frame hasn't passed
			if (_delta < _rate)
			{
				return;
			}
			
			// update timer
			_gameTime = _time;
			FP._platformTime = _time - _platformTime;
			
			// update console
			if (FP.Console != null) FP.Console.Update();
			
			// update loop
			if (_delta > _skip) _delta = _skip;
			while (_delta >= _rate)
			{
				FP.Elapsed = _rate * FP.Timescale * 0.001f;
				
				// update timer
				_updateTime = _time;
				_delta -= _rate;
				_prev = _time;
				
				// update loop
				if (!Paused) Update();
				
				// update input
				Input.Update();
				
				// update timer
				_time = FP.GetTimer();
				FP._updateTime = _time - _updateTime;
			}
			
			// update timer
			_renderTime = _time;
			
			// render loop
			if (!Paused) Render();
			
			FP.Console.Render();
			
			// update timer
			_time = _platformTime = FP.GetTimer();
			FP._renderTime = _time - _renderTime;
			FP._gameTime =  _time - _gameTime;
		}
		
		private void FramerateIndependantLoop()
		{
			// update timer
			_time = _gameTime = FP.GetTimer();
			FP._platformTime = (uint) _time - _platformTime;
			_updateTime = _time;
			FP.Elapsed = (_time - _last) / 1000.0f;
			if (FP.Elapsed > MaxElapsed) FP.Elapsed = MaxElapsed;
			FP.Elapsed *= FP.Timescale;
			_last = _time;
			
			// update console
			if (FP.Console != null) FP.Console.Update();
			
			// update loop
			if (!Paused) Update();
			
			// update input
			Input.Update();
			
			// update timer
			_time = _renderTime = FP.GetTimer();
			FP._updateTime = _time - _updateTime;
			
			// render loop
			if (!Paused) Render();
			
			// update timer
			_time = _platformTime = FP.GetTimer();
			FP._renderTime = _time - _renderTime;
			FP._gameTime = _time - _gameTime;
		}
		#endregion
		
		/// <summary>
		/// Override this; called when the Engine starts up.
		/// </summary>
		public virtual void Init()
		{
		}
		
		/// <summary>
		/// Updates the engine.
		/// </summary>
		private void Update()
		{
            Sfx.UpdateSounds();
            Music.UpdateJams();
            
            Draw.Update();
            
			if (FP.Tweener.Active && FP.Tweener._tween != null)
			{
				FP.Tweener.UpdateTweens();
			}
			
			if (FP._world.Active)
			{
				if (FP._world._tween != null)
				{
					FP._world.UpdateTweens();
				}
				
				FP._world.Update();
			}
			
			FP._world.UpdateLists();
			
			if (FP._goto != null)
			{
				CheckWorld();
			}
		}
		
		private void Render()
		{
			var t = FP.GetTimer();
			if (_frameLast == 0) _frameLast = t;
			
			// render loop
			FP.Screen.Clear(ClearColor);
			if (FP._world.Visible)
			{
				var view = new View(FP.Screen.DefaultView);
				
				view.Rotation = -FP.Camera.Angle;
				view.Zoom(FP.Camera.Zoom);
				//	don't need to set the position
				
				FP.Screen.SetView(view);
				
				FP._world.Render();
			}
			
			FP.Console.Render();
			
			Draw.Render();
			
			FP.Screen.Display();
			
			// more timing stuff
			t = FP.GetTimer();
			_frameList.Add(t - _frameLast);
			_frameListSum += t - _frameLast;
			
			if (_frameList.Count > 10)
			{
				_frameListSum -= _frameList[0];
				_frameList.RemoveAt(0);
			}
			
			FP.Framerate = 1000 / ((float)_frameListSum / (float)_frameList.Count);
			_frameLast = t;
		}
		
		private void CheckWorld()
		{
			if (FP._goto == null)
			{
				return;
			}
			
			FP._world.End();
			FP._world.UpdateLists();
			if (FP._world != null && FP._world.AutoClear && FP._world._tween != null)
			{
				FP._world.ClearTweens();
			}
			
			FP._world = FP._goto;
			FP._goto = null;
			FP.Camera = FP._world.Camera;
			
			FP._world.UpdateLists();
			FP._world.Begin();
			FP._world.UpdateLists();
		}
		
		#region Timing
		private float _delta = 0;
		private uint _time;
		private float _last;
		//	TODO:	private Timer _timer;
		private float _rate;
		private float _skip;
		private float _prev;
		#endregion
		
		#region Debug timing
		private uint _updateTime;
		private uint _renderTime;
		private uint _gameTime;
		private uint _platformTime;
		#endregion
		
		#region Framerate tracking
		private uint _frameLast = 0;
		private uint _frameListSum = 0;
		private List<uint> _frameList;
		#endregion
		
		#region Event handlers
		private static void OnClosed(object sender, EventArgs e)
		{
			Window window = sender as Window;
			window.Close();
		}
		
		private static void OnFocusGained(object sender, EventArgs e)
		{
			FP.Focused = true;
		}
		
		private static void OnFocusLost(object sender, EventArgs e)
		{
			FP.Focused = false;
		}
		#endregion
	}
}
