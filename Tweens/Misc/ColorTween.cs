/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/21/2013
 * Time: 7:18 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using SFML.Graphics;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Tweens a color's red, green, and blue properties independently.
	/// </summary>
	public class ColorTween : Tween
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public ColorTween(OnComplete complete, uint type = PERSIST) :
		base(0, type, complete, null)
		{
			Color = new Color();
		}
		
		/// <summary>
		/// Tweens the color to a new color.
		/// </summary>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="fromColor"></param>
		/// <param name="toColor">Start color.</param>
		/// <param name="ease">Optional easer function.</param>
		public void tween(float duration, Color fromColor, Color toColor, Easer ease = null)
		{
			Color = fromColor;
			uint from = FP.HexColor(fromColor);
			uint to = FP.HexColor(toColor);
			
			_startR = FP.GetRed(from);
			_startG = FP.GetGreen(from);
			_startB = FP.GetBlue(from);
			
			_rangeR = FP.GetRed(to) - _startR;
			_rangeG = FP.GetGreen(to) - _startG;
			_rangeB = FP.GetBlue(to) - _startB;
			
			_target = duration;
			_ease = ease;
			Start();
		}
		
		/// <summary>
		/// Updates the tween.
		/// </summary>
		public override void Update()
		{
			base.Update();
			
			var r = Convert.ToUInt32(FP.Clamp(_startR + _rangeR * _t, 0, 255));
			var g = Convert.ToUInt32(FP.Clamp(_startG + _rangeG * _t, 0, 255));
			var b = Convert.ToUInt32(FP.Clamp(_startB + _rangeB * _t, 0, 255));
			var color = r << 16 | g << 8 | b;
			
			Color = FP.Color(color);
		}
		
		// Color information.
		
		/// <summary>
		/// The tweened color.
		/// </summary>
		public Color Color;
		
		private float _startR;
		private float _startG;
		private float _startB;
		
		private float _rangeR;
		private float _rangeG;
		private float _rangeB;

		
	}
}