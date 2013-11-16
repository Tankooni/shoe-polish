/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/13/2013
 * Time: 1:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Punk.Utils
{
	/// <summary>
	/// Description of InputStates.
	/// </summary>
	internal class InputStates<T>
	{
		public InputStates()
		{
			down = new Dictionary<T, bool>();
			pressed = new Dictionary<T, bool>();
			released = new Dictionary<T, bool>();
		}
		
		public void InitCycle()
		{
			inputs = pressed.Keys.ToList();
		}
		
		public Dictionary<T, bool> down;
		public Dictionary<T, bool> pressed;
		public Dictionary<T, bool> released;
		public List<T> inputs;
		
		public void Cycle()
		{
			foreach (var input in inputs)
			{
				pressed[input] = false;
			}
			
			foreach (var input in inputs)
			{
				released[input] = false;
			}
		}
	}
}
