using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SerializedVariableDictionaryField : BindableElement
	{
		public static readonly string UssClassName = "pirho-serialized-variable-dictionary-field";

		protected VariableDictionaryControl _control;

		public void Setup(VariableDictionaryProxy proxy)
		{
			Setup(new VariableDictionaryControl(proxy));
		}

		protected void Setup(VariableDictionaryControl control)
		{
			Clear();

			_control = control;

			Add(_control);
			AddToClassList(UssClassName);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var list = property
					.FindPropertyRelative(SerializedVariableDictionary.BindingProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty)
					.FindPropertyRelative("Array.size");

				var sizeBinding = new ChangeTriggerControl<int>(null, (oldSize, size) => _control.Refresh());
				sizeBinding.Watch(list);
			}
		}
	}

	public class SerializedVariableDictionaryProxy : VariableDictionaryProxy
	{
		private const string _invalidBindingError = "(PCSVDPIB) invalid binding '{0}' for SerializedVariableDictionaryField: property '{1}' is not a SerializedVariableDictionary";

		public SerializedProperty Property { get; private set; }

		public SerializedVariableDictionaryProxy(SerializedProperty property)
		{
			var variables = property.GetObject<SerializedVariableDictionary>();

			if (variables != null)
			{
				var owner = property.serializedObject.targetObject;

				Property = property
					.FindPropertyRelative(SerializedVariableDictionary.BindingProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty);

				Setup(variables, owner);
			}
			else
			{
				Debug.LogErrorFormat(_invalidBindingError, property.propertyPath, property.propertyPath, property.propertyType);
			}
		}

		public override VisualElement CreateField(int index)
		{
			return new VariableField(Property, Variables, index, null) { userData = index };
		}

		protected class VariableField : VariableControl
		{
			private IVariableMap _variables;
			private int _index;
			
			public VariableField(SerializedProperty property, IVariableMap variables, int index, VariableDefinition definition) : base(variables.GetVariable(variables.VariableNames[index]), definition, property.serializedObject.targetObject)
			{
				_variables = variables;
				_index = index;

				RegisterCallback<ChangeEvent<Variable>>(evt => _variables.SetVariable(variables.VariableNames[_index], evt.newValue));

				var data = property.GetParentObject<SerializedDataList>();
				var element = property.GetArrayElementAtIndex(_index);

				Add(new ChangeTriggerControl<string>(element, (oldValue, newValue) =>
				{
					using (var reader = new SerializedDataReader(data, index))
						SetValue(VariableHandler.Load(reader));
				}));
			}
		}
	}
}
