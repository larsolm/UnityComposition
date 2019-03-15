using System;
using System.Collections.Generic;
using System.Reflection;

namespace PiRhoSoft.CompositionEngine
{
	public class PropertyMap
	{
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
					var property = Property.Create(ownerType, info, mapping.Readable, mapping.Writable);
					Properties.Add(property);
				}
			}

			foreach (var info in properties)
			{
				var mapping = info.GetCustomAttribute<MappedVariableAttribute>();

				if (mapping != null)
				{
					var property = Property.Create(ownerType, info, mapping.Readable, mapping.Writable);
					Properties.Add(property);
				}
			}
		}
	}
}
