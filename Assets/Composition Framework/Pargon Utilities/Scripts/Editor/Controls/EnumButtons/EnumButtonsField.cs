using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumButtonsField : BaseField<Enum>
	{
		private const string _invalidTypeError = "(PUEEBFIT) invalid property for EnumButtonsField: '{0}' is type '{1}' which is not an enum";

		public new static readonly string ussClassName = "pirho-enum-buttons-field";
		public new static readonly string labelUssClassName = ussClassName + "__label";
		public new static readonly string inputUssClassName = ussClassName + "__input";

		public EnumButtonsField(SerializedProperty property, string label, Type type, bool? useFlags = null) : base(label, null)
		{
			if (type.IsEnum)
			{
				var value = Enum.ToObject(type, property.intValue) as Enum;

				Setup(value, useFlags);
				SetupBinding(property, type);
			}
			else
			{
				Debug.LogErrorFormat(_invalidTypeError, property.propertyPath, type.Name);
			}
		}

		private void Setup(Enum value, bool? useFlags = null)
		{
			var control = new EnumButtonsControl(value, useFlags);

			AddToClassList(ussClassName);
			labelElement.AddToClassList(labelUssClassName);
			control.AddToClassList(inputUssClassName);

			this.SetVisualInput(control);
			this.RegisterValueChangedCallback(evt => control.SetValueWithoutNotify(evt.newValue));
			control.RegisterCallback<ChangeEvent<Enum>>(evt => base.value = evt.newValue);
		}

		private void SetupBinding(SerializedProperty property, Type type)
		{
			// ideally this could just be bindingPath = property.propertyPath but 2019.2 and earlier don't support
			// flags in enum bindings and 2019.3 only supports it for EnumFlagsField (BindingExtensions.CreateEnumBindingObject
			// checks specifically for EnumFlagsField or EnumField rather than BaseField<Enum>)

			var bindingExtensions = Type.GetType("UnityEditor.UIElements.BindingExtensions, UnityEditor");
			var defaultBindGeneric = bindingExtensions.GetMethod("DefaultBind", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var defaultBind = defaultBindGeneric.MakeGenericMethod(typeof(Enum));
			var serializedObjectUpdateWrapper = bindingExtensions.GetNestedType("SerializedObjectUpdateWrapper", System.Reflection.BindingFlags.NonPublic);

			var wrapper = Activator.CreateInstance(serializedObjectUpdateWrapper, property.serializedObject);

			Func<SerializedProperty, Enum> getter = p => Enum.ToObject(type, p.intValue) as Enum;
			Action<SerializedProperty, Enum> setter = (p, v) => p.intValue = (int)Enum.Parse(type, v.ToString());
			Func<Enum, SerializedProperty, Func<SerializedProperty, Enum>, bool> comparer = (v, p, g) => g(p).Equals(v);

			defaultBind.Invoke(null, new object[] { this, wrapper, property, getter, setter, comparer });
		}

		#region UXML Support

		public EnumButtonsField() : base(null, null) { }

		public new class UxmlFactory : UxmlFactory<EnumButtonsField, UxmlTraits> { }

		public new class UxmlTraits : BaseField<Enum>.UxmlTraits
		{
			private UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", use = UxmlAttributeDescription.Use.Required };
			private UxmlBoolAttributeDescription _flags = new UxmlBoolAttributeDescription { name = "flags" };
			private UxmlStringAttributeDescription _value = new UxmlStringAttributeDescription { name = "value" };

			public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(element, bag, cc);

				var field = element as EnumButtonsField;
				var typeName = _type.GetValueFromBag(bag, cc);

				var type = Type.GetType(typeName, false);

				if (type == null)
				{
					// error, missing type
				}
				else if (!type.IsEnum)
				{
					// error, invalid type
				}
				else
				{
					bool flags = true;
					var useFlags = _flags.TryGetValueFromBag(bag, cc, ref flags) ? (bool?)flags : null;
					var valueName = _value.GetValueFromBag(bag, cc);

					try
					{
						var value = string.IsNullOrEmpty(valueName)
							? Enum.ToObject(type, 0)
							: Enum.Parse(type, valueName);

						field.Setup(value as Enum, useFlags);

						// need to hijack binding by clearing bindingPath and calling SetupBinding from a SerializedObjectBindEvent which is internal
					}
					catch
					{
						// error: warning value, use default
					}
				}
			}
		}

		#endregion
	}
}