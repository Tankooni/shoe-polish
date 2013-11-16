/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 7/26/2013
 * Time: 8:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Punk.Utils
{
	/// <summary>
	/// A small utility class to allow messages to "return" values. Functionally similar to out and ref parameters.
	/// Pass a MessageResult instance to a message call and set the value from the message response.
	/// </summary>
	public class MessageResult
	{
		public MessageResult() {}
		
		/// <summary>
		/// The payload value.
		/// </summary>
		public Object Value;
	}
}
