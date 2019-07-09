using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public class EnumButtonsField : BaseField<Enum>
	{
		private const string _invalidTypeWarning = "(PUEBEIT) Invalid property for EnumButtonsField: '{0}' is type '{1}' which is not an enum";

		public new static readonly string ussClassName = "pirho-enum-buttons-field";
		public new static readonly string labelUssClassName = ussClassName + "__label";
		public new static readonly string inputUssClassName = ussClassName + "__input";

		public EnumButtonsField(SerializedProperty property, string label, Type type, bool? useFlags = null) : base(label, null)
		{
			if (type.IsEnum)
			{
#if UNITY_2019_3_OR_NEWER
				bindingPath = property.propertyPath;
#else
				SetupCustomBinding(property, type);
#endif

				var value = Enum.ToObject(type, property.intValue) as Enum;
				var control = new EnumButtonsControl(value, useFlags);

				AddToClassList(ussClassName);
				labelElement.AddToClassList(labelUssClassName);
				control.AddToClassList(inputUssClassName);

				this.SetVisualInput(control);
				this.RegisterValueChangedCallback(evt => control.SetValueWithoutNotify(evt.newValue));
				control.RegisterCallback<ChangeEvent<Enum>>(evt => base.value = evt.newValue);
			}
			else
			{
				Debug.LogErrorFormat(property.propertyPath, type.Name);
			}
		}

#if !UNITY_2019_3_OR_NEWER

		// Unity 2019_2 does not support binding flag enums internally so this mess of reflection sets up the binding manually

		private void SetupCustomBinding(SerializedProperty property, Type type)
		{
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

#endif
	}
}