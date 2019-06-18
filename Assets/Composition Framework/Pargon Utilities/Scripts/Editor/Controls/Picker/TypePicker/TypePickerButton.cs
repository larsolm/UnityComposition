using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class TypePickerButton : BasePickerButton<string>
	{
		private const string _invalidTypeWarning = "(PITPIT) Invalid type for TypePicker: the type '{0}' could not be found";

		private class Factory : UxmlFactory<TypePickerButton, Traits> { }

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

				var typePicker = ve as TypePickerButton;
				var typeName = _type.GetValueFromBag(bag, cc);
				var type = Type.GetType(typeName);
				var showAbstract = _showAbstract.GetValueFromBag(bag, cc);
				var value = _value.GetValueFromBag(bag, cc);

				if (typePicker.Type != null)
					typePicker.Setup(type, showAbstract, value);
				else
					Debug.LogWarningFormat(_invalidTypeWarning, typeName);
			}
		}

		public Type Type { get; private set; }
		public bool ShowAbstract { get; private set; }

		public void Setup(Type type, bool showAbstract, SerializedProperty property)
		{
			Setup(type, showAbstract, property.stringValue);
			BindToProperty(property);
		}

		public void Setup(Type type, bool showAbstract, string initialValue)
		{
			Type = type;
			ShowAbstract = showAbstract;
			
			var initialType = Type.GetType(initialValue);
			var picker = new TypePicker();
			picker.Setup(Type, ShowAbstract, initialType);
			picker.OnSelected += selectedValue => value = selectedValue.AssemblyQualifiedName;

			Setup(picker, initialValue);
		}

		#region BindableValueElement Implementation

		protected override void SetValueToProperty(SerializedProperty property, string value)
		{
			property.stringValue = value;
		}

		protected override string GetValueFromProperty(SerializedProperty property)
		{
			return property.stringValue;
		}

		protected override void Refresh()
		{
			var type = Type.GetType(value);
			var text = type == null ? $"None ({Type.Name})" : type.Name;
			var icon = AssetPreview.GetMiniTypeThumbnail(type);

			SetLabel(icon, text);
		}

		#endregion
	}
}
