using PiRhoSoft.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	[InitializeOnLoad]
	public static class Autocomplete
	{
		private const string _invalidItemTypeWarning = "(AIITW) failed to register autocomplete type for object type '{0}': '{1}' is not instantiable as an IAutocompleteItem";
		private static Dictionary<Type, Type> _registry = new Dictionary<Type, Type>();

		static Autocomplete()
		{
			var types = TypeCache.GetTypesWithAttribute<AutocompleteAttribute>();

			foreach (var type in types)
			{
				var attribute = TypeHelper.GetAttribute<AutocompleteAttribute>(type);
				AddType(attribute.Type, type);
			}
		}

		public static void AddType(Type objectType, Type itemType)
		{
			if (itemType.IsCreatableAs<AutocompleteItem>())
				_registry.Add(objectType, itemType);
			else
				Debug.LogWarningFormat(_invalidItemTypeWarning, objectType.Name, itemType.Name);
		}

		public static AutocompleteItem GetItem(object obj)
		{
			var type = obj.GetType();

			if (_registry.TryGetValue(type, out var itemType))
			{
				var item = Activator.CreateInstance(itemType) as AutocompleteItem;
				item.Setup(obj);
				return item;
			}

			return new ObjectAutocompleteItem(obj);
		}
	}
}
