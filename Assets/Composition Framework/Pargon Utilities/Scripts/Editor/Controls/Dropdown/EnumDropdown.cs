using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumDropdown : Dropdown<int>
	{
		private const string _invalidTypeWarning = "(PICEDIT) Invalid type for EnumButtons: the type '{0}' must be an Enum type";
		private const string _invalidValueWarning = "(PICEDIV) Could not parse value of '{0}', because it isn't defined in the enum of type '{1}'";

		public EnumDropdown(SerializedProperty property) : base(property) { }
		public EnumDropdown(Object owner, Func<int> getValue, Action<int> setValue) : base(owner, getValue, setValue) { }

		public void Setup<T>(T value) where T : Enum
		{
			var type = typeof(T);
			var stringValue = value.ToString();

			if (!Enum.IsDefined(type, stringValue))
			{
				Debug.LogErrorFormat(_invalidValueWarning, stringValue, type.Name);
				return;
			}

			var v = (int)Enum.Parse(type, stringValue);
			var values = Enum.GetValues(type).Cast<int>().ToList();
			var options = Enum.GetNames(type).ToList();

			Setup(options, values, v);
		}

		public void Setup(Type type, int value)
		{
			if (type == null || !type.IsEnum)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			var values = Enum.GetValues(type).Cast<int>().ToList();
			var options = Enum.GetNames(type).ToList();

			Setup(options, values, value);
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
