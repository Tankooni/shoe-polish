/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/13/2013
 * Time: 1:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using SFML.Window;

namespace Punk.Utils
{
	/// <summary>
	/// Description of InputList.
	/// </summary>
	public class InputList
	{
		internal InputList(params object[] args)
		{
			foreach (object input in args)
			{
				AddInput(input);
			}
		}
		
		public void AddInput(object input)
		{
			if (input is Keyboard.Key)
			{
				AddKey((Keyboard.Key) input);
			}
			else if (input is Mouse.Button)
			{
				AddButton((Mouse.Button) input);
			}
			else
			{
				throw new ArgumentException("Inputs must be of type Keyboard.Key or Mouse.Button");
			}
		}
		
		public void AddKey(Keyboard.Key key)
		{
			if (!keys.Contains(key))	keys.Add(key);
		}
		
		public void AddButton(Mouse.Button button)
		{
			if (!mouseButtons.Contains(button))	mouseButtons.Add(button);
		}
		
		 internal List<Mouse.Button> mouseButtons;
		 internal List<Keyboard.Key> keys;
		
	}
}
