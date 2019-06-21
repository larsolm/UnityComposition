using System;
using System.Collections.Generic;
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

		public TypePicker() { }
		public TypePicker(SerializedProperty property) : base(property) { }
		public TypePicker(Object owner, Func<string> getValue, Action<string> setValue) : base(owner, getValue, setValue) { }

		public void Setup(Type type, bool showAbstract, string value)
		{
			if (type == null)
			{
				Debug.LogWarningFormat(_invalidTypeWarning, type);
				return;
			}

			_type = type;

			var initialType = Type.GetType(value);
			var picker = new Picker();
			picker.Setup(_type, showAbstract, initialType);
			picker.OnSelected += selectedValue =>
			{
				Value = selectedValue.AssemblyQualifiedName;
				ElementHelper.SendChangeEvent(this, value, Value);
			};

			Setup(picker, value);
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
			var type = Type.GetType(value);
			var text = type == null ? $"None ({_type.Name})" : type.Name;
			var icon = AssetPreview.GetMiniTypeThumbnail(type);

			SetLabel(icon, text);
		}

		#region UXML

		private class Factory : UxmlFactory<Editor.TypePicker, Traits> { }

		private class Traits : UxmlTraits
		{

			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };
			private UxmlBoolAttributeDescription _showAbstract = new UxmlBoolAttributeDescription { name = "show-abstract", defaultValue = true };

			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get { yield break; }
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);

				var button = ve as Editor.TypePicker;
				var typeName = _type.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName);
				var showAbstract = _showAbstract.GetValueFromBag(bag, cc);
				var value = _value.GetValueFromBag(bag, cc);

				if (type != null)
					button.Setup(type, showAbstract, value);
				else
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
			}
		}

		#endregion
	}
}
