using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class TypePicker : BasePickerButton<string>
	{
		private class Picker : BasePicker<Type>
		{
			public void Setup(Type type, bool showAbstract, Type initialType)
			{
				var types = TypeHelper.GetTypeList(type, showAbstract);
				CreateTree(types.BaseType.Name, types.Paths, types.Types, initialType, iconType => AssetPreview.GetMiniTypeThumbnail(iconType));
			}
		}

		private const string _invalidTypeWarning = "(PUCTPIT) Invalid type for TypePicker: the type '{0}' could not be found";

		private Type _type;

		public TypePicker(SerializedProperty property) : base(property) { }
		public TypePicker(Object owner, Func<string> getValue, Action<string> setValue) : base(owner, getValue, setValue) { }

		public void Setup(Type type, bool showAbstract, Type value)
		{
			if (type == null)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			_type = type;

			var valueName = value?.AssemblyQualifiedName;
			var initialType = Type.GetType(valueName ?? string.Empty);
			var picker = new Picker();
			picker.Setup(_type, showAbstract, initialType);
			picker.OnSelected += selectedValue =>
			{
				Value = selectedValue?.AssemblyQualifiedName;
				UpdateElement(Value);
				ElementHelper.SendChangeEvent(this, valueName, Value);
			};

			Setup(picker, valueName);
		}

		public override string GetValueFromProperty(SerializedProperty property)
		{
			return property.stringValue;
		}

		public override void UpdateProperty(string value, VisualElement element, SerializedProperty property)
		{
			property.stringValue = value;
		}

		protected override void UpdateElement(string value)
		{
			var type = Type.GetType(value ?? string.Empty);
			var text = type == null ? $"None ({_type.Name})" : type.Name;
			var icon = AssetPreview.GetMiniTypeThumbnail(type);

			SetLabel(icon, text);
		}
	}
}
