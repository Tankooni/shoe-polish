/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 6/28/2013
 * Time: 1:10 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Punk.Utils;
using SFML.Window;

namespace Punk.Tweens.Motion
{
	/// <summary>
	/// Description of QuadPath.
	/// </summary>
	public class QuadPath : Motion
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="complete">Optional completion callback.</param>
		/// <param name="type">Tween type.</param>
		public QuadPath(OnComplete complete = null, uint type = 0) : 
		base(0, complete, type, null)
		{
			_points = new List<Vector2f>();
			_curve = new List<Vector2f>();
			_curveT = new List<float>();
			_curveD = new List<float>();
			
			_curveT.Add(0);
		}
		
		/**
		 * Starts moving along the path.
		 * @param	duration	Duration of the movement.
		 * @param	ease		Optional easer function.
		 */
		public void SetMotion(float duration, Easer ease = null)
		{
			UpdatePath();
			_target = duration;
			_speed = _distance / duration;
			_ease = ease;
			Start();
		}
		
		/**
		 * Starts moving along the path at the speed.
		 * @param	speed		Speed of the movement.
		 * @param	ease		Optional easer function.
		 */
		public void SetMotionSpeed(float speed, Easer ease = null)
		{
			UpdatePath();
			_target = _distance / speed;
			_speed = speed;
			_ease = ease;
			Start();
		}
		
		/**
		 * Adds the point to the path.
		 * @param	x		X position.
		 * @param	y		Y position.
		 */
		public void AddPoint(float x = 0, float y = 0)
		{
			_updateCurve = true;
			if (_points.Count == 0)
			{
				_curve.Add(new Vector2f(x, y));
				X = x;
				Y = y;
			}
			
			_points.Add(new Vector2f(x, y));
		}
		
		/**
		 * Gets the point on the path.
		 * @param	index		Index of the point.
		 * @return	The Point object.
		 */
		public Vector2f GetPoint(uint index = 0)
		{
			if (_points.Count == 0)
			{
				throw new Exception("No points have been added to the path yet.");
			}
			
			return _points[(int) (index % _points.Count)];
		}
		
		/** @private Starts the Tween. */
		override public void Start() 
		{
			_index = 0;
			base.Start();
		}
		
		/** @private Updates the Tween. */
		override public void Update() 
		{
			base.Update();
			
			if (_index < _curve.Count - 1)
			{
				while (_t > _curveT[(int) (_index + 1)])
				{
					_index ++;
				}
			}
			
			float td = _curveT[(int) _index];
			float tt = _curveT[(int) (_index + 1)] - td;
			
			td = (_t - td) / tt;
			_a = _curve[(int) _index];
			_b = _points[(int) (_index + 1)];
			_c = _curve[ (int) (_index + 1)];
			X = _a.X * (1 - td) * (1 - td) + _b.X * 2 * (1 - td) * td + _c.X * td * td;
			Y = _a.Y * (1 - td) * (1 - td) + _b.Y * 2 * (1 - td) * td + _c.Y * td * td;
		}
		
		/** @private Updates the path, preparing the curve. */
		private void UpdatePath()
		{
			if (_points.Count < 3)	throw new Exception("A QuadPath must have at least 3 points to operate.");
			if (!_updateCurve) return;
			_updateCurve = false;
			
			// produce the curve points
			Vector2f p;
			Vector2f c;
			Vector2f l = _points[1];
			int i = 2;
			while (i < _points.Count)
			{
				p = _points[i];
				if (_curve.Count > i - 1)
				{
					c = _curve[i - 1];
				}
				else
				{
					_curve.Insert(i - 1, new Vector2f());
					c = _curve[i - 1];
				}
				
				if (i < _points.Count - 1)
				{
					c.X = l.X + (p.X - l.X) / 2;
					c.Y = l.Y + (p.Y - l.Y) / 2;
				}
				else
				{
					c.X = p.X;
					c.Y = p.Y;
				}
				
				_curve[i - 1] = c;
				
				l = p;
				i ++;
			}
			
			// find the total distance of the path
			i = 0;
			_distance = 0;
			while (i < _curve.Count - 1)
			{
				_curveD.Add(CurveLength(_curve[i], _points[i + 1], _curve[i + 1]));
				_distance += _curveD[i ++];
			}
			
			// find t for each point on the curve
			i = 1;
			float d = 0;
			while (i < _curve.Count - 1)
			{
				d += _curveD[i];
				_curveT.Add(d / _distance);
			}
			
			_curveT.Add(1);
		}
		
		/**
		 * Amount of points on the path.
		 */
		public float PointCount
		{
			get { return _points.Count; }
		}
		
		/** @private Calculates the length of the curve. */
		private float CurveLength(Vector2f start, Vector2f control, Vector2f finish)
		{
			var a = new Vector2f();
			var b = new Vector2f();
			
			a.X = start.X - 2 * control.X + finish.X;
			a.Y = start.Y - 2 * control.Y + finish.Y;
			b.X = 2 * control.X - 2 * start.X;
			b.Y = 2 * control.Y - 2 * start.Y;
			float A = 4 * (a.X * a.X + a.Y * a.Y);
			float B = 4 * (a.X * b.X + a.Y * b.Y);
			float C = b.X * b.X + b.Y * b.Y;
			float ABC = 2 * (float) Math.Sqrt(A + B + C);
			float A2 = (float) Math.Sqrt(A);
			float A32 = 2 * A * A2;
			float C2 = 2 * (float) Math.Sqrt(C);
			float BA = B / A2;
			return (A32 * ABC + A2 * B * (ABC - C2) + (4 * C * A - B * B) * (float) Math.Log((2 * A2 + BA + ABC) / (BA + C2))) / (4 * A32);
		}
		
		// Path information.
		private List<Vector2f> _points;
		private float _distance = 0;
		private float _speed = 0;
		private uint _index = 0;
		
		// Curve information.
		private bool _updateCurve = true;
		private List<Vector2f> _curve;
		private List<float> _curveT;
		private List<float> _curveD;
		
		// Curve points.
		private Vector2f _a;
		private Vector2f _b;
		private Vector2f _c;
	}
}
