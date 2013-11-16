/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/7/2013
 * Time: 11:24 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Graphics
{
	/// <summary>
	/// Non-animated image. Can be drawn to the screen with transformations.
	/// </summary>
	public class Image : Graphic
	{
		/// <summary>
		/// Rotation of the image, in degrees.
		/// </summary>
		public float Angle = 0;
		
		/// <summary>
		/// Scale of the image, affects both x and y scale.
		/// </summary>
		public float Scale = 1;
		
		/// <summary>
		/// X scale of the image.
		/// </summary>
		public float ScaleX = 1;
		
		/// <summary>
		/// Y scale of the image.
		/// </summary>
		public float ScaleY = 1;
		
		/// <summary>
		/// X origin of the image, determines transformation point.
		/// </summary>
		public float OriginX = 0;
		
		/// <summary>
		/// Y origin of the image, determines transformation point.
		/// </summary>
		public float OriginY = 0;
		
		/// <summary>
		/// If you want to draw the image horizontally flipped.
		/// This will flip the texture without moving it, as would happen if you set ScaleX to -1.
		/// </summary>
		public bool FlippedX
		{
			get
			{
				return _flipX;
			}
			
			set
			{
				_flipX = value;
				UpdateTextureRect();
			}
		}
		
		/// <summary>
		/// If you want to draw the image vertically flipped.
		/// This will flip the texture without moving it, as would happen if you set ScaleY to -1.
		/// </summary>
		public bool FlippedY
		{
			get
			{
				return _flipY;
			}
			
			set
			{
				_flipY = value;
				UpdateTextureRect();
			}
		}
		
		/// <summary>
		/// Updates the texture rectangle.
		/// </summary>
		protected virtual void UpdateTextureRect()
		{
			int left = _sourceRectSize.Left;
			int top = _sourceRectSize.Top;
			int height = _sourceRectSize.Height;
			int width = _sourceRectSize.Width;
			
			if (_flipX)
			{
				left = width + left;
				width = -width;
			}
			
			if (_flipY)
			{
				top = height + top;
				height = -height;
			}
			
			_sourceRect = new IntRect(left, top, width, height);
		}
		
		/// <summary>
		/// The width of the image
		/// </summary>
		public virtual uint Width
		{
			get
			{
				return (uint) _sourceRect.Width;
			}
		}
		
		/// <summary>
		/// The height of the image
		/// </summary>
		public virtual uint Height
		{
			get
			{
				return (uint) _sourceRect.Height;
			}
		}
		
		/// <summary>
		/// The scaled width of the image.
		/// </summary>
		public uint ScaledWidth
		{
			get
			{
				return (uint) (_sourceRect.Width * Scale * ScaleX);
			}
		}
		
		/// <summary>
		/// The scaled height of the image.
		/// </summary>
		public uint ScaledHeight
		{
			get
			{
				return (uint) (_sourceRect.Height * Scale * ScaleY);
			}
		}
		
		/// <summary>
		/// Clipping rectangle for the image.
		/// </summary>
		public virtual IntRect ClipRect
		{
			get
			{
				return _sourceRect;
			}
			
			set
			{
				_sourceRect = value;
				_sourceRectSize = value;
				
				UpdateTextureRect();
			}
		}
		
		/// <summary>
		/// The texture that the image uses (null if in Shape mode).
		/// </summary>
		public Texture Source
		{
			get
			{
				return _source;
			}
		}
		
		//	TODO:	We don't need blend modes and I don't know if they're possible
		
		/// <summary>
		/// If the image should be drawn transformed with pixel smoothing.
		/// </summary>
		public bool Smooth;	//	FIXME:	This currently does nothing
		
		private void UpdateColorTransform()
		{
			//	FIXME:	Doesn't completely correspond to FP due to the differences between color transform behaviors.
			
			_color.A = (byte) (255 * Alpha);
			
			if (_spriteMode)
			{
				_sprite.Color = _color;
			}
			else
			{
				_shape.FillColor = _color;
			}
		}
		
		/// <summary>
		/// The alpha of the image
		/// </summary>
		public float Alpha
		{
			get
			{
				return _alpha;
			}
			
			set
			{
				//	Make sure the value is within [0, 1]
				value = value < 0 ? 0 : (value > 1 ? 1 : value);
				_alpha = value;
				UpdateColorTransform();
			}
		}
		
		/// <summary>
		/// Set the tint color of the image. Setting this to White (255, 255, 255 or 0xffffff) will disable tinting.
		/// If you set the alpha of this property, it will be overwritten by the Alpha property when the color transform is updated.
		/// </summary>
		public Color Color
		{
			get
			{
				var result = new Color(_color);
				result.A = 255;
				
				return result;
			}
			
			set
			{
				_color = value;
				_color.A = 255;
				UpdateColorTransform();
			}
		}
		
		/// <summary>
		/// Simple constructor for shape functions lol
		/// </summary>
		protected Image()
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="texture">Source image.</param>
		/// <param name="clipRect">Optional rectangle defining area of the source image to draw.</param>
		public Image(Texture texture, IntRect? clipRect = null)
		{
			Init(texture, clipRect);
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="texture">Source image.</param>
		/// <param name="clipRect">Optional rectangle defining area of the source image to draw.</param>
		protected void Init(Texture texture, IntRect? clipRect = null)
		{
			Debug.Assert(texture != null, "Texture cannot be null!");
			
			_source = texture;
			_sourceSize = texture.Size;
			
			if (clipRect.HasValue)
			{
				ClipRect = clipRect.Value;
			}
			else
			{
				ClipRect = new IntRect(0, 0, (int) _source.Size.X, (int) _source.Size.Y);
			}
			
			_sprite = new Sprite(_source, _sourceRect);
			_spriteMode = true;
			
			_color = Color.White;
			
			UpdateColorTransform();
		}
		
		/// <summary>
		/// Create a solid-color rectangle image.
		/// </summary>
		/// <param name="width">The width of the image.</param>
		/// <param name="height">The height of the image.</param>
		/// <param name="color">The color of the image.</param>
		/// <returns>The created image.</returns>
		public static Image CreateRect(uint width, uint height, Color color)
		{
			var result = new Image();
			result._shape = new RectangleShape(new Vector2f(width, height));
			result._color = color;
			
			result._spriteMode = false;
			result._sprite = null;
			
			result.ClipRect = new IntRect(0, 0, (int) width, (int) height);
			result._sourceSize = new Vector2u(width, height);
			
			result.UpdateColorTransform();
			
			return result;
		}
		
		/// <summary>
		/// Create a solid-color circle image.
		/// </summary>
		/// <param name="radius">The radius of the image.</param>
		/// <param name="color">The color of the image.</param>
		/// <returns>The created image.</returns>
		public static Image CreateCircle(uint radius, Color color)
		{
			var result = new Image();
			result._shape = new CircleShape((float) radius);
			result._color = color;
			
			result._spriteMode = false;
			result._sprite = null;
			
			result.ClipRect = new IntRect(0, 0, (int) radius * 2, (int) radius * 2);
			result._sourceSize = new Vector2u(radius * 2, radius * 2);
			
			result.UpdateColorTransform();
			
			return result;
		}
		
		/// <summary>
		/// Renders the graphic to the screen buffer.
		/// </summary>
		public override void Render(float x, float y, Camera camera)
		{
			if (_spriteMode)
			{
				_sprite.Scale = new Vector2f(ScaleX * Scale, ScaleY * Scale);
				_sprite.Rotation = -Angle;	//	TODO:	is reversing angles ideal?
				_sprite.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
				_sprite.Origin = new Vector2f(OriginX, OriginY);
				_sprite.TextureRect = _sourceRect;
				
				if (Shader != null)
				{
					FP.Screen.Draw(_sprite, new RenderStates(Shader));
				}
				else
				{
					FP.Screen.Draw(_sprite);
				}
			}
			else
			{
				_shape.Scale = new Vector2f(ScaleX * Scale, ScaleY * Scale);
				_shape.Rotation = -Angle;	//	TODO:	is reversing angles necessary?
				_shape.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
				_shape.Origin = new Vector2f(OriginX, OriginY);
				
				if (Shader != null)
				{
					FP.Screen.Draw(_shape, new RenderStates(Shader));
				}
				else
				{
					FP.Screen.Draw(_shape);
				}
			}
			
		}

		/// <summary>
		/// Centers the Image's originX/Y to its center.
		/// </summary>		
		public virtual void CenterOrigin()
		{
			OriginX = Width / 2;
			OriginY = Height / 2;
		}
		
		/// <summary>
		/// Centers the Image's originX/Y to its center.
		/// </summary>
		public void CenterOO()
		{
			CenterOrigin();
		}
		
		/// <summary>
		/// Whether the image is rendered with a sprite or a shape
		/// </summary>
		private bool _spriteMode;
		
		/// <summary>
		/// The texture to be rendered with the sprite
		/// </summary>
		protected Texture _source;
		
		/// <summary>
		/// If the image is in sprite mode, this is the sprite it will render
		/// </summary>
		protected Sprite _sprite;
		
		/// <summary>
		/// If the image is in shape mode, this is the shape it will render
		/// </summary>
		private Shape _shape;
		
		/// <summary>
		/// The clipping rectangle
		/// </summary>
		protected IntRect _sourceRect;
		
		/// <summary>
		/// The size of the clipping rectangle, used for accurate flipping.
		/// </summary>
		protected IntRect _sourceRectSize;
		
		/// <summary>
		/// The size of the image's texture or shape.
		/// </summary>
		protected Vector2u _sourceSize;
		
		/// <summary>
		/// The transparency of the image
		/// </summary>
		protected float _alpha = 1;
		
		/// <summary>
		/// The tint color of the image.
		/// </summary>
		protected Color _color;
		
		/// <summary>
		/// Whether the image is flipped on the X axis.
		/// </summary>
		protected bool _flipX = false;
		
		/// <summary>
		/// Whether the image is flipped on the Y axis.
		/// </summary>
		protected bool _flipY = false;	
	}
}
