using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SerializedVariableListField : BindableElement
	{
		public static readonly string UssClassName = "pirho-serialized-variable-list-field";

		private VariableListControl _control;

		public void Setup(SerializedProperty property, SerializedVariableListProxy proxy)
		{
			bindingPath = property.propertyPath;
			Setup(proxy);
		}

		public void Setup(VariableListProxy proxy)
		{
			Clear();

			_control = new VariableListControl(proxy);

			Add(_control);
			AddToClassList(UssClassName);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
			{
				var list = property
					.FindPropertyRelative(SerializedVariableList.DataProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty)
					.FindPropertyRelative("Array.size");

				var sizeBinding = new ChangeTriggerControl<int>(null, (oldSize, size) => _control.Refresh());
				sizeBinding.Watch(list);
			}
		}
	}

	public class SerializedVariableListProxy : VariableListProxy
	{
		private const string _invalidBindingError = "(PCSVLPIB) invalid binding '{0}' for SerializedVariableListField: property '{1}' is not a SerializedVariableList";

		public SerializedProperty Property { get; private set; }

		public SerializedVariableListProxy(SerializedProperty property)
		{
			var variables = property.GetObject<SerializedVariableList>();

			if (variables != null)
			{
				var owner = property.serializedObject.targetObject;

				Property = property
					.FindPropertyRelative(SerializedVariableList.DataProperty)
					.FindPropertyRelative(SerializedDataList.ContentProperty);

				Setup(variables, owner);
			}
			else
			{
				Debug.LogErrorFormat(_invalidBindingError, property.propertyPath, property.propertyPath, property.propertyType);
			}
		}

		public override VisualElement CreateElement(int index)
		{
			return new VariableField(Property, Variables, index, null) { userData = index };
		}

		private class VariableField : VariableControl
		{
			private IVariableList _variables;
			private int _index;
			
			public VariableField(SerializedProperty property, IVariableList variables, int index, VariableDefinition definition) : base(variables.GetVariable(index), definition, property.serializedObject.targetObject)
			{
				_variables = variables;
				_index = index;

				RegisterCallback<ChangeEvent<Variable>>(evt => _variables.SetVariable(_index, evt.newValue));

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
