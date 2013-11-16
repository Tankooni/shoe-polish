/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/12/2013
 * Time: 7:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk.Graphics;
using Punk.Utils;
using SFML.Window;

namespace Punk.Debugging
{
	/// <summary>
	/// Description of Class.
	/// </summary>
	public class Console
	{
		public Keyboard.Key ToggleKey;
		
		/*
		 * This class is extremely basic comared to the original in Flashpunk.
		 * No entity movement, rubberband selection, or watch window -- hitbox and origin drawing only.
		 * This will hopefully change in the near future. :)
		 */
		
		public Console()
		{
			ToggleKey = Keyboard.Key.Tilde;
			
			LOG = new List<string>();
			WATCH_LIST = new List<string>();
			
			ENTITY_LIST = new List<Entity>();
			SCREEN_LIST = new List<Entity>();
			SELECT_LIST = new List<Entity>();
			
			_debug = true;
		}
		
		public void Log(params object[] data)
		{
			var s = "";
			
			// Iterate through data to build a string.
			for (int i = 0; i < data.Length; ++i)
			{
				if (i > 0) s += " ";
				s += (data[i] != null) ? data[i].ToString() : "null";
			}
			
			// Replace newlines with multiple log statements.
			if (s.IndexOf("\n") >= 0)
			{
				var a = s.Split('\n');
				foreach (var split in a) LOG.Add(split);
			}
			else
			{
				LOG.Add(s);
			}
			
			// If the log is running, update it.
			UpdateLog();
		}
		
		public void Watch(params string[] properties)
		{
			if (properties.Length > 1)
			{
				foreach (string i in properties) WATCH_LIST.Add(i);
			}
		}
		
		public void Error(params object[] data)
		{
			Log(data);
		}
		
		public void Enable()
		{
			// Quit if the console is already enabled.
			if (_enabled) return;
			
			// Enable it and add the Sprite to the stage.
			_enabled = true;
//			FP.engine.addChild(_sprite);
			
			// Used to determine some text sizing.
			var big = FP.Width >= 480;
			
			// The transparent FlashPunk logo overlay bitmap.
			background = Image.CreateRect(FP.Width, FP.Height, FP.Color(0x0));
			background.Alpha = BG_ALPHA;
			
			logo = new Image(Library.GetTexture("Punk.Embeds/console_logo.png"));
			logo.Alpha = LOGO_ALPHA;
			logo.CenterOO();
			logo.X = FP.HalfWidth;
			logo.Y = FP.HalfHeight;
			
//			_sprite.addChild(_back);
//			_back.bitmapData = new BitmapData(width, height, true, 0xFFFFFFFF);
//			var b:BitmapData = (new CONSOLE_LOGO).bitmapData;
//			FP.matrix.identity();
//			FP.matrix.tx = Math.max((_back.bitmapData.width - b.width) / 2, 0);
//			FP.matrix.ty = Math.max((_back.bitmapData.height - b.height) / 2, 0);
//			FP.matrix.scale(Math.min(width / _back.bitmapData.width, 1), Math.min(height / _back.bitmapData.height, 1));
//			_back.bitmapData.draw(b, FP.matrix, null, BlendMode.MULTIPLY);
//			_back.bitmapData.draw(_back.bitmapData, null, null, BlendMode.INVERT);
//			_back.bitmapData.colorTransform(_back.bitmapData.rect, new ColorTransform(1, 1, 1, 0.5));
//			
//			// The entity and selection sprites.
//			_sprite.addChild(_entScreen);
//			_entScreen.addChild(_entSelect);
//			
//			// The entity count text.
//			_sprite.addChild(_entRead);
//			_entRead.addChild(_entReadText);
//			_entReadText.defaultTextFormat = format(16, 0xFFFFFF, "right");
//			_entReadText.embedFonts = true;
//			_entReadText.width = 100;
//			_entReadText.height = 20;
//			_entRead.x = width - _entReadText.width;
//			
//			// The entity count panel.
//			_entRead.graphics.clear();
//			_entRead.graphics.beginFill(0, .5);
//			_entRead.graphics.drawRoundRectComplex(0, 0, _entReadText.width, 20, 0, 0, 20, 0);
//			
//			// The FPS text.
//			_sprite.addChild(_fpsRead);
//			_fpsRead.addChild(_fpsReadText);
//			_fpsReadText.defaultTextFormat = format(16);
//			_fpsReadText.embedFonts = true;
//			_fpsReadText.width = 70;
//			_fpsReadText.height = 20;
//			_fpsReadText.x = 2;
//			_fpsReadText.y = 1;
//			
//			// The FPS and frame timing panel.
//			_fpsRead.graphics.clear();
//			_fpsRead.graphics.beginFill(0, .75);
//			_fpsRead.graphics.drawRoundRectComplex(0, 0, big ? 320 : 160, 20, 0, 0, 0, 20);
//			
//			// The frame timing text.
//			if (big) _sprite.addChild(_fpsInfo);
//			_fpsInfo.addChild(_fpsInfoText0);
//			_fpsInfo.addChild(_fpsInfoText1);
//			_fpsInfoText0.defaultTextFormat = format(8, 0xAAAAAA);
//			_fpsInfoText1.defaultTextFormat = format(8, 0xAAAAAA);
//			_fpsInfoText0.embedFonts = true;
//			_fpsInfoText1.embedFonts = true;
//			_fpsInfoText0.width = _fpsInfoText1.width = 60;
//			_fpsInfoText0.height = _fpsInfoText1.height = 20;
//			_fpsInfo.x = 75;
//			_fpsInfoText1.x = 60;
//			
//			// The memory usage
//			_fpsRead.addChild(_memReadText);
//			_memReadText.defaultTextFormat = format(16);
//			_memReadText.embedFonts = true;
//			_memReadText.width = 110;
//			_memReadText.height = 20;
//			_memReadText.x = _fpsInfo.x + _fpsInfo.width + 5;
//			_memReadText.y = 1;
//			
//			// The output log text.
//			_sprite.addChild(_logRead);
//			_logRead.addChild(_logReadText0);
//			_logRead.addChild(_logReadText1);
//			_logReadText0.defaultTextFormat = format(16, 0xFFFFFF);
//			_logReadText1.defaultTextFormat = format(big ? 16 : 8, 0xFFFFFF);
//			_logReadText0.embedFonts = true;
//			_logReadText1.embedFonts = true;
//			_logReadText0.selectable = false;
//			_logReadText0.width = 80;
//			_logReadText0.height = 20;
//			_logReadText1.width = width;
//			_logReadText0.x = 2;
//			_logReadText0.y = 3;
//			_logReadText0.text = "OUTPUT:";
//			_logHeight = height - 60;
//			_logBar = new Rectangle(8, 24, 16, _logHeight - 8);
//			_logBarGlobal = _logBar.clone();
//			_logBarGlobal.y += 40;
//			if (big) _logLines = _logHeight / 16.5;
//			else _logLines = _logHeight / 8.5;
//			
//			// The debug text.
//			_sprite.addChild(_debRead);
//			_debRead.addChild(_debReadText0);
//			_debRead.addChild(_debReadText1);
//			_debReadText0.defaultTextFormat = format(16, 0xFFFFFF);
//			_debReadText1.defaultTextFormat = format(8, 0xFFFFFF);
//			_debReadText0.embedFonts = true;
//			_debReadText1.embedFonts = true;
//			_debReadText0.selectable = false;
//			_debReadText0.width = 80;
//			_debReadText0.height = 20;
//			_debReadText1.width = 160;
//			_debReadText1.height = int(height / 4);
//			_debReadText0.x = 2;
//			_debReadText0.y = 3;
//			_debReadText1.x = 2;
//			_debReadText1.y = 24;
//			_debReadText0.text = "DEBUG:";
//			_debRead.y = height - (_debReadText1.y + _debReadText1.height);
//			
//			// The button panel buttons.
//			_sprite.addChild(_butRead);
//			_butRead.addChild(_butDebug = new CONSOLE_DEBUG);
//			_butRead.addChild(_butOutput = new CONSOLE_OUTPUT);
//			_butRead.addChild(_butPlay = new CONSOLE_PLAY).x = 20;
//			_butRead.addChild(_butPause = new CONSOLE_PAUSE).x = 20;
//			_butRead.addChild(_butStep = new CONSOLE_STEP).x = 40;
//			updateButtons();
//			
//			// The button panel.
//			_butRead.graphics.clear();
//			_butRead.graphics.beginFill(0, .75);
//			_butRead.graphics.drawRoundRectComplex(-20, 0, 100, 20, 0, 0, 20, 20);
			
			// Default the display to debug view
			Debug = true;
			
			// Set the state to unpaused.
			Paused = false;
		}
		
		public void Update()
		{
			if (Input.Pressed(ToggleKey))
			{
				_visible = !_visible;
			}
			
			UpdateEntityLists(FP.World.Count != ENTITY_LIST.Count);
		}
		
		public void Render()
		{
			if (_visible)
			{
				var view = new SFML.Graphics.View(FP.Screen.GetView());
				
				FP.Screen.SetView(FP.Screen.DefaultView);
				background.Render(0, 0, FP.Camera);
				logo.Render(0, 0, FP.Camera);
				
				FP.Screen.SetView(view);
				RenderEntities();
			}
		}
		
				/**
		 * If the Console is currently in paused mode.
		 */
		public bool Paused
		{
			get { return _paused; }
			set
			{
				if (!_enabled) return;
			
				// Set the console to paused.
				_paused = value;
				FP.Engine.Paused = value;
				
				// Panel visibility.
//				_back.visible = value;
//				_entScreen.visible = value;
//				_butRead.visible = value;
				
				// If the console is paused.
				if (value)
				{
					// Set the console to paused mode.
					if (_debug) Debug = true;
					else UpdateLog();
				}
				else
				{
					// Set the console to running mode.
//					_debRead.visible = false;
//					_logRead.visible = true;
//					UpdateLog();
//					ENTITY_LIST.length = 0;
//					SCREEN_LIST.length = 0;
//					SELECT_LIST.length = 0;
				}
			}
		}
		
		public bool Debug
		{
			get { return _debug; }
			set
			{
				// Quit if the console isn't enabled.
				if (!_enabled) return;
				
//				// Set the console to debug mode.
//				_debug = value;
//				_debRead.visible = value;
//				_logRead.visible = !value;
//				
//				// Update console state.
//				if (value) updateEntityLists();
//				else updateLog();
//				renderEntities();
			}
		}
		
		private void UpdateEntityLists(bool fetchList = true)
		{
			// If the list should be re-populated.
			if (fetchList)
			{
				ENTITY_LIST = new List<Entity>();
				FP.World.GetAll(ENTITY_LIST);
			}
			
			// Update the list of Entities on screen.
			SCREEN_LIST = new List<Entity>();
			foreach (var e in ENTITY_LIST)
			{
//				if (e.CollideRect(e.X, e.Y, FP.Camera.X - FP.HalfWidth, FP.Camera.Y - FP.HalfHeight, FP.Width, FP.Height))
					SCREEN_LIST.Add(e);
			}
		}
		
		private void RenderEntities()
		{
			if (_debug)
			{
				var verts = new SFML.Graphics.VertexArray(SFML.Graphics.PrimitiveType.Lines);
				
//				var g:Graphics = _entScreen.graphics,
//					sx:Number = FP.screen.scaleX * FP.screen.scale,
//					sy:Number = FP.screen.scaleY * FP.screen.scale;
//				g.clear();
				
				var scale = FP.Camera.Zoom;
				foreach (var e in SCREEN_LIST)
				{
					// If the Entity is not selected.
//					if (SELECT_LIST.indexOf(e) < 0)
//					{
//						 Draw the normal hitbox and position.
					
						var color = FP.Color(0xff0000);
						var x = e.X - e.OriginX + (FP.Camera.X - FP.HalfWidth);
						var y = e.Y - e.OriginY + (FP.Camera.Y - FP.HalfHeight);
						var width = e.Width;
						var height = e.Height;
					
						if (e.Width != 0 && e.Height != 0)
						{
							if (e.Mask != null) e.Mask.RenderDebug(verts);
							
							Draw.Rect(x, y, width, height, color);
						}
						
						color = FP.Color(0x00ff00);
						x = e.X + (FP.Camera.X - FP.HalfWidth) - 3;
						y = e.Y + (FP.Camera.Y - FP.HalfHeight) - 3;
						width = 6;
						height = 6;
						
						Draw.Rect(x, y, width, height, color);
//					}
//					else
//					{
//						// Draw the selected hitbox and position.
//						if (e.width && e.height)
//						{
//							g.lineStyle(1, 0xFFFFFF);
//							g.drawRect((e.x - e.originX - FP.camera.x) * sx, (e.y - e.originY - FP.camera.y) * sy, e.width * sx, e.height * sy);
//							if (e.mask) e.mask.renderDebug(g);
//						}
//						g.lineStyle(1, 0xFFFFFF);
//						g.drawRect((e.x - FP.camera.x) * sx - 3, (e.y - FP.camera.y) * sy - 3, 6, 6);
//					}
				
				}
				
				FP.Screen.Draw(verts);
			}
		}
		
		
		// Console state information.
		/** @private */ private bool _visible;
		/** @private */ private bool _enabled;
		/** @private */ private bool _paused;
		/** @private */ private bool _debug;
		/** @private */ private bool _scrolling;
		/** @private */ private bool _selecting;
		/** @private */ private bool _dragging;
		/** @private */ private bool _panning;
		
//		// Console display objects.
//		/** @private */ private var _format:TextFormat = new TextFormat("default");
//		/** @private */ private var _back:Bitmap = new Bitmap;
		
		private Image background;
		private Image logo;
		private const float BG_ALPHA = 0.5f;
		private const float LOGO_ALPHA = 0.15f;
//		
//		// FPS panel information.
//		/** @private */ private var _fpsRead:Sprite = new Sprite;
//		/** @private */ private var _fpsReadText:TextField = new TextField;
//		/** @private */ private var _fpsInfo:Sprite = new Sprite;
//		/** @private */ private var _fpsInfoText0:TextField = new TextField;
//		/** @private */ private var _fpsInfoText1:TextField = new TextField;
//		/** @private */ private var _memReadText:TextField = new TextField;
//		
//		// Output panel information.
//		/** @private */ private var _logRead:Sprite = new Sprite;
//		/** @private */ private var _logReadText0:TextField = new TextField;
//		/** @private */ private var _logReadText1:TextField = new TextField;
//		/** @private */ private var _logHeight:uint;
//		/** @private */ private var _logBar:Rectangle;
//		/** @private */ private var _logBarGlobal:Rectangle;
//		/** @private */ private var _logScroll:Number = 0;
//		
//		// Entity count panel information.
//		/** @private */ private var _entRead:Sprite = new Sprite;
//		/** @private */ private var _entReadText:TextField = new TextField;
//		
//		// Debug panel information.
//		/** @private */ private var _debRead:Sprite = new Sprite;
//		/** @private */ private var _debReadText0:TextField = new TextField;
//		/** @private */ private var _debReadText1:TextField = new TextField;
//
//		// Button panel information
//		/** @private */ private var _butRead:Sprite = new Sprite;
//		/** @private */ private var _butDebug:Bitmap;
//		/** @private */ private var _butOutput:Bitmap;
//		/** @private */ private var _butPlay:Bitmap;
//		/** @private */ private var _butPause:Bitmap;
//		/** @private */ private var _butStep:Bitmap;
//		
//		// Entity selection information.
//		/** @private */ private var _entScreen:Sprite = new Sprite;
//		/** @private */ private var _entSelect:Sprite = new Sprite;
//		/** @private */ private var _entRect:Rectangle = new Rectangle;
//		
//		// Log information.
		private int _logLines = 33;
		private List<string> LOG;
//		
//		// Entity lists.
		private List<Entity> ENTITY_LIST;
		private List<Entity> SCREEN_LIST;
		private List<Entity> SELECT_LIST;
//		
//		// Watch information.
		private List<string> WATCH_LIST;
		
//		// Reference the Text class so we can access its embedded font
//		private static var textRef:Text;
		
		void UpdateLog()
		{
			if (_enabled && _visible)
			{
				//	on-screen console
			}
			
			while (LOG.Count > 0)
			{
				System.Console.WriteLine(LOG[LOG.Count - 1]);
				LOG.RemoveAt(LOG.Count - 1);
			}
		}
	}
}
