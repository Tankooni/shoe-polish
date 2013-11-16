/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/24/2013
 * Time: 1:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk.Utils
{
	/// <summary>
	/// Description of GridType.
	/// </summary>
	internal struct GridSize : IEquatable<GridSize>
	{
		public GridSize(int w, int h)
		{
			CellWidth = w;
			CellHeight = h;
		}
		
		public int CellWidth;
		public int CellHeight;
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is GridSize) && Equals((GridSize)obj);
		}
		
		public bool Equals(GridSize other)
		{
			return this.CellWidth == other.CellWidth && this.CellHeight == other.CellHeight;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * CellWidth.GetHashCode();
				hashCode += 1000000009 * CellHeight.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(GridSize lhs, GridSize rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(GridSize lhs, GridSize rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
