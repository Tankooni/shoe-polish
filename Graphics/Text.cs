/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 5/22/2013
 * Time: 1:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Graphics
{
	/// <summary>
	/// Used for drawing text using embedded fonts.
	/// </summary>
	public class Text : Image
	{
		/// <summary>
		/// Text style types.
		/// </summary>
		public enum Styles
		{
			Bold = SFML.Graphics.Text.Styles.Bold,
			Italic = SFML.Graphics.Text.Styles.Italic,
			Regular = SFML.Graphics.Text.Styles.Regular,
			Underlined = SFML.Graphics.Text.Styles.Underlined
		};
		
		/// <summary>
		/// The font to assign to new Text objects.
		/// </summary>
		public static Font font;

		/// <summary>
		/// The font size to assign to new Text objects.
		/// </summary>
		public static uint size = 16;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="text">Text to display</param>
		/// <param name="x">X offset</param>
		/// <param name="y">Y offset</param>
		/// <param name="width">Image width (leave as 0 to size to the starting text string).</param>
		/// <param name="height">Image height (leave as 0 to size to the starting text string).</param>
		public Text(string text, float x = 0, float y = 0, uint width = 0, uint height = 0)
		{
			if (Text.font == null)
			{
				 Text.font = Library.GetFont("Punk.Embeds/DefaultFont.ttf");
			}
			
			_field.Font = Text.font;
			_field.CharacterSize = Text.size;
			_field.Color = FP.Color(0xFFFFFF);
			_field.DisplayedString = text;
			if (width == 0) width = (uint)_field.GetGlobalBounds().Width;
			if (height == 0) height = (uint)_field.GetGlobalBounds().Height;
			_source = new Texture(width, height);
			base.Init(_source);
			updateBuffer();
			this.X = x;
			this.Y = y;
			
			//TODO: Take this out once text field rects are in.
			// This _width and _height refers to the size of the text field rect.
			_width = width;
			_height = height;
		}

		public void updateBuffer(bool clearBefore = false) 
		{
			_field.Style = (SFML.Graphics.Text.Styles)(
				(bold ? Styles.Bold : Styles.Regular) |
				(italic ? Styles.Italic : Styles.Regular) |
				(underline ? Styles.Underlined : Styles.Regular));
			
//			_field.width = _width = _field.textWidth + 4;
//			_field.height = _height = _field.textHeight + 4;
//			_source.fillRect(_sourceRect, 0);
		}
		
		public override void Render(float x, float y, Camera camera)
		{
			_field.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
			_field.Scale = new Vector2f(ScaleX * Scale, ScaleY * Scale);
			_field.Rotation = -Angle;
			_field.Color = _color;
			FP.Screen.Draw(_field);
		}

		/// <summary>
		/// Centers the Text's originX/Y to its center.
		/// </summary>
		override public void CenterOrigin() 
		{
			OriginX = _width / 2;
			OriginY = _height / 2;
		}
		
		/// <summary>
		/// If the text has bolded.
		/// </summary>
		public bool Bolded
		{
			get { return bold; }
			set
			{
				bold = value;
				updateBuffer();
			}
		}
		
		/// <summary>
		/// If the text is italicized.
		/// </summary>
		public bool Italicized
		{
			get { return italic; }
			set
			{
				italic = value;
				updateBuffer();
			}
		}
		
		/// <summary>
		/// If the text is underlined.
		/// </summary>
		public bool Underlined
		{
			get { return underline; }
			set
			{
				underline = value;
				updateBuffer();
			}
		}
		
		/// <summary>
		/// Text string.
		/// </summary>
		public string String
		{
			get
			{
				return _text;
			}
			set
			{
				if (_text != value)
				{
					_field.DisplayedString = _text = value;
					updateBuffer();
				}
			}
		}

		/// <summary>
		/// Font family.
		/// </summary>
		public Font Font
		{
			get { return _font; }
			set
			{
				if (_font != value)
				{
					_field.Font = _font = value;
					updateBuffer();
				}
			}
		}

		/// <summary>
		/// Font size.
		/// </summary>
		public uint Size
		{
			get { return _size; }
			set
			{
				if (_size != value)
				{
					_field.CharacterSize = _size = value;
					updateBuffer();
				}
			}
		}

		/// <summary>
		/// Width of the text image.
		/// </summary>
		override public uint Width { get { return _width; } }

		/// <summary>
		/// Height of the text image.
		/// </summary>
		override public uint Height { get { return _height; } }

		private bool bold = false;
		private bool italic = false;
		private bool underline = false;
		
		// Text information.
		/** @private */ private SFML.Graphics.Text _field = new SFML.Graphics.Text();
		/** @private */ private uint _width;
		/** @private */ private uint _height;
		/** @private */ private string _text;
		/** @private */ private Font _font;
		/** @private */ private uint _size;
		
	}
}
