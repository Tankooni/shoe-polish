/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/7/2013
 * Time: 5:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using SFML.Window;

namespace Punk.Utils
{
	/// <summary>
	/// Static class updated by Engine. Use for defining and checking keyboard/mouse input.
	/// </summary>
	public static class Input
	{
		/// <summary>
		/// An updated string containing the last 100 characters pressed on the keyboard.
		/// Useful for creating text input fields, such as highscore entries, etc.
		/// </summary>
		public static string KeyString = "";
		
		public static event EventHandler<JoystickConnectEventArgs> ControllerConnected;
		public static event EventHandler<JoystickConnectEventArgs> ControllerDisconnected;
		
		/// <summary>
		/// The last key pressed.
		/// </summary>
		public static Keyboard.Key LastKey;
		
		/// <summary>
		/// Called by engine. Sets up event listeners
		/// </summary>
		internal static void Init()
		{
			keys = new InputStates<Keyboard.Key>();
			mouseButtons = new InputStates<Mouse.Button>();
			
			firstJoystickCheck = 3;
			
			_mousePos = new Vector2f();
			
			var allkeys = Enum.GetValues(typeof(Keyboard.Key));
			var allbuttons = Enum.GetValues(typeof(Mouse.Button));
			
			foreach (Keyboard.Key key in allkeys)
			{
				keys.down.Add(key, false);
				keys.pressed.Add(key, false);
				keys.released.Add(key, false);
			}
			
			foreach (Mouse.Button button in allbuttons)
			{
				mouseButtons.down.Add(button, false);
				mouseButtons.pressed.Add(button, false);
				mouseButtons.released.Add(button, false);
			}
			
			mouseButtons.InitCycle();
			keys.InitCycle();
			
			ControllerConnected = (s, e) => {};
			ControllerDisconnected = (s, e) => {};
			
			FP.Screen.KeyPressed += OnKeyPressed;
			FP.Screen.KeyReleased += OnKeyReleased;
			
			FP.Screen.MouseButtonPressed += OnMousePressed;
			FP.Screen.MouseButtonReleased += OnMouseReleased;
			
			FP.Screen.MouseWheelMoved += OnMouseWheelMoved;
			
			FP.Screen.TextEntered += OnTextEntered;
			
			FP.Screen.JoystickConnected += OnJoystickConnected;
			FP.Screen.JoystickDisconnected += OnJoystickDisonnected;
			FP.Screen.JoystickButtonPressed += OnJoystickPressed;
			FP.Screen.JoystickButtonReleased += OnJoystickReleased;
			FP.Screen.JoystickMoved += OnJoystickMoved;
		}
		
		
		#region Event handlers
		private static void OnKeyPressed(object sender, KeyEventArgs e)
		{
			var key = e.Code;
			LastKey = key;
			
			if (!keys.down[key])
			{
				keys.pressed[key] = true;
				keys.down[key] = true;
			}
		}
		
		private static void OnKeyReleased(object sender, KeyEventArgs e)
		{
			var key = e.Code;
			
			keys.released[key] = true;
			keys.down[key] = false;
		}
		
		private static void OnMousePressed(object sender, MouseButtonEventArgs e)
		{
			var button = e.Button;
			
			if (!mouseButtons.down[button])
			{
				mouseButtons.pressed[button] = true;
				mouseButtons.down[button] = true;
			}
		}
		
		private static void OnMouseReleased(object sender, MouseButtonEventArgs e)
		{
			var button = e.Button;
			
			mouseButtons.released[button] = true;
			mouseButtons.down[button] = false;
		}
		
		private static void OnMouseWheelMoved(object sender, MouseWheelEventArgs e)
		{
			_mouseWheelDelta = e.Delta;
		}
		
		private static void OnTextEntered(object sender, TextEventArgs e)
		{
			if (e.Unicode == "\b" && KeyString.Length > 0)
			{
				KeyString = KeyString.Remove(KeyString.Length - 1, 1);
			}
			else
			{
				KeyString += e.Unicode;
			}
			
			if (KeyString.Length > MAX_KEYSTRING)
			{
				KeyString = KeyString.Remove(0, KeyString.Length - MAX_KEYSTRING);
			}
		}
		
		private static void OnJoystickConnected(object sender, JoystickConnectEventArgs e)
		{
			Controller.OnConnected(sender, e);
			ControllerConnected(sender, e);
		}
		
		private static void OnJoystickDisonnected(object sender, JoystickConnectEventArgs e)
		{
			Controller.OnDisconnected(sender, e);
			ControllerDisconnected(sender, e);
		}
		
		private static void OnJoystickMoved(object sender, JoystickMoveEventArgs e)
		{
			Controller.OnJoystickMoved(sender, e);
		}
		
		private static void OnJoystickPressed(object sender, JoystickButtonEventArgs e)
		{
			Controller.OnJoystickPressed(sender, e);
		}
		
		private static void OnJoystickReleased(object sender, JoystickButtonEventArgs e)
		{
			Controller.OnJoystickReleased(sender, e);
		}
		#endregion
		
		#region Mouse
		/// <summary>
		/// If the mouse wheel was moved this frame, this was the delta.
		/// </summary>
		public static int MouseWheelDelta
		{
			get
			{
				return _mouseWheelDelta;
			}
		}
		
		/// <summary>
		/// If the mouse wheel was moved this frame.
		/// </summary>
		public static bool MouseWheel
		{
			get
			{
				return _mouseWheelDelta != 0;
			}
		}
		
		/// <summary>
		/// X position of the mouse on the screen.
		/// </summary>
		public static int MouseX
		{
			get
			{
				return (int) _mousePos.X;
			}
		}
		
		/// <summary>
		/// Y position of the mouse on the screen.
		/// </summary>
		public static int MouseY
		{
			get
			{
				return (int) _mousePos.Y;
			}
		}
		#endregion
		
		#region Status
		
		/// <summary>
		/// Defines a new input.
		/// </summary>
		/// <param name="name">String to map the input to.</param>
		/// <param name="inputs">The keys or buttons to use for the Input.</param>
		public static void Define(string name, params object[] inputs)
		{
			if (_control.ContainsKey(name))
			{
				var current = _control[name];
				
				foreach (var input in inputs)
				{
					current.AddInput(input);
				}
			}
			else
			{
				_control.Add(name, new InputList(inputs));
			}
		}
		
		/// <summary>
		///	If the input is held down.
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input or key is held down.</returns>
		public static bool Check(string input)
		{
			if (!_control.ContainsKey(input))
			{
				return false;
			}
			
			foreach (var key in _control[input].keys)
			{
				if (keys.down[key])
				{
					return true;
				}
			}
			
			foreach (var button in _control[input].mouseButtons)
			{
				if (mouseButtons.down[button])
				{
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// If the key is held down.
		/// </summary>
		/// <param name="input">The key to check.</param>
		/// <returns>If the key is held down.</returns>
		public static bool Check(Keyboard.Key input)
		{
			return keys.down[input];
		}
		
		/// <summary>
		/// If the mouse button is held down.
		/// </summary>
		/// <param name="input">The button to check.</param>
		/// <returns>If the key is held down.</returns>
		public static bool Check(Mouse.Button input)
		{
			return mouseButtons.down[input];
		}
		
		/// <summary>
		///	If the input is held down.
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input or key is held down.</returns>
		public static bool Down(string input)
		{
			return Check(input);
		}
		
		/// <summary>
		/// If the key is held down.
		/// </summary>
		/// <param name="input">The key to check.</param>
		/// <returns>If the key is held down.</returns>
		public static bool Down(Keyboard.Key input)
		{
			return Check(input);
		}
		
		/// <summary>
		/// If the mouse button is held down.
		/// </summary>
		/// <param name="input">The button to check.</param>
		/// <returns>If the button is held down.</returns>
		public static bool Down(Mouse.Button input)
		{
			return Check(input);
		}
		
		/// <summary>
		/// If the input was pressed this frame
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input, key or button was pressed this frame.</returns>
		public static bool Pressed(string input)
		{
			foreach (var key in _control[input].keys)
			{
				if (keys.pressed[key])
				{
					return true;
				}
			}
			
			foreach (var button in _control[input].mouseButtons)
			{
				if (mouseButtons.pressed[button])
				{
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// If the key was pressed this frame.
		/// </summary>
		/// <param name="input">The key to check.</param>
		/// <returns>If the key was pressed this frame.</returns>
		public static bool Pressed(Keyboard.Key input)
		{
			return keys.pressed[input];
		}
		
		/// <summary>
		/// If the button was pressed this frame.
		/// </summary>
		/// <param name="input">The button to check.</param>
		/// <returns>If the button was pressed this frame.</returns>
		public static bool Pressed(Mouse.Button input)
		{
			return mouseButtons.pressed[input];
		}
		
		/// <summary>
		/// If the input was released this frame.
		/// </summary>
		/// <param name="input">The input to check.</param>
		/// <returns>If the input was released this frame.</returns>
		public static bool Released(string input)
		{
			
			if (!_control.ContainsKey(input))
			{
				return false;
			}
			
			foreach (var key in _control[input].keys)
			{
				if (keys.released[key])
				{
					return true;
				}
			}
			
			foreach (var button in _control[input].mouseButtons)
			{
				if (mouseButtons.released[button])
				{
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>
		/// If the key was released this frame.
		/// </summary>
		/// <param name="input">The key to check.</param>
		/// <returns>If the key was released this frame.</returns>
		public static bool Released(Keyboard.Key input)
		{
			return keys.released[input];
		}
		
		/// <summary>
		/// If the button was released this frame.
		/// </summary>
		/// <param name="input">The button to check.</param>
		/// <returns>If the button was released this frame.</returns>
		public static bool Released(Mouse.Button input)
		{
			return mouseButtons.released[input];
		}
		
		#endregion
		
		/// <summary>
		/// Returns the inputs mapped to the input name.
		/// </summary>
		/// <param name="name">The input name.</param>
		/// <returns>A list of InputSources</returns>
		public static InputList Inputs(string name)
		{
			return _control[name];
		}
		
		/// <summary>
		/// Called by Engine to update the input.
		/// </summary>
		internal static void Update()
		{
			if (!FP.Screen.IsOpen())
			{
				return;
			}
			
			keys.Cycle();
			mouseButtons.Cycle();
			
			_mouseWheelDelta = 0;
			
			var convertedCoords = FP.Screen.MapPixelToCoords(Mouse.GetPosition(FP.Screen));
			_mousePos.X = convertedCoords.X;
			_mousePos.Y = convertedCoords.Y;
			
			if (firstJoystickCheck > 0)
			{
				firstJoystickCheck--;
				if (firstJoystickCheck == 0)
				{
					for (uint i = 0; i < Joystick.Count; ++i)
					{
						if (Joystick.IsConnected(i))
						{
							var e = new JoystickConnectEvent();
							e.JoystickId = i;
							
							ControllerConnected(FP.Screen, new JoystickConnectEventArgs(e));
						}
					}
				}
			}
			
			Joystick.Update();
			Controller.UpdateAll();
			
			//	TODO:	cursor style
		}
		
		private static int firstJoystickCheck;
		
		private static InputStates<Keyboard.Key> keys;
		private static InputStates<Mouse.Button> mouseButtons;
		private static Dictionary<string, InputList> _control;
		
		private static int _mouseWheelDelta = 0;
		private static Vector2f _mousePos;
		
		private const int MAX_KEYSTRING = 100;
	}
}
