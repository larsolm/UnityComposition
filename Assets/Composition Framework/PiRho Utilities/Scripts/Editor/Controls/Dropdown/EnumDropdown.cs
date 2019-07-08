using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumDropdown<T> : EnumDropdown where T : Enum
	{
		public EnumDropdown(T value, SerializedProperty property) : base(typeof(T), GetValue(value), property) { }
		public EnumDropdown(T value, Object owner, Func<int> getValue, Action<int> setValue) : base(typeof(T), GetValue(value), owner, getValue, setValue) { }

		private static int GetValue(T value)
		{
			return (int)Enum.Parse(typeof(T), value.ToString());
		}
	}

	public class EnumDropdown : Dropdown<int>
	{
		private const string _invalidTypeWarning = "(PUCEDIT) Invalid type for EnumButtons: the type '{0}' must be an Enum type";

		public EnumDropdown(Type type, int value, SerializedProperty property) : base(GetNames(type), GetValues(type), value, property) { }
		public EnumDropdown(Type type, int value, Object owner, Func<int> getValue, Action<int> setValue) : base(GetNames(type), GetValues(type), value, owner, getValue, setValue) { }

		private static List<string> GetNames(Type type)
		{
			if (type == null || !type.IsEnum)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return new List<string>();
			}

			return Enum.GetNames(type).ToList();
		}

		private static List<int> GetValues(Type type)
		{
			if (type == null || !type.IsEnum)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return new List<int>();
			}

			return Enum.GetValues(type).Cast<int>().ToList();
		}

		public override int GetValueFromProperty(SerializedProperty property)
		{
			return property.intValue;
		}

		public override void UpdateProperty(int value, VisualElement element, SerializedProperty property)
		{
			property.intValue = value;
		}
	}
}
