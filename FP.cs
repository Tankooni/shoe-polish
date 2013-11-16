/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/2/2013
 * Time: 8:27 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk.Tweens.Misc;
using Punk.Utils.Reflect;
using SFML;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using Punk.Utils;

namespace Punk
{
	/// <summary>
	/// Static catch-all class used to access global properties and functions.
	/// </summary>
	public static class FP
	{
		/// <summary>
		/// Width of the game.
		/// </summary>
		public static uint Width;
		
		/// <summary>
		/// Height of the game.
		/// </summary>
		public static uint Height;
		
		/// <summary>
		/// Half width of the game.
		/// </summary>
		public static uint HalfWidth
		{
			get { return Width / 2;}
		}
		
		/// <summary>
		/// Half height of the game.
		/// </summary>
		public static uint HalfHeight
		{
			get { return Height / 2;}
		}
		
		/// <summary>
		/// If the game is running at a fixed framerate.
		/// </summary>
		public static bool Fixed;
		
		/// <summary>
		/// If times should be given in frames (as opposed to seconds).
		/// Default is true in fixed timestep mode and false in variable timestep mode.
		/// </summary>
		public static bool TimeInFrames;
		
		/// <summary>
		/// The assigned framerate
		/// </summary>
		public static float Framerate;
		
		/// <summary>
		/// Time elapsed since the last frame (in seconds).
		/// </summary>
		public static float Elapsed;
		
		/// <summary>
		/// Timescale assigned to Elapsed.
		/// </summary>
		public static float Timescale;
		
		/// <summary>
		/// The display window.
		/// </summary>
		public static RenderWindow Screen;
		
		//	TODO:	Flashpunk has a buffer that the screen is drawn to. We probably don't need it, and we can't get it anyway
			
		/// <summary>
		/// A rectangle representing the size of the screen.
		/// </summary>
		public static IntRect Bounds
		{
			get { return new IntRect(0, 0, (int) Width, (int) Height); }
		}
		
		/// <summary>
		/// The view camera. Can be rotated or scaled as well as positioned.
		/// </summary>
		public static Camera Camera;
		
		/// <summary>
		/// If the game currently has input focus or not.
		/// </summary>
		public static bool Focused;
		
		/// <summary>
		/// Resize the window.
		/// </summary>
		/// <param name="width">New width</param>
		/// <param name="height">New height</param>
		public static void Resize(uint width, uint height)
		{
			Width = width;
			Height = height;
			//	Don't need to set halfsizes or bounds
			
			Screen.Size = new Vector2u(width, height);
		}
		
		/// <summary>
		/// The currently active World.
		/// </summary>
		public static World World
		{
			get
			{
				return _world;
			}
			set
			{
				if (_world == value) return;
				_goto = value;
			}
		}
		
		//	TODO:	FP setCamera and resetCamera methods. Do we need them?
		
		#region SoundMixer
		
		/// <summary>
		/// Global sound volume. All sounds will be multiplied by this value.
		/// </summary>
		public static float Volume
		{
			get
			{
				return _volume;
			}
			set
			{
				if (value < 0)
				{
					_volume = 0;
				}
				
				if (value == _volume)
				{
					return;
				}
				
				//	TODO:	Set sound transform or whatever
				_volume = value;
			}
		}
		
		/// <summary>
		/// Global sound pan. All sounds will be offset by this value.
		/// </summary>
		public static float Pan
		{
			get
			{
				return _pan;
			}
			set
			{
				if (value < -1)
				{
					value = -1;
				}
				
				if (value > 1)
				{
					value = 1;
				}
				
				if (_pan == value)
				{
					return;
				}
				
				//	TODO:	Set sound transform or whatever
			}
		}
		
		#endregion
		
		/// <summary>
		/// Randomly chooses and returns one of the provided values.
		/// </summary>
		/// <param name="choices">The Objects you want to randomly choose from. Can be ints, floats, Points, etc.</param>
		/// <returns>A randomly chosen one of the provided parameters.</returns>
		public static T Choose<T>(params T[] choices)
		{
			if (choices.Length == 1 && choices[0] is List<T>)
			{
				var list = choices[0] as List<T>;
				return list[(int) Rand((uint) list.Count)];
			}
			
			return choices[(int) Rand((uint) choices.Length)];
		}
		
		/// <summary>
		/// Finds the sign of the provided value.
		/// </summary>
		/// <param name="value">The value to evaluate.</param>
		/// <returns>1 if value is greater than 0, -1 if value is less than 0, and 0 when value == 0.</returns>
		public static int Sign(float value)
		{
			return value < 0 ? -1 : (value > 0 ? 1 : 0);
		}
		
		/// <summary>
		/// Approaches the value towards the target, by the specified amount, without overshooting the target.
		/// </summary>
		/// <param name="value">The starting value.</param>
		/// <param name="target">The target that you want value to approach.</param>
		/// <param name="amount">How much you want the value to approach target by.</param>
		/// <returns>The new value.</returns>
		public static float Approach(float value, float target, float amount)
		{
			if (value < target - amount)
			{
				return value + amount;
			}
			else if (value > target + amount)
			{
				return value - amount;
			}
			else
			{
				return target;
			}
		}
		
		/// <summary>
		/// Linear interpolation between two values.
		/// </summary>
		/// <param name="a">First value</param>
		/// <param name="b">Second value</param>
		/// <param name="t">Interpolation factor</param>
		/// <returns>When t=0, returns a. When t=1, returns b. When t=0.5, will return halfway between a and b. Etc.</returns>
		public static float Lerp(float a, float b, float t = 1)
		{
			return a + (b - a) * t;
		}
		
		/// <summary>
		/// Linear interpolation between two colors.
		/// </summary>
		/// <param name="from">First color.</param>
		/// <param name="to">Second color.</param>
		/// <param name="t">Interpolation value. Clamped to the range [0, 1].</param>
		/// <returns>RGB component-interpolated color value.</returns>
		public static uint ColorLerp(uint from, uint to, float t)
		{
			if (t <= 0) { return from; }
			if (t >= 1) { return to; }
			float a = from >> 24 & 0xFF;
			float r = from >> 16 & 0xFF;
			float g = from >> 8 & 0xFF;
			float b = from & 0xFF;
			
			float da = (to >> 24 & 0xFF) - a;
			float dr = (to >> 16 & 0xFF) - r;
			float db = (to >> 8 & 0xFF) - g;
			float dg = (to & 0xFF) - b;

			a += da * t;
			r += dr * t;
			g += dg * t;
			b += db * t;
		
			return (uint) a << 24 | (uint) r << 16 | (uint) g << 8 | (uint) b;
		}
		
		/// <summary>
		/// Steps X and Y coordinates towards a point.
		/// </summary>
		/// <param name="x">The X value to move</param>
		/// <param name="y">The Y value to move</param>
		/// <param name="toX">X position to step towards.</param>
		/// <param name="toY">Y position to step towards.</param>
		/// <param name="distance">The distance to step (will not overshoot target).</param>
		public static void StepTowards(ref float x, ref float y, float toX, float toY, float distance)
		{
			var point = new Vector2f(toX - x, toY - y);
			
			if (point.Length() <= distance)
			{
				x = toX;
				y = toY;
				return;
			}
			
			point = point.Normalized(distance);
			
			x += point.X;
			y += point.Y;
		}
		
		/// <summary>
		/// Anchors the object to a position
		/// </summary>
		/// <param name="objX">X of the object to anchor</param>
		/// <param name="objY">Y of the object to anchor</param>
		/// <param name="anchorX">X of the anchor</param>
		/// <param name="anchorY">Y of the anchor</param>
		/// <param name="distance">The max distance that the object can be from the anchor</param>
		public static void AnchorTo(ref float objX, ref float objY, float anchorX, float anchorY, float distance = 0, float? minDistance = null)
		{
            var point = new Vector2f(objX - anchorX, objY - anchorY);

            if (point.Length() > distance)
            {
                point = point.Normalized(distance);
            }

            if (minDistance.HasValue && point.Length() < minDistance.Value)
            {
                point = point.Normalized(minDistance.Value);
            }

            objX = anchorX + point.X;
            objY = anchorY + point.Y;

		}
		
		/// <summary>
		/// Rotates the object around the anchor by the specified amount.
		/// </summary>
		/// <param name="objX">X position of the object to rotate around the anchor.</param>
		/// <param name="objY">Y position of the object to rotate around the anchor.</param>
		/// <param name="anchorX">X position of the anchor to rotate around.</param>
		/// <param name="anchorY">Y position of the anchor to rotate around.</param>
		/// <param name="angle">The amount of degrees to rotate by.</param>
		/// <param name="relative">Whether the rotation is relative (default true).</param>
		public static void RotateAround(ref float objX, ref float objY, float anchorX, float anchorY, float angle = 0, bool relative = true)
		{
			if (relative)
			{
				angle += FP.Angle(anchorX, anchorY, objX, objY);
			}
			
			FP.AngleXY(ref objX, ref objY, angle, FP.Distance(anchorX, anchorY, objX, objY), anchorX, anchorY);
			
		}
		
		/// <summary>
		/// Finds the angle (in degrees) from point 1 to point 2.
		/// </summary>
		/// <param name="x1">The first x-position.</param>
		/// <param name="y1">The first y-position.</param>
		/// <param name="x2">The second x-position.</param>
		/// <param name="y2">The second y-position.</param>
		/// <returns>The angle from (x1, y1) to (x2, y2).</returns>
		public static float Angle(float x1, float y1, float x2, float y2)
		{
			double a = Math.Atan2(y2 - y1, x2 - x1) * DEG;
			return (float) (a < 0 ? a + 360 : a);
		}
		
		/// <summary>
		/// Sets the x/y values of an object to a vector of the specified angle and length.
		/// </summary>
		/// <param name="X">X coordinate of the object to set.</param>
		/// <param name="Y">Y coordinate of the object to set.</param>
		/// <param name="angle">The angle of the vector, in degrees.</param>
		/// <param name="length">The distance to the vector from (0, 0).</param>
		/// <param name="xOffset">X offset.</param>
		/// <param name="yOffset">Y offset.</param>
		public static void AngleXY(ref float X, ref float Y, float angle, float length, float xOffset = 0, float yOffset = 0)
		{
			angle *= RAD;
			X = (float) (Math.Cos(angle) * length + xOffset);
			Y = (float) (Math.Sin(angle) * length + yOffset);
		}
		
		/// <summary>
		/// Gets the difference of two angles, wrapped around to the range -180 to 180.
		/// </summary>
		/// <param name="a">First angle in degrees.</param>
		/// <param name="b">Second angle in degrees.</param>
		/// <returns>Difference in angles, wrapped around to the range -180 to 180.</returns>
		public static float AngleDiff(float a, float b)
		{
			float diff = a - b;
			
			while (diff > 180) { diff -= 360; }
			while (diff <= -180) { diff += 360; }

			return diff;
		}
		
		/// <summary>
		/// Find the distance between two points.
		/// </summary>
		/// <param name="x1">The first x-position.</param>
		/// <param name="y1">The first y-position.</param>
		/// <param name="x2">The second x-position.</param>
		/// <param name="y2">The second y-position.</param>
		/// <returns>The distance.</returns>
		public static float Distance(float x1, float y1, float x2, float y2)
		{
			return (float) Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
		}
		
		/// <summary>
		/// Find the distance between two rectangles. Will return 0 if the rectangles overlap.
		/// </summary>
		/// <param name="x1">The x-position of the first rect.</param>
		/// <param name="y1">The y-position of the first rect.</param>
		/// <param name="w1">The width of the first rect.</param>
		/// <param name="h1">The height of the first rect.</param>
		/// <param name="x2">The x-position of the second rect.</param>
		/// <param name="y2">The y-position of the second rect.</param>
		/// <param name="w2">The width of the second rect.</param>
		/// <param name="h2">The height of the second rect.</param>
		/// <returns>The distance.</returns>
		public static float DistanceRects(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
		{
			if (x1 < x2 + w2 && x2 < x1 + w1)
			{
				if (y1 < y2 + h2 && y2 < y1 + h1) return 0;
				if (y1 > y2) return y1 - (y2 + h2);
				return y2 - (y1 + h1);
			}
			if (y1 < y2 + h2 && y2 < y1 + h1)
			{
				if (x1 > x2) return x1 - (x2 + w2);
				return x2 - (x1 + w1);
			}
			if (x1 > x2)
			{
				if (y1 > y2) return Distance(x1, y1, (x2 + w2), (y2 + h2));
				return Distance(x1, y1 + h1, x2 + w2, y2);
			}
			if (y1 > y2) return Distance(x1 + w1, y1, x2, y2 + h2);
			return Distance(x1 + w1, y1 + h1, x2, y2);
		}
		
		/// <summary>
		/// Find the distance between a point and a rectangle. Returns 0 if the point is within the rectangle.
		/// </summary>
		/// <param name="px">The x-position of the point.</param>
		/// <param name="py">The y-position of the point.</param>
		/// <param name="rx">The x-position of the rect.</param>
		/// <param name="ry">The y-position of the rect.</param>
		/// <param name="rw">The width of the rect.</param>
		/// <param name="rh">The height of the rect.</param>
		/// <returns>The distance.</returns>
		public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh)
		{
			if (px >= rx && px <= rx + rw)
			{
				if (py >= ry && py <= ry + rh) return 0;
				if (py > ry) return py - (ry + rh);
				return ry - py;
			}
			if (py >= ry && py <= ry + rh)
			{
				if (px > rx) return px - (rx + rw);
				return rx - px;
			}
			if (px > rx)
			{
				if (py > ry) return Distance(px, py, rx + rw, ry + rh);
				return Distance(px, py, rx + rw, ry);
			}
			if (py > ry) return Distance(px, py, rx, ry + rh);
			return Distance(px, py, rx, ry);
		}
		
		/// <summary>
		/// Clamps the value within the minimum and maximum values.
		/// </summary>
		/// <param name="value">The Number to evaluate.</param>
		/// <param name="min">The minimum range.</param>
		/// <param name="max">The maximum range.</param>
		/// <returns>The clamped value.</returns>
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (max.CompareTo(min) > 0)
			{
				if (value.CompareTo(min) < 0) return min;
				else if (value.CompareTo(max) > 0) return max;
				else return value;
			} else {
				// Min/max swapped
				if (value.CompareTo(max) < 0) return max;
				else if (value.CompareTo(min) > 0) return min;
				else return value;
			}
		}
		
		/// <summary>
		/// Clamps the object inside the rectangle.
		/// </summary>
		/// <param name="objX">The X property of the object to clamp.</param>
		/// <param name="objY">The Y property of the object to clamp.</param>
		/// <param name="x">Rectangle's x.</param>
		/// <param name="y">Rectangle's y.</param>
		/// <param name="width">Rectangle's width.</param>
		/// <param name="height">Rectangle's height.</param>
		/// <param name="padding">Optional padding around the edges.</param>
		public static void ClampInRect(ref float objX, ref float objY, float x, float y, float width, float height, float padding = 0)
		{
			objX = Clamp(objX, x + padding, x + width - padding);
			objY = Clamp(objY, y + padding, y + height - padding);
		}
		
		/// <summary>
		/// Transfers a value from one scale to another scale. For example, scale(.5, 0, 1, 10, 20) == 15, and scale(3, 0, 5, 100, 0) == 40.
		/// </summary>
		/// <param name="value">The value on the first scale.</param>
		/// <param name="min">The minimum range of the first scale.</param>
		/// <param name="max">The maximum range of the first scale.</param>
		/// <param name="min2">The minimum range of the second scale.</param>
		/// <param name="max2">The maximum range of the second scale.</param>
		/// <returns>The scaled value.</returns>
		public static float Scale(float value, float min, float max, float min2, float max2)
		{
			return min2 + ((value - min) / (max - min)) * (max2 - min2);
		}
		
		/// <summary>
		/// Transfers a value from one scale to another scale, but clamps the return value within the second scale.
		/// </summary>
		/// <param name="value">The value on the first scale.</param>
		/// <param name="min">The minimum range of the first scale.</param>
		/// <param name="max">The maximum range of the first scale.</param>
		/// <param name="min2">The minimum range of the second scale.</param>
		/// <param name="max2">The maximum range of the second scale.</param>
		/// <returns>The scaled and clamped value.</returns>
		public static float ScaleClamped(float value, float min, float max, float min2, float max2)
		{
			value = min2 + ((value - min) / (max - min)) * (max2 - min2);
			if (max2 > min2)
			{
				value = value < max2 ? value : max2;
				return value > min2 ? value : min2;
			}
			value = value < min2 ? value : min2;
			return value > max2 ? value : max2;
		}
		
		#region Random number generator
		
		/// <summary>
		/// The random seed used by FP's random functions.
		/// </summary>
		public static uint RandomSeed
		{
			get
			{
				return _getseed;
			}
			
			set
			{
				_seed = Clamp(value, 1u, 2147483646u);
				_getseed = _seed;
			}
		}
		
		/// <summary>
		/// Randomizes the random seed using C#'s Random class.
		/// </summary>
		public static void RandomizeSeed()
		{
			_seed = (uint) new Random(Guid.NewGuid().GetHashCode()).Next();
		}
		
		/// <summary>
		/// A pseudo-random float produced using FP's random seed, where the result is greater than 0 and less than 1.
		/// </summary>
		public static float Random
		{
			get
			{
				unchecked
				{
					_seed = (_seed * 16807) % 2147483647;
					return _seed / 2147483647.0f;
				}
			}
		}
		
		/// <summary>
		/// Returns a pseudo-random uint.
		/// </summary>
		/// <param name="amount">The returned value will always be [0, result, amount].</param>
		/// <returns>The value.</returns>
		public static uint Rand(uint amount)
		{
			unchecked
			{
				_seed = (_seed * 16807) % 2147483647;
				return (uint) Math.Floor((_seed / 2147483647.0f) * amount);
			}
		}
		
		#endregion
		
		/// <summary>
		/// Returns the next item after current in the list of options.
		/// </summary>
		/// <param name="current">The currently selected item (must be one of the options).</param>
		/// <param name="options">An array of all the items to cycle through.</param>
		/// <param name="loop">If true, will jump to the first item after the last item is reached.</param>
		/// <returns>The next item in the list.</returns>
		public static T Next<T>(T current, IList<T> options, bool loop = true)
		{
			if (loop) return options[(options.IndexOf(current) + 1) % options.Count];
			return options[Math.Max(options.IndexOf(current) + 1, options.Count - 1)];
		}
		
		/// <summary>
		/// Returns the previous item before current in the list of options.
		/// </summary>
		/// <param name="current">The currently selected item (must be one of the options).</param>
		/// <param name="options">An array of all the items to cycle through.</param>
		/// <param name="loop">If true, will jump to the last item after the first item is reached.</param>
		/// <returns>The next item in the list.</returns>
		public static T Prev<T>(T current, IList<T> options, bool loop = true)
		{
			if (loop) return options[((options.IndexOf(current) - 1) + options.Count) % options.Count];
			return options[Math.Max(options.IndexOf(current) - 1, 0)];
		}
		
		/// <summary>
		/// Swaps the current item between a and b. Useful for quick state/string/value swapping.
		/// </summary>
		/// <param name="current">The currently selected item.</param>
		/// <param name="a">Item a</param>
		/// <param name="b">Item b</param>
		/// <returns>Returns a if current is b, and b if current is a.</returns>
		public static T Swap<T>(T current, T a, T b)
		{
			return current.Equals(a) ? b : a;
		}
		
		#region Color stuff
		
		/// <summary>
		/// Creates a color value by combining the chosen RGB values.
		/// </summary>
		/// <param name="r">The red value of the color, from 0 to 255.</param>
		/// <param name="g">The green value of the color, from 0 to 255.</param>
		/// <param name="b">The blue value of the color, from 0 to 255.</param>
		/// <returns>The combined color.</returns>
		public static uint GetColorRGB(uint r, uint g, uint b)
		{
			return r << 16 | g << 8 | b;
		}
		
		/// <summary>
		/// Creates a color value with the chosen HSV values.
		/// </summary>
		/// <param name="h">The hue of the color (from 0 to 1).</param>
		/// <param name="s">The saturation of the color (from 0 to 1).</param>
		/// <param name="v">The value of the color (from 0 to 1).</param>
		/// <returns>The created color</returns>
		public static uint GetColorHSV(uint h, uint s, uint v)
		{
			h = h < 0 ? 0 : (h > 1 ? 1 : h);
			s = s < 0 ? 0 : (s > 1 ? 1 : s);
			v = v < 0 ? 0 : (v > 1 ? 1 : v);
			h = (h * 360);
			
			int hi = (int) (h / 60) % 6;
			float f = h / 60 - (int) (h / 60);
			float p = (v * (1 - s));
			float q = (v * (1 - f * s));
			float t = (v * (1 - (1 - f) * s));
			
			switch (hi)
			{
				case 0: return (uint) v * 255 << 16 | (uint) t * 255 << 8 | (uint) p * 255;
				case 1: return (uint) q * 255 << 16 | (uint) v * 255 << 8 | (uint) p * 255;
				case 2: return (uint) p * 255 << 16 | (uint) v * 255 << 8 | (uint) t * 255;
				case 3: return (uint) p * 255 << 16 | (uint) q * 255 << 8 | (uint) v * 255;
				case 4: return (uint) t * 255 << 16 | (uint) p * 255 << 8 | (uint) v * 255;
				case 5: return (uint) v * 255 << 16 | (uint) p * 255 << 8 | (uint) q * 255;
				default: return 0;
			}
		}
		
		/// <summary>
		/// Finds the red factor of a color.
		/// </summary>
		/// <param name="color">The color to evaluate.</param>
		/// <returns>A uint from 0 to 255.</returns>
		public static byte GetRed(uint color)
		{
			return (byte) (color >> 16 & 0xFF);
		}
		
		/// <summary>
		/// Finds the green factor of a color.
		/// </summary>
		/// <param name="color">The color to evaluate.</param>
		/// <returns>A uint from 0 to 255.</returns>
		public static byte GetGreen(uint color)
		{
			return (byte) (color >> 8 & 0xFF);
		}
		
		/// <summary>
		/// Finds the blue factor of a color.
		/// </summary>
		/// <param name="color">The color to evaluate.</param>
		/// <returns>A uint from 0 to 255.</returns>
		public static byte GetBlue(uint color)
		{
			return (byte) (color & 0xFF);
		}
		
		/// <summary>
		/// Create an SFML color instance from a Flash-style uint (0xRRGGBB)
		/// </summary>
		/// <param name="color">A Flash-style color (0xRRGGBB)</param>
		/// <returns>The converted color</returns>
		public static Color Color(uint color)
		{
			return new Color(FP.GetRed(color), FP.GetGreen(color), FP.GetBlue(color));
		}
		
		/// <summary>
		/// Create a Flash-style color (0xRRGGBB) from an SFML color instance.
		/// </summary>
		/// <param name="color">An SFML color instance.</param>
		/// <returns>The converted color.</returns>
		public static uint HexColor(Color color)
		{
			return GetColorRGB(color.R, color.G, color.B);
		}
		
		#endregion
		
		/// <summary>
		/// Sets a time flag.
		/// </summary>
		/// <returns>Time elapsed (in milliseconds) since the last time flag was set.</returns>
		public static uint TimeFlag()
		{
			uint t = (uint) Timer.ElapsedMilliseconds;
			uint e = t - _time;
			_time = t;
			return e;
		}
		
		public static Debugging.Console Console { get; internal set; }
		
		/// <summary>
		/// Logs data to the console
		/// </summary>
		/// <param name="data">The data parameters to log, can be variables, objects, etc. Parameters will be separated by a space (" ").</param>
		public static void Log(params object[] data)
		{
			Console.Log(data);
		}
		
		public static void Error(params object[] data)
		{
			Console.Error(data);
		}
		
		//	TODO:	can we do the Watch list thing? Do we ever use it?
		
		//	FP loads XML here. This is replaced by the content class.
		
		/// <summary>
		/// Global Tweener for tweening values across multiple worlds.
		/// </summary>
		public static Tweener Tweener { get; internal set; }
		
		/// <summary>
		/// Tweens numeric public properties of an Object.
		/// Shorthand for creating a MultiVarTween tween, starting it and adding it to a Tweener.
		/// </summary>
		/// <param name="target">The object containing the properties to tween.</param>
		/// <param name="values">An object containing key/value pairs of properties and target values.</param>
		/// <param name="duration">Duration of the tween.</param>
		/// <param name="options">An object containing key/value pairs of the following optional parameters:
		/// 							type 		Tween type.
		/// 							complete 	Optional completion callback function.
		/// 							ease 		Optional easer function.
		/// 							tweener 	The Tweener to add this Tween to.
		/// 							delay 		A length of time to wait before starting this tween.
		/// </param>
		/// <example>FP.Tween(object, new { x = 500, y = 350 }, 2.0f, new { ease = easeFunction, complete = onComplete } );</example>
		/// <returns>The added MultiVarTween object.</returns>
		public static MultiVarTween Tween(object target, object values, float duration, object options = null)
		{
			uint type = Punk.Tween.ONESHOT;
			Tween.OnComplete complete = null;
			Tween.Easer ease = null;
			Tweener tweener = FP.Tweener;
			float delay = 0;
			
			if (target is Tweener)
				tweener = target as Tweener;
			
			if (options != null)
			{
				if (options is Tween.OnComplete)		complete = options as Tween.OnComplete;
				if (options.HasOwnProperty("type"))		type = options.GetProp<uint>("type");
				if (options.HasOwnProperty("complete"))	complete = options.GetProp<Tween.OnComplete>("complete");
				if (options.HasOwnProperty("ease"))		ease = options.GetProp<Tween.Easer>("ease");
				if (options.HasOwnProperty("tweener"))	tweener = options.GetProp<Tweener>("tweener");
				if (options.HasOwnProperty("delay"))	delay = options.GetProp<float>("delay");
			}
			
			var tween = new MultiVarTween(complete, type);
			tween.Tween(target, values, duration, ease, delay);
			tweener.AddTween(tween);
			return tween;
		}
		
		/// <summary>
		/// Schedules a callback for the future.
		/// Shorthand for creating an Alarm tween, starting it and adding it to a Tweener.
		/// </summary>
		/// <param name="delay">The duration to wait before calling the callback.</param>
		/// <param name="complete">The function to be called.</param>
		/// <param name="type">The tween type (PERSIST, LOOPING or ONESHOT). Defaults to ONESHOT.</param>
		/// <param name="tweener">The Tweener object to add this Alarm to. Defaults to FP.Tweener.</param>
		/// <returns>The added Alarm object.</returns>
		public static Alarm Alarm(float delay, Tween.OnComplete complete, uint type = 2, Tweener tweener = null)
		{
			tweener = tweener ?? FP.Tweener;
			
			var alarm = new Alarm(delay, complete, type);
			Tweener.AddTween(alarm, true);
			return alarm;
		}
		
		/// <summary>
		/// Gets an array of frame indices.
		/// </summary>
		/// <param name="from">Starting frame.</param>
		/// <param name="to">Ending frame.</param>
		/// <param name="skip">Skip amount every frame (eg. use 1 for every 2nd frame).</param>
		/// <returns></returns>
		public static int[] MakeFrames(int from, int to, int skip = 0)
		{
			var a = new List<int>();
			skip++;
			
			if (from < to)
			{
				while (from <= to)
				{
					a.Add(from);
					from += skip;
				}
			}
			else
			{
				while (from >= to)
				{
					a.Add(from);
					from -= skip;
				}
			}
			return a.ToArray();
		}
		
		/// <summary>
		/// Create a frame array.
		/// </summary>
		/// <param name="frames">The frames to insert into the array</param>
		/// <returns>The frame array.</returns>
		public static int[] Frames(params int[] frames)
		{
			return frames;
		}
		
		/// <summary>
		/// Shuffles the elements in a list.
		/// </summary>
		/// <param name="shuffle">The list to shuffle</param>
		public static void Shuffle<T>(List<T> shuffle)
		{
			int i = shuffle.Count;
			int j = 0;	//	initial value is never used
			
			while (--i > 0)
			{
				T t = shuffle[i];
				shuffle[i] = shuffle[j = (int) Rand((uint) i + 1U)];
				shuffle[j] = t;
			}
		}
		
		//	FP has a sort method here, but that's built into the List<T> class
		
		#region World information
		internal static World _world;
		internal static World _goto;
		#endregion
		
		
		#region Console information
		//	TODO:	internal static var _console:Console;
		#endregion
		
		#region Timing information
		
		/// <summary>
		/// High-resolution timer.
		/// </summary>
		/// <returns>Milliseconds since the Engine started up.</returns>
		public static uint GetTimer()
		{
			return (uint) FP.Timer.ElapsedMilliseconds;
		}
		
		/// <summary>
		/// Time since the game started
		/// </summary>
		internal static Stopwatch Timer;
		
		internal static uint _time;
		internal static uint _updateTime;
		internal static uint _renderTime;
		internal static uint _gameTime;
		internal static uint _platformTime;
		#endregion
		
		#region Random
		// Pseudo-random number generation (the seed is set in Engine's constructor).
		private static uint _getseed;
		private static uint _seed;
		#endregion
		
		#region Volume control
		private static float _volume = 1;
		private static float _pan = 0;
		//	TODO:	private static var _soundTransform:SoundTransform = new SoundTransform;
		#endregion
		
		// Used for rad-to-deg and deg-to-rad conversion.
		
		/// <summary>
		/// Multiply a radian angle value by this to convert it to degrees.
		/// </summary>
		public const float DEG = (float) -180 / (float) Math.PI;
		
		/// <summary>
		/// Multiply a degree angle by this to convert it to radians.
		/// </summary>
		public const float RAD = (float) Math.PI / -180;
		
		#region Globals
		
		/// <summary>
		/// The currently active Engine.
		/// </summary>
		public static Punk.Engine Engine;
		#endregion
		
		//	TODO:	get these back in order if needed
		// Global objects used for rendering, collision, etc.
		internal static Camera _dummyCamera = new Camera();
//		/** @private */ public static var point:Point = new Point;
//		/** @private */ public static var point2:Point = new Point;
//		/** @private */ public static var zero:Point = new Point;
//		/** @private */ public static var rect:Rectangle = new Rectangle;
//		/** @private */ public static var matrix:Matrix = new Matrix;
//		/** @private */ public static var sprite:Sprite = new Sprite;
//		/** @private */ public static var entity:Entity;
	}
}
