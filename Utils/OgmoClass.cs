/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 7/5/2013
 * Time: 5:42 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Fasterflect;

namespace Punk.Utils
{
	/// <summary>
	/// Description of OgmoClass.
	/// </summary>
	internal class OgmoClass
	{
		private string name;
		private Type type;
		
		internal OgmoClass(string name, Type type, string[] args = null)
		{
			if (!type.Inherits<Entity>())
				throw new InvalidCastException(string.Format("Error registering type {0} with Ogmo; type must extend Entity.", type.Name));
			
			this.name = name;
			this.type = type;
		}
		
		internal Entity CreateInstance(XmlNode entity)
		{
			//	Eventually support parameters
			Entity result = (Entity) type.CreateInstance();
			
			var flags = 
				BindingFlags.IgnoreCase |
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Instance |
				BindingFlags.Default;
			
			foreach (XmlAttribute element in entity.Attributes)
			{
				var prop = result.TryGetPropertyValue(element.Name, flags);
				var field = result.TryGetFieldValue(element.Name, flags);
				
				//	try to convert to a numeric type
				//	catch exceptions if casts aren't valid
				if (prop != null)
				{
					try {
						result.TrySetPropertyValue(element.Name, Convert.ChangeType(element.Value, Type.GetTypeCode(prop.GetType())), flags);
					} catch {}
					
				}
				else if (field != null)
				{
					try {
						result.TrySetFieldValue(element.Name, Convert.ChangeType(element.Value, Type.GetTypeCode(field.GetType())), flags);
					} catch {}
				}
				else
				{
					//	fall back to setting a string if it hasn't been set yet
					try {
						result.TrySetFieldValue(element.Name, element.Value, flags);
						result.TrySetPropertyValue(element.Name, element.Value, flags);
					} catch {}
					
					//	then try a bool
					bool value;
					if (bool.TryParse(element.Value, out value))
					{
						try {
							result.TrySetFieldValue(element.Name, value, flags);
							result.TrySetPropertyValue(element.Name, value, flags);
						} catch {}
					}
				}
			}
			
			result.Load(entity);
			return result;
		}
	}
}
