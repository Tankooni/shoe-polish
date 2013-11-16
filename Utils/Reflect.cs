/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/22/2013
 * Time: 4:09 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using Fasterflect;

namespace Punk.Utils.Reflect
{
	/// <summary>
	/// A simple class for basic reflection.
	/// </summary>
	public static class Reflect
	{
		/// <summary>
		/// Whether a property or field exists on an object.
		/// </summary>
		/// <param name="obj">The object that the property should exist on.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>Whether the property exists.</returns>
		public static bool HasOwnProperty(this object obj, string name)
		{
			var type = obj.GetType();
			
			return (type.Property(name) != null || type.Field(name) != null);
		}
		
		/// <summary>
		/// Get the value of a property or field with reflection.
		/// </summary>
		/// <param name="obj">The object that the property is on.</param>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value of the property.</returns>
		public static T GetProp<T>(this object obj, string name)
		{
			var type = obj.GetType();
			
			var prop = type.Property(name);
			var field = type.Field(name);
			
			if (prop != null)
			{
				Debug.Assert(prop.CanRead, "Property has no get accessor.");
				return (T) obj.GetPropertyValue(name);
			}
			else if (field != null)
			{
				return (T) obj.GetFieldValue(name);
			}
			
			throw new Exception(string.Format("Property {0} does not exist on {1}.", name, type.Name));
		}
	}
}
