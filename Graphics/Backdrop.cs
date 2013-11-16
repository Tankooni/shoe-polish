/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/11/2013
 * Time: 5:52 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk.Utils;
using SFML.Graphics;
using System.Diagnostics;
using SFML.Window;

namespace Punk.Graphics
{
	/// <summary>
	/// Description of Backdrop.
	/// </summary>
	public class Backdrop : Graphic
	{
		public Backdrop(Texture source, bool repeatX = true, bool repeatY = true)
		{
			Debug.Assert(source != null, "Texture cannot be null!");
			_source = source;
			
			_repeatX = repeatX;
			_repeatY = repeatY;
			
			vertexArray = new VertexArray(PrimitiveType.Quads);
			renderStates = new RenderStates(_source);
			transform = new Transformable();
			
			texCoords = new Vector2f[]
			{
				new Vector2f(),
				new Vector2f(_source.Size.X, 0),
				new Vector2f(_source.Size.X, _source.Size.Y),
				new Vector2f(0, _source.Size.Y),
			};
			
			positions = new Vector2f[4];
			
		}
		
		public override void Render(float x, float y, Camera camera)
		{
			vertexArray.Clear();
			
			var sourceSize = _source.Size;
			var cam = camera.TopRight;
			
			var point = new Vector2f(X + x - cam.X * ScrollX, Y + y - cam.Y * ScrollY);
			
			int countX = (int) Math.Ceiling((float) FP.Width * camera.Zoom / (float) sourceSize.X);
			int countY = (int) Math.Ceiling((float) FP.Height * camera.Zoom / (float) sourceSize.Y);
			
			countX += (int) Math.Ceiling(countX / 2.0f);
			countY += (int) Math.Ceiling(countY / 2.0f);
			
			//	reset values if not repeating on that axis
			//	if we're being uptight about it, this is slower than
			//	putting the count computation in a conditional, but it's cleaner this way.
			if (_repeatX)
			{
				countX++;
				
				point.X %= sourceSize.X;
				if (point.X > cam.X) point.X -= sourceSize.X;
			}
			else
			{
				countX = 1;
			}
			
			if (_repeatY)
			{
				countY++;
				point.Y %= sourceSize.Y;
				if (point.Y > cam.Y) point.Y -= sourceSize.Y;
			}
			else
			{
				countY = 1;
			}
			
			for (int i = 0; i < countX; i++)
			{
				for (int j = 0; j < countY; j++)
				{
					positions[0] = new Vector2f(sourceSize.X * i, 					sourceSize.Y * j);					//	top left
					positions[1] = new Vector2f(sourceSize.X * i + sourceSize.X,	sourceSize.Y * j);					//	bottom left
					positions[2] = new Vector2f(sourceSize.X * i + sourceSize.X, 	sourceSize.Y * j + sourceSize.Y);	//	bottom right
					positions[3] = new Vector2f(sourceSize.X * i, 					sourceSize.Y * j + sourceSize.Y);	//	top right
					
					vertexArray.Append(new Vertex(positions[0], texCoords[0]));
					vertexArray.Append(new Vertex(positions[1], texCoords[1]));
					vertexArray.Append(new Vertex(positions[2], texCoords[2]));
					vertexArray.Append(new Vertex(positions[3], texCoords[3]));
				}
			}
			
			transform.Position = point;
			transform.Origin = new Vector2f(((float) FP.Width * camera.Zoom) / 2.0f, ((float) FP.Height * camera.Zoom) / 2.0f);
			renderStates.Transform = transform.Transform;
			FP.Screen.Draw(vertexArray, renderStates);
		}
		
		private Vector2f[] texCoords;
		private Vector2f[] positions;
		
		private Texture _source;
		private bool _repeatX;
		private bool _repeatY;
		private Transformable transform;
		
		private VertexArray vertexArray;
		private RenderStates renderStates;
	}
}
