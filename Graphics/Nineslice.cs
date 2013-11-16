/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/21/2013
 * Time: 1:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Graphics
{
	//	TODO: Texture flipping
	
	/// <summary>
	/// A box that automatically repeats portions of its source texture to create a seamlessly tiled image.
	/// </summary>
	public class Nineslice : Graphic
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
		/// How many columns to render
		/// </summary>
		public uint Columns
		{
			get { return _size.X; }
			set
			{
				_size.X = value;
				RebuildGeometry();
			}
		}
		
		/// <summary>
		/// How many rows to render
		/// </summary>
		public uint Rows
		{
			get { return _size.Y; }
			set
			{
				_size.Y = value;
				RebuildGeometry();
			}
		}
		
		/// <summary>
		/// The width of the graphic
		/// </summary>
		public float Width
		{
			get { return Columns * sliceWidth; }
		}
		
		/// <summary>
		/// The height of the graphic
		/// </summary>
		public float Height
		{
			get { return Rows * sliceHeight; }
		}
		
		/// <summary>
		/// The scaled width of the image.
		/// </summary>
		public float ScaledWidth
		{
			get { return Width * Scale * ScaleX; }
		}
		
		/// <summary>
		/// The scaled height of the image.
		/// </summary>
		public float ScaledHeight
		{
			get { return Height * Scale * ScaleY; }
		}
		
		/// <summary>
		/// The alpha of the image
		/// </summary>
		public float Alpha
		{
			get { return _alpha; }
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
			get { return _color; }
			set
			{
				_color = value;
				UpdateColorTransform();
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="source">The source texture. Should be divisible by three in both directions.</param>
		/// <param name="columns">How many columns wide the graphic is.</param>
		/// <param name="rows">How many rows high the graphic is.</param>
		public Nineslice(Texture source, uint columns, uint rows)
		{
			Debug.Assert(source != null, "Source texture cannot be null!");
			
			_source = source;
			_size = new Vector2u(columns, rows);
			
			_transform = new Transformable();
			_vertexArray = new VertexArray(PrimitiveType.Quads, columns * rows * 4);
			_renderStates = new RenderStates(source);
			_color = Color.White;
			
			UpdateSliceRects();
			UpdateColorTransform();
		}
		
		private void UpdateSliceRects()
		{
			sliceWidth = (int) _source.Size.X / 3;
			sliceHeight = (int) _source.Size.Y / 3;
			
			_textureRects = new IntRect[9];
			
			for (int i = 0; i < 9; i++)
			{
				_textureRects[i] = new IntRect((i % 3) * sliceWidth, (int) Math.Floor(i / 3.0f) * sliceHeight, sliceWidth, sliceHeight);
	        }
			
			RebuildGeometry();
		}
		
		/// <summary>
		/// Renders the graphic.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="camera"></param>
		public override void Render(float x, float y, Camera camera)
		{
			base.Render(x, y, camera);
			
			_transform.Scale = new Vector2f(ScaleX * Scale, ScaleY * Scale);
			_transform.Rotation = -Angle;	//	TODO:	is reversing angles ideal?
			_transform.Position = new Vector2f(X + x - (camera.X - FP.HalfWidth) * ScrollX, Y + y - (camera.Y - FP.HalfHeight) * ScrollY);
			_transform.Origin = new Vector2f(OriginX, OriginY);
			
			_renderStates.Transform = _transform.Transform;
			FP.Screen.Draw(_vertexArray, _renderStates);
		}
		
		private void RebuildGeometry()
		{
			_vertexArray.Clear();
			
			//	arrays are allocated on the heap, so only construct it once.
			var texCoords = new Vector2f[4];
			
			BuildCornerTopLeft(texCoords);
			BuildTop(texCoords);
			BuildCornerTopRight(texCoords);
			
			BuildLeft(texCoords);
			BuildCenter(texCoords);
			BuildRight(texCoords);
			
			BuildCornerBottomLeft(texCoords);
			BuildBottom(texCoords);
			BuildCornerBottomRight(texCoords);
		}
		
		private void UpdateColorTransform()
		{
			//	FIXME:	Doesn't completely correspond to FP due to the differences between color transform behaviors.
			
			_color.A = (byte) (255 * Alpha);
			RebuildGeometry();
		}
		
		/**
		 * All vertices are wound clockwise beginning at the top-left corner.
		 * Slices correspond to array indices thusly:
		 * 0, 1, 2
		 * 3, 4, 5
		 * 6, 7, 8
		 **/
		#region Slice builders
		private void BuildCornerTopLeft(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 0);
			
			_vertexArray.Append(new Vertex(new Vector2f(0, 			0), 			_color, texCoords[0]));
			_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 0), 			_color, texCoords[1]));
			_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, sliceHeight), 	_color, texCoords[2]));
			_vertexArray.Append(new Vertex(new Vector2f(0, 			sliceHeight), 	_color, texCoords[3]));
		}
		
		private void BuildTop(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 1);
			
			if (StretchTop)
			{
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 				0),				_color, texCoords[0]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, 0), 			_color, texCoords[1]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, sliceHeight), 	_color, texCoords[2]));
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth,					sliceHeight), 	_color, texCoords[3]));
			}
			else
			{
				for (int i = 1; i < Columns - 1; i++)
				{
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				0), 			_color, texCoords[0]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	0), 			_color, texCoords[1]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	sliceHeight), 	_color, texCoords[2]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				sliceHeight), 	_color, texCoords[3]));
				}
			}
		}
		
		private void BuildCornerTopRight(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 2);
			
			_vertexArray.Append(new Vertex(new Vector2f((Columns- 1) * sliceWidth, 			0), 			_color, texCoords[0]));
			_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, 				0), 			_color, texCoords[1]));
			_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, 				sliceHeight), 	_color, texCoords[2]));
			_vertexArray.Append(new Vertex(new Vector2f((Columns- 1) * sliceWidth, 			sliceHeight), 	_color, texCoords[3]));
		}
		
		private void BuildLeft(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 3);
			
			if (StretchLeft)
			{
				_vertexArray.Append(new Vertex(new Vector2f(0, 			sliceHeight), 				_color, texCoords[0]));
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, sliceHeight), 				_color, texCoords[1]));
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, (Rows - 1) * sliceHeight), 	_color, texCoords[2]));
				_vertexArray.Append(new Vertex(new Vector2f(0, 			(Rows - 1) * sliceHeight), 	_color, texCoords[3]));
			}
			else
			{
				for (int i = 1; i < Rows - 1; i++)
				{
					_vertexArray.Append(new Vertex(new Vector2f(0, 				sliceHeight * i), 				_color, texCoords[0]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 	sliceHeight * i), 				_color, texCoords[1]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 	sliceHeight * i + sliceHeight), _color, texCoords[2]));
					_vertexArray.Append(new Vertex(new Vector2f(0, 				sliceHeight * i + sliceHeight), _color, texCoords[3]));
				}
			}
		}
		
		private void BuildCenter(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 4);	//	index for center
			
			if (StretchCenter)
			{
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 				sliceHeight),				_color, texCoords[0]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns- 1) * sliceWidth, 	sliceHeight), 				_color, texCoords[1]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns- 1) * sliceWidth, 	(Rows - 1) * sliceHeight), 	_color, texCoords[2]));
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 		 		(Rows - 1) * sliceHeight), 	_color, texCoords[3]));
			}
			else
			{
				for (int i = 1; i < Columns - 1; i++)
				{
					for (int j = 1; j < Rows - 1; j++)
					{
						_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				sliceHeight * j), 				_color, texCoords[0]));
						_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	sliceHeight * j), 				_color, texCoords[1]));
						_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	sliceHeight * j + sliceHeight), _color, texCoords[2]));
						_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				sliceHeight * j + sliceHeight), _color, texCoords[3]));
					}
				}
			}
		}
		
		private void BuildRight(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 5);
			
			if (StretchRight)
			{
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, sliceHeight), 				_color, texCoords[0]));
				_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, 		sliceHeight), 				_color, texCoords[1]));
				_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth,		(Rows - 1) * sliceHeight), 	_color, texCoords[2]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, (Rows - 1) * sliceHeight), 	_color, texCoords[3]));
			}
			else
			{
				for (int i = 1; i < Rows - 1; i++)
				{
					_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth,	sliceHeight * i), 				_color, texCoords[0]));
					_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, 		sliceHeight * i), 				_color, texCoords[1]));
					_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, 		sliceHeight * i + sliceHeight), _color, texCoords[2]));
					_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, sliceHeight * i + sliceHeight), _color, texCoords[3]));
				}
			}
		}
		
		private void BuildCornerBottomLeft(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 6);
			
			_vertexArray.Append(new Vertex(new Vector2f(0, 			(Rows - 1) * sliceHeight), 	_color, texCoords[0]));
			_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, (Rows - 1) * sliceHeight), 	_color, texCoords[1]));
			_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, Rows * sliceHeight), 		_color, texCoords[2]));
			_vertexArray.Append(new Vertex(new Vector2f(0, 			Rows * sliceHeight), 		_color, texCoords[3]));
		}
		
		private void BuildBottom(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 7);
			
			if (StretchBottom)
			{
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth, 				(Rows - 1) * sliceHeight),	_color, texCoords[0]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, (Rows - 1) * sliceHeight), 	_color, texCoords[1]));
				_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, Rows * sliceHeight), 		_color, texCoords[2]));
				_vertexArray.Append(new Vertex(new Vector2f(sliceWidth,					Rows * sliceHeight), 		_color, texCoords[3]));
			}
			else
			{
				for (int i = 1; i < Columns - 1; i++)
				{
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				(Rows - 1) * sliceHeight), 	_color, texCoords[0]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	(Rows - 1) * sliceHeight), 	_color, texCoords[1]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i + sliceWidth, 	Rows * sliceHeight), 		_color, texCoords[2]));
					_vertexArray.Append(new Vertex(new Vector2f(sliceWidth * i, 				Rows * sliceHeight), 		_color, texCoords[3]));
				}
			}
		}

		private void BuildCornerBottomRight(Vector2f[] texCoords)
		{
			GenerateTexCoords(texCoords, 8);
			
			_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, (Rows - 1) * sliceHeight), 	_color, texCoords[0]));
			_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, (Rows - 1) * sliceHeight), 		_color, texCoords[1]));
			_vertexArray.Append(new Vertex(new Vector2f(Columns * sliceWidth, Rows * sliceHeight), 				_color, texCoords[2]));
			_vertexArray.Append(new Vertex(new Vector2f((Columns - 1) * sliceWidth, Rows * sliceHeight), 		_color, texCoords[3]));
		}
		#endregion
		
		private void GenerateTexCoords(Vector2f[] texCoords, int index)
		{
			texCoords[0] = new Vector2f(_textureRects[index].Left, 							_textureRects[index].Top);								//	top left
			texCoords[1] = new Vector2f(_textureRects[index].Left + _textureRects[4].Width, 	_textureRects[index].Top);								//	top right
			texCoords[2] = new Vector2f(_textureRects[index].Left + _textureRects[4].Width,	_textureRects[index].Top + _textureRects[4].Height);	//	bottom right
			texCoords[3] = new Vector2f(_textureRects[index].Left,							_textureRects[index].Top + _textureRects[4].Height);	//	bottom left
		}
		/// <summary>
		/// Whether the texture should be stretched instead of repeating.
		/// </summary>
		public bool StretchCenter, StretchTop, StretchLeft, StretchBottom, StretchRight;
		
		private IntRect[] _textureRects;
		private Vector2u _size;
		private bool _flipX, _flipY;
		private Texture _source;
		
		private float _alpha = 1;
		private Color _color;
		private Transformable _transform;
		private VertexArray _vertexArray;
		private RenderStates _renderStates;
		private int sliceWidth, sliceHeight;
	}
}
