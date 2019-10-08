using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.Composition.Editor
{
	public class AutocompleteAttribute : Attribute
	{
		public Type Type { get; }
		public AutocompleteAttribute(Type type) => Type = type;
	}

	public interface IAutocompleteItem
	{
		string Name { get; }

		bool AllowsCustomFields { get; }
		bool IsCastable { get; }
		bool IsIndexable { get; }

		IAutocompleteItem GetField(string name);
		IAutocompleteItem GetTypeField(string type);
		IAutocompleteItem GetIndexField();
		IEnumerable<IAutocompleteItem> GetFields();
		IEnumerable<Type> GetTypes();
	}

	public abstract class AutocompleteItem : IAutocompleteItem
	{
		public string Name { get; set; }

		public bool AllowsCustomFields { get; protected set; }
		public bool IsCastable { get; protected set; }
		public bool IsIndexable { get; protected set; }

		public IAutocompleteItem IndexField { get; protected set; }
		public List<IAutocompleteItem> Fields { get; protected set; }
		public List<Type> Types { get; protected set; }

		public IAutocompleteItem GetField(string name) => string.IsNullOrEmpty(name) ? null : Fields?.Where(field => field.Name == name).FirstOrDefault();
		public IAutocompleteItem GetTypeField(string name) => Autocomplete.GetItem(string.IsNullOrEmpty(name) ? null : Types?.Where(type => type.Name == name).FirstOrDefault());
		public IAutocompleteItem GetIndexField() => IndexField;
		public IEnumerable<IAutocompleteItem> GetFields() => Fields ?? Enumerable.Empty<IAutocompleteItem>();
		public IEnumerable<Type> GetTypes() => Types ?? Enumerable.Empty<Type>();

		public abstract void Setup(object obj);

		#region Helpers

		protected List<Type> GetCastTypes(object obj)
		{
			if (obj is Component component)
				return GetCastTypes(component.gameObject, component.GetType());
			else if (obj is GameObject gameObject)
				return GetCastTypes(gameObject, null);

			return null;
		}

		private List<Type> GetCastTypes(GameObject obj, Type except)
		{
			var types = new List<Type>();
			var components = obj.GetComponents<Component>();

			foreach (var component in components)
			{
				var type = component.GetType();

				if (type != except && !types.Contains(type))
					types.Add(type);
			}

			return types;
		}

		#endregion
	}

	public abstract class AutocompleteItem<T> : AutocompleteItem where T : class
	{
		public override void Setup(object obj) => Setup(obj as T);
		protected abstract void Setup(T obj);
	}

	public class LeafAutocompleteItem : IAutocompleteItem
	{
		public string Name { get; private set; }

		public bool AllowsCustomFields => false;
		public bool IsCastable => false;
		public bool IsIndexable => false;

		public IAutocompleteItem GetField(string name) => null;
		public IAutocompleteItem GetTypeField(string name) => null;
		public IAutocompleteItem GetIndexField() => null;
		public IEnumerable<IAutocompleteItem> GetFields() => Enumerable.Empty<IAutocompleteItem>();
		public IEnumerable<Type> GetTypes() => Enumerable.Empty<Type>();

		public LeafAutocompleteItem(string name) => Name = name;
	}
}
