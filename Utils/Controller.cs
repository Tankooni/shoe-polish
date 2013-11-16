/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 5/22/2013
 * Time: 12:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using SFML.Window;

namespace Punk.Utils
{
	/// <summary>
	/// Description of Controller.
	/// </summary>
	public class Controller
	{
		internal static Dictionary<uint, Controller> controllers;
		internal static List<Controller> controllerList;
		
		static Controller()
		{
			controllers = new Dictionary<uint, Controller>();
			controllerList = new List<Controller>();
		}
		
		internal static void UpdateAll()
		{
			foreach (var controller in controllerList)
			{
				controller.Update();
			}
		}
		
		uint _id;
		private Dictionary<Button, bool> _down;
		private Dictionary<Button, bool> _pressed;
		private Dictionary<Button, bool> _released;
		private Dictionary<string, List<Button>> _control;
		private List<Button> _inputs;
		
		/// <summary>
		/// Value thumbstick must reach before registering it's position. Defaults to 10.0f.
		/// </summary>
		public float DeadZone = 20.0f;
		
		public Axis LeftStick;
		public Axis RightStick;
		public Axis DPad;
		
		public event EventHandler<JoystickConnectEventArgs> Connected;
		public event EventHandler<JoystickConnectEventArgs> Disconnected;
		
		/// <summary>
		/// Controller buttons!
		/// </summary>
		public enum Button
		{
			Unknown = -1,
			A = 0, B, X, Y,
			LB, RB,
			Back, Start,
			LS, RS
		};
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Id">Controller ID (zero-based)</param>
		public Controller(uint Id)
		{
			_id = Id;
			
			LeftStick = new Axis();
			RightStick = new Axis();
			DPad = new Axis();
			
			_down = new Dictionary<Button, bool>();
			_pressed = new Dictionary<Button, bool>();
			_released = new Dictionary<Button, bool>();
			_control = new Dictionary<string, List<Button>>();
			_inputs = new List<Button>();
			
			var buttons = Enum.GetValues(typeof(Button));
			
			foreach (Button button in buttons)
			{
				_down.Add(button, false);
				_pressed.Add(button, false);
				_released.Add(button, false);
				_inputs.Add(button);
			}
			
			AddController(this, Id);
			
			Connected = (s, e) => {};
			Disconnected = (s, e) => {};
		}
		
		void AddController(Controller controller, uint id)
		{
			controllers[id] = this;
			controllerList.Add(this);
		}
		
		internal void Update()
		{
			foreach (var input in _inputs)
			{
				_pressed[input] = false;
			}
			
			foreach (var input in _inputs)
			{
				_released[input] = false;
			}
			
			DPad.Update(Joystick.GetAxisPosition(_id, Joystick.Axis.PovY), -Joystick.GetAxisPosition(_id, Joystick.Axis.PovX), DeadZone, 1);
			LeftStick.Update(Joystick.GetAxisPosition(_id, Joystick.Axis.X), Joystick.GetAxisPosition(_id, Joystick.Axis.Y), DeadZone);
			RightStick.Update(Joystick.GetAxisPosition(_id, Joystick.Axis.U), Joystick.GetAxisPosition(_id, Joystick.Axis.R), DeadZone);
		}
		
		/// <summary>
		/// Defines a new input.
		/// </summary>
		/// <param name="name">String to map the input to.</param>
		/// <param name="controllerID">Controller ID.</param>
		/// <param name="inputs">The keys or buttons to use for the Input.</param>
		public void Define(string name, uint controllerID, params Button[] inputs)
		{
			if (_control.ContainsKey(name))
			{
				List<Button> current = _control[name];
				
				foreach (var input in inputs)
				{
					var cb = (Button) input;
					if (!current.Contains(cb))
					{
						current.Add(cb);
					}
				}
			}
			else
			{
				List<Button> newButtons = new List<Button>();
				
				foreach (var input in inputs)
				{
					var cb = (Button) input;
					if (!newButtons.Contains(cb))
					{
						newButtons.Add(cb);
					}
				}
				
				_control.Add(name, newButtons);
			}
		}
		
		/// <summary>
		///	If the input is held down.
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input or key is held down.</returns>
		public bool Check(string input)
		{
			if (!_control.ContainsKey(input))
			{
				return false;
			}
			
			foreach (var inputSource in _control[input])
			{
				if (_down[inputSource])
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
		public bool Check(Button input)
		{
			return _down[input];
		}
		
		/// <summary>
		///	If the input is held down.
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input or key is held down.</returns>
		public bool Down(string input)
		{
			return Check(input);
		}
		
		/// <summary>
		/// If the key is held down.
		/// </summary>
		/// <param name="input">The key to check.</param>
		/// <returns>If the key is held down.</returns>
		public bool Down(Button input)
		{
			return Check(input);
		}
		
		/// <summary>
		/// If the input was pressed this frame
		/// </summary>
		/// <param name="input">An input name, key or button to check for.</param>
		/// <returns>If the input, key or button was pressed this frame.</returns>
		public bool Pressed(string input)
		{
			if (!_control.ContainsKey(input))
			{
				return false;
			}
			
			foreach (var inputSource in _control[input])
			{
				if (_pressed[inputSource])
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
		public bool Pressed(Button input)
		{
			return _pressed[input];
		}
		
		/// <summary>
		/// If the input was released this frame.
		/// </summary>
		/// <param name="input">The input to check.</param>
		/// <returns>If the input was released this frame.</returns>
		public bool Released(string input)
		{
			if (!_control.ContainsKey(input))
			{
				return false;
			}
			
			foreach (var inputSource in _control[input])
			{
				if (_released[inputSource])
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
		public bool Released(Button input)
		{
			return _released[input];
		}
		
		/// <summary>
		/// Returns the inputs mapped to the input name.
		/// </summary>
		/// <param name="name">The input name.</param>
		/// <returns>A list of InputSources</returns>
		public List<Button> Inputs(string name)
		{
			if (_control.ContainsKey(name))
			{
				return _control[name];
			}
			
			return new List<Button>();
		}
		
		internal void OnButtonPressed(object s, JoystickButtonEventArgs e)
		{
			var cb = (Controller.Button)e.Button;
			
			if (!_down[cb])
			{
				_pressed[cb] = true;
				_down[cb] = true;
			}
		}
		
		internal void OnButtonReleased(object s, JoystickButtonEventArgs e)
		{
			var cb = (Button)e.Button;
			
			_released[cb] = true;
			_down[cb] = false;
		}
		
		internal void OnStickMove(object s, JoystickMoveEventArgs e)
		{
		}
		
//		public enum Axis
//		{
//X/Y: lstick (-100 - 100)
//R/U: rstick (-100 - 100)
//Z: lt (0 - 99)
//Z: rt (0 - -99)
//PovX: dpad up/down (-100 - 100)
//PovY: dpad l/r (-100 - 100)
//		}
		
		public static void OnJoystickReleased(object sender, JoystickButtonEventArgs e)
		{
			if (controllers.ContainsKey(e.JoystickId))
			{
				controllers[e.JoystickId].OnButtonReleased(sender, e);
			}
		}
		
		public static void OnJoystickPressed(object sender, JoystickButtonEventArgs e)
		{
			if (controllers.ContainsKey(e.JoystickId))
			{
				controllers[e.JoystickId].OnButtonPressed(sender, e);
			}
		}
		
		public static void OnJoystickMoved(object sender, JoystickMoveEventArgs e)
		{
			if (controllers.ContainsKey(e.JoystickId))
			{
				controllers[e.JoystickId].OnStickMove(sender, e);
			}
		}
		
		public static void OnConnected(object sender, JoystickConnectEventArgs e)
		{
			if (controllers.ContainsKey(e.JoystickId))
			{
				var controller = controllers[e.JoystickId];
				controller.Connected(sender, e);
			}
		}
		
		public static void OnDisconnected(object sender, JoystickConnectEventArgs e)
		{
			if (controllers.ContainsKey(e.JoystickId))
			{
				var controller = controllers[e.JoystickId];
				controller.Disconnected(sender, e);
			}
		}
	}
}
