/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/22/2013
 * Time: 5:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Punk.Masks
{
	/// <summary>
	/// Description of Grid.
	/// </summary>
	public class Grid : Hitbox
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="width">The width of the grid.</param>
		/// <param name="height">The width of the grid.</param>
		/// <param name="tileWidth">The width of each tile in the grid.</param>
		/// <param name="tileHeight">The height of each tile in the grid.</param>
		/// <param name="x">X offset.</param>
		/// <param name="y">Y offset.</param>
		public Grid(int width, int height, int tileWidth, int tileHeight, int x = 0, int y = 0) 
		{
			// check for illegal grid size
			if (width == 0 || height == 0 || tileWidth == 0 || tileHeight == 0)
			{
				throw new Exception("Illegal Grid, sizes cannot be 0.");
			}
			
			// set grid properties
			Columns = (uint) (width / tileWidth);
			Rows = (uint) (height / tileHeight);
			
			_data = new bool[Columns, Rows];
			
			_tile = new IntRect(0, 0, (int) tileWidth, (int) tileHeight);
			
			_x = x;
			_y = y;
			_width = width;
			_height = height;
			
			// set callback voids
			_check[typeof(Mask)] = CollideMask;
			_check[typeof(Hitbox)] = CollideHitbox;
			//	TODO:	_check[typeof(Pixelmask)] = CollidePixelmask;
			_check[typeof(Grid)] = CollideGrid;
		}
		
		/// <summary>
		/// Sets the value of the tile.
		/// </summary>
		/// <param name="column">Tile column.</param>
		/// <param name="row">Tile row.</param>
		/// <param name="solid">If the tile should be solid.</param>
		public void SetTile(int column = 0, int row = 0, bool solid = true)
		{
			if (UsePositions)
			{
				column /= _tile.Width;
				row /= _tile.Height;
			}
			
			_data[column, row] = solid;
		}
		
		/// <summary>
		/// Makes the tile non-solid.
		/// </summary>
		/// <param name="column">Tile column.</param>
		/// <param name="row">Tile row.</param>
		public void ClearTile(int column = 0, int row = 0)
		{
			SetTile(column, row, false);
		}
		
		/// <summary>
		/// Gets the value of a tile.
		/// </summary>
		/// <param name="column">Tile column.</param>
		/// <param name="row">Tile row.</param>
		/// <returns>The tile value.</returns>
		public bool GetTile(int column = 0, int row = 0)
		{
			if (UsePositions)
			{
				column /= _tile.Width;
				row /= _tile.Height;
			}
			return _data[column, row];
		}
		
		/// <summary>
		/// Sets the value of a rectangle region of tiles.
		/// </summary>
		/// <param name="column">First column.</param>
		/// <param name="row">First row.</param>
		/// <param name="width">Columns to fill.</param>
		/// <param name="height">Rows to fill.</param>
		/// <param name="solid">If the tiles should be solid.</param>
		public void SetRect(int column = 0, int row = 0, int width = 1, int height = 1, bool solid = true)
		{
			if (UsePositions)
			{
				column /= _tile.Width;
				row /= _tile.Height;
				width /= _tile.Width;
				height /= _tile.Height;
			}
			
			for (int i = column; i < width + column; i++)
			{
				for (int j = row; j < height + row; j++)
		     	{
					SetTile(i, j, solid);
				}
			}
		}
		
		/// <summary>
		/// Makes the rectangular region of tiles non-solid.
		/// </summary>
		/// <param name="column">First column.</param>
		/// <param name="row">First row.</param>
		/// <param name="Width">Columns to fill.</param>
		/// <param name="Height">Rows to fill.</param>
		public void ClearRect(int column = 0, int row = 0, int Width = 1, int Height = 1)
		{
			SetRect(column, row, Width, Height, false);
		}
		
		/// <summary>
		/// Loads the grid data from a string.
		/// </summary>
		/// <param name="str">The string data, which is a set of tile values (0 or 1) separated by the columnSep and rowSep strings.</param>
		/// <param name="columnSep">The string that separates each tile value on a row, default is ",".</param>
		/// <param name="rowSep">The string that separates each row of tiles, default is "\n".</param>
		public void LoadFromString(string str, string columnSep = ",", string rowSep = "\n")
		{
			string[] row = str.Split(new string[] { rowSep }, StringSplitOptions.None);
			int rows = row.Length;
			
			string[] col;
			int cols = 0;
			
			int x, y;
			
			for (y = 0; y < rows; y ++)
			{
				if (row[y] == "") continue;
				
				col = row[y].Split(new string[] { columnSep }, StringSplitOptions.None);
				cols = col.Length;
				
				for (x = 0; x < cols; x ++)
				{
					if (col[x] == "") continue;
					SetTile(x, y, int.Parse(col[x]) > 0);
				}
			}
		}
		
		/**
		* Saves the grid data to a string.
		* @param columnSep		The string that separates each tile value on a row, default is ",".
		* @param rowSep			The string that separates each row of tiles, default is "\n".
		*/
		public string SaveToString(string columnSep = ",", string rowSep = "\n")
		{
			string s = "";
			
			int x, y;
			
			for (y = 0; y < Rows; y++)
			{
				for (x = 0; x < Columns; x ++)
				{
					s += GetTile(x, y) ? '1' : '0';
					if (x != Columns - 1) s += columnSep;
				}
				if (y != Rows - 1) s += rowSep;
			}
			return s;
		}
		
		/// <summary>
		/// The tile Width.
		/// </summary>
		public int TileWidth
		{
			get
			{
				return _tile.Width;
			}
		}
		
		/// <summary>
		/// The tile Height.
		/// </summary>
		public int TileHeight
		{
			get
			{
				return _tile.Height;
			}
		}
		
		/// <summary>
		/// How many columns the grid has.
		/// </summary>
		public uint Columns { get; private set; }
		
		/// <summary>
		/// How many rows the grid has.
		/// </summary>
		public uint Rows { get; private set; }
		
		/// <summary>
		/// If x/y positions should be used instead of columns/rows.
		/// </summary>
		public bool UsePositions;
		
		/// <summary>
		/// Collides against an Entity.
		/// </summary>
		/// <param name="other">The other Entity.</param>
		/// <returns>Whether the two collide.</returns>
		public override bool Collide(Mask other)
		{
			var rect = new IntRect();
			var point = new Vector2i();
			
			rect.Left = (int) (other.Parent.X - other.Parent.OriginX - Parent.X + Parent.OriginX);
			rect.Top = (int) (other.Parent.Y - other.Parent.OriginY - Parent.Y + Parent.OriginY);
			
			point.X = (int) ((rect.Left + other.Parent.Width - 1) / _tile.Width) + 1;
			point.Y = (int) ((rect.Top + other.Parent.Height -1) / _tile.Height) + 1;
			
			rect.Left = (int) (rect.Left / _tile.Width);
			rect.Top = (int) (rect.Top / _tile.Height);
			
			rect.Width = (int) (point.X - rect.Left);
			rect.Height = (int) (point.Y - rect.Top);
			
			return HitTest(rect);
		}
		
		private bool HitTest(IntRect rect)
		{
			for (int i = rect.Left; i < rect.Width + rect.Left; i++)
			{
				for (int j = rect.Top; j < rect.Height + rect.Top; j++)
		     	{
					if (GetTile(i, j))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Collides against a Hitbox.
		/// </summary>
		/// <param name="other">The hitbox to collide with.</param>
		/// <returns>Whether the two collide.</returns>
		protected override bool CollideHitbox(Mask other)
		{
			var rect = new IntRect();
			var point = new Vector2i();
			
			var actual = other as Hitbox;
			Debug.Assert(actual != null);
			
			rect.Left = (int) (actual.Parent.X + actual._x - Parent.X - _x);
			rect.Top = (int) (actual.Parent.Y + actual._y - Parent.Y - _y);
			point.X = (int) ((rect.Left + actual._width - 1) / _tile.Width) + 1;
			point.Y = (int) ((rect.Top + actual._height -1) / _tile.Height) + 1;
			rect.Left = (int) (rect.Left / _tile.Width);
			rect.Top = (int) (rect.Top / _tile.Height);
			rect.Width = point.X - rect.Left;
			rect.Height = point.Y - rect.Top;
			
			return HitTest(rect);
		}
		
//		/** @private Collides against a Pixelmask. */
//		private bool CollidePixelmask(Mask other)
//		{
//			var actual = actual as Hitbox;
//			Debug.Assert(actual != null);
//			
//			
//			var x1:int = other.Parent.X + other._x - Parent.X - _x,
//				y1:int = other.Parent.Y + other._y - Parent.Y - _y,
//				x2:int = ((x1 + other._width - 1) / _tile.Width),
//				y2:int = ((y1 + other._height - 1) / _tile.Height);
//			_point.x = x1;
//			_point.y = y1;
//			x1 /= _tile.Width;
//			y1 /= _tile.Height;
//			_tile.x = x1 * _tile.Width;
//			_tile.y = y1 * _tile.Height;
//			var xx:int = x1;
//			while (y1 <= y2)
//			{
//				while (x1 <= x2)
//				{
//					if (_data.getPixel32(x1, y1))
//					{
//						if (other._data.hitTest(_point, 1, _tile)) return true;
//					}
//					x1 ++;
//					_tile.x += _tile.Width;
//				}
//				x1 = xx;
//				y1 ++;
//				_tile.x = x1 * _tile.Width;
//				_tile.y += _tile.Height;
//			}
//			return false;
//		}
		
		/// <summary>
		/// Collides against a Mask.
		/// </summary>
		/// <param name="other">The mask to collide with.</param>
		/// <returns>Whether the two collide.</returns>
		private bool CollideGrid(Mask other)
		{
			var actual = other as Grid;
			Debug.Assert(actual != null);
			
			// Find the X edges
			float ax1 = Parent.X + _x;
			float ax2 = ax1 + _width;
			float bx1 = actual.Parent.X + actual._x;
			float bx2 = bx1 + actual._width;
			if (ax2 < bx1 || ax1 > bx2) return false;
			
			// Find the Y edges
			float ay1 = Parent.Y + _y;
			float ay2 = ay1 + _height;
			float by1 = actual.Parent.Y + actual._y;
			float by2 = by1 + actual._height;
			if (ay2 < by1 || ay1 > by2) return false;
			
			// Find the overlapping area
			float ox1 = ax1 > bx1 ? ax1 : bx1;
			float oy1 = ay1 > by1 ? ay1 : by1;
			float ox2 = ax2 < bx2 ? ax2 : bx2;
			float oy2 = ay2 < by2 ? ay2 : by2;
			
			// Find the smallest tile size, and snap the top and left overlapping
			// edges to that tile size. This ensures that corner checking works
			// properly.
			
			float tw, th;
			if (_tile.Width < actual._tile.Width)
			{
				tw = _tile.Width;
				ox1 -= Parent.X + _x;
				ox1 = (int) (ox1 / tw) * tw;
				ox1 += Parent.X + _x;
			}
			else
			{
				tw = actual._tile.Width;
				ox1 -= actual.Parent.X + actual._x;
				ox1 = (int) (ox1 / tw) * tw;
				ox1 += actual.Parent.X + actual._x;
			}
			if (_tile.Height < actual._tile.Height)
			{
				th = _tile.Height;
				oy1 -= Parent.Y + _y;
				oy1 = (int) (oy1 / th) * th;
				oy1 += Parent.Y + _y;
			}
			else
			{
				th = actual._tile.Height;
				oy1 -= actual.Parent.Y + actual._y;
				oy1 = (int) (oy1 / th) * th;
				oy1 += actual.Parent.Y + actual._y;
			}
			
			// Step through the overlapping rectangle
			for (float y= oy1; y < oy2; y += th)
			{
				// Get the row indices for the top and bottom edges of the tile
				int ar1 = (int) (y - Parent.Y - _y) / _tile.Height;
				int br1 = (int) (y - actual.Parent.Y - actual._y) / actual._tile.Height;
				int ar2 = (int) ((y - Parent.Y - _y) + (th - 1)) / _tile.Height;
				int br2 = (int) ((y - actual.Parent.Y - actual._y) + (th - 1)) / actual._tile.Height;
				
				for (float x= ox1; x < ox2; x += tw)
				{
					// Get the column indices for the left and right edges of the tile
					int ac1 = (int) (x - Parent.X - _x) / _tile.Width;
					int bc1 = (int) (x - actual.Parent.X - actual._x) / actual._tile.Width;
					int ac2 = (int) ((x - Parent.X - _x) + (tw - 1)) / _tile.Width;
					int bc2 = (int) ((x - actual.Parent.X - actual._x) + (tw - 1)) / actual._tile.Width;
					
					// Check all the corners for collisions
					if ((GetTile(ac1, ar1) && actual.GetTile(bc1, br1))
					 || (GetTile(ac2, ar1) && actual.GetTile(bc2, br1))
					 || (GetTile(ac1, ar2) && actual.GetTile(bc1, br2))
					 || (GetTile(ac2, ar2) && actual.GetTile(bc2, br2)))
					{
						return true;
					}
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// Render debug information in the console.
		/// </summary>
		public override void RenderDebug(SFML.Graphics.VertexArray vertexArray)
		{
			//	TODO:	render the grid lol
		}
		
		private bool[,] _data;
		private IntRect _tile;

	}
}
