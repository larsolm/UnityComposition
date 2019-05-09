using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class PropertyMap
	{
		private const string _invalidFieldError = "(CPMIF) failed to map field '{0}' on type {1}: {2} is not a supported type";
		private const string _invalidPropertyError = "(CPMIP) failed to map property '{0}' on type {1}: {2} is not a supported type";

		public List<Property> Properties = new List<Property>();

		public PropertyMap(Type ownerType)
		{
			var fields = ownerType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			var properties = ownerType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (var info in fields)
			{
				var mapping = info.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
				{
					var property = Property.Create(info, !mapping.ReadOnly);

					if (property != null)
						Properties.Add(property);
					else
						Debug.LogWarningFormat(_invalidFieldError, info.Name, ownerType.Name, info.FieldType.Name);
				}
			}

			foreach (var info in properties)
			{
				var mapping = info.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
				{
					var property = Property.Create(info, !mapping.ReadOnly);

					if (property != null)
						Properties.Add(property);
					else
						Debug.LogWarningFormat(_invalidPropertyError, info.Name, ownerType.Name, info.PropertyType.Name);
				}
			}
		}
	}
}
