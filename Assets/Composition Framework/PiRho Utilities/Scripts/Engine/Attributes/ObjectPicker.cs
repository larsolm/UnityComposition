using System;
using UnityEngine;

namespace PiRhoSoft.Utilities
{
	public class ObjectPickerAttribute : PropertyAttribute
	{
		public Type BaseType { get; private set; }

		public ObjectPickerAttribute()
		{
		}

		public ObjectPickerAttribute(Type baseType)
		{
			BaseType = baseType; 
		}
	}
}
