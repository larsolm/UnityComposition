using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SerializedVariableField : BindableElement
	{
		public SerializedVariable Value { get; private set; }

		private readonly SerializedProperty _property;
		private readonly VariableControl _control;

		public SerializedVariableField(SerializedProperty property, SerializedProperty definitionProperty) : this(property, definitionProperty.GetObject<VariableDefinition>())
		{
			var typeProperty = definitionProperty.FindPropertyRelative(VariableDefinition.TypeProperty);
			var dataProperty = definitionProperty
				.FindPropertyRelative(VariableDefinition.ConstraintProperty)
				.FindPropertyRelative(SerializedDataItem.ContentProperty);

			var dataWatcher = new ChangeTriggerControl<string>(dataProperty, (oldValue, newValue) => Refresh());
			var typeWatcher = new ChangeTriggerControl<Enum>(typeProperty, (oldValue, newValue) => Refresh());

			Add(dataWatcher);
			Add(typeWatcher);
		}

		public SerializedVariableField(SerializedProperty property, VariableDefinition definition)
		{
			bindingPath = property.propertyPath;
			Value = property.GetObject<SerializedVariable>();
			_property = property;

			_control = new VariableControl(Value.Variable, definition, property.serializedObject.targetObject);
			_control.RegisterCallback<ChangeEvent<Variable>>(evt => SetValue(evt.newValue));

			var dataProperty = property
				.FindPropertyRelative(SerializedVariable.DataProperty)
				.FindPropertyRelative(SerializedDataItem.ContentProperty);

			var dataWatcher = new ChangeTriggerControl<string>(dataProperty, (oldValue, newValue) => Refresh());

			Add(_control);
			Add(dataWatcher);
		}

		public void SetValue(Variable value)
		{
			Value.Variable = value;
			((ISerializationCallbackReceiver)Value).OnBeforeSerialize();
			_property.serializedObject.Update();
		}

		public void Refresh()
		{
			_control.SetValueWithoutNotify(Value.Variable);
		}
	}
}
