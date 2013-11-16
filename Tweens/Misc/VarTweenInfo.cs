/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 5/14/2013
 * Time: 3:49 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Fasterflect;

namespace Punk.Tweens.Misc
{
	/// <summary>
	/// Utility class that encapsulates getters and setters for VarTween properties.
	/// </summary>
	internal class VarTweenInfo
	{
		[Flags]
		public enum Options
		{
			Read,
			Write
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="obj">The object that the property belongs to</param>
		/// <param name="property">The name of the property</param>
		/// <param name="options">Optional options to determine whether the property must have a certain accessor. Use only if you know what you're doing. :X</param>
		public VarTweenInfo(object obj, string property, Options options = Options.Read | Options.Write)
		{
			_object = obj;
			_property = property;
			
			var type = _object.GetType();
			
			var field = type.Field(property);
			var prop = type.Property(property);
			
			if (field != null)	//	Using a field
			{
				_typeCode = Type.GetTypeCode(_object.GetFieldValue(_property).GetType());
				CheckTypeCode(property, _typeCode);
				
				isField = true;
			}
			else if (prop != null)	//	Using a property
			{
				//	Make sure we have both a getter and a setter
				if (!prop.CanRead && (options & Options.Read) > 0)
				{
					throw new Exception(string.Format("Property '{0}' on object of type {1} has no setter accessor.", prop, type.FullName));
				}
				
				if (!prop.CanWrite && (options & Options.Write) > 0)
				{
					throw new Exception(string.Format("Property '{0}' on object of type {1} has no getter accessor.", prop, type.FullName));
				}
				
				_typeCode = Type.GetTypeCode(_object.GetPropertyValue(_property).GetType());
				CheckTypeCode(property, _typeCode);
				
				isField = false;
			}
			else
			{
				//	Couldn't find either
				throw new Exception(string.Format("Field or property '{0}' not found on object of type {1}.", property, type.FullName));
			}
			
		}
		
		void CheckTypeCode(string property, TypeCode typeCode)
		{
			if (!(_typeCode == TypeCode.Int16 	||
		    _typeCode == TypeCode.Int32 		||
		    _typeCode == TypeCode.Int64 		||
		    _typeCode == TypeCode.UInt16 		||
		    _typeCode == TypeCode.UInt32		||
		    _typeCode == TypeCode.UInt64		||
		    _typeCode == TypeCode.Single		||
		    _typeCode == TypeCode.Double		))
			{
				throw new InvalidCastException(string.Format("Property or field to tween must be numeric.", property, _object));
			}
		}
		
		/// <summary>
		/// Get the property's value.
		/// </summary>
		public float Value
		{
			get
			{
				if (isField)
				{
					return Convert.ToSingle(_object.GetFieldValue(_property));
				}
				else
				{
					return Convert.ToSingle(_object.GetPropertyValue(_property));
				}
				
			}
			
			set
			{
				if (isField)
				{
					_object.SetFieldValue(_property, Convert.ChangeType(value, _typeCode));
				}
				else
				{
					_object.SetPropertyValue(_property, Convert.ChangeType(value, _typeCode));
				}
			}
		}
		
		private bool isField;
		private object _object;
		private string _property;
		private TypeCode _typeCode;
	}
}
