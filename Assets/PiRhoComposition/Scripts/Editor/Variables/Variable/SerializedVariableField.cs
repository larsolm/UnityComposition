using PiRhoSoft.Utilities;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class SerializedVariableField : BaseField<Variable>
	{
		private const string _invalidBindingError = "(PCSVFIB) invalid binding '{0}' for SerializedVariableListField: property '{1}' is not a SerializedVariableList";

		public static readonly string UssClassName = "pirho-serialized-variable-field";
		public static readonly string LabelUssClassName = UssClassName + "__label";
		public static readonly string InputUssClassName = UssClassName + "__input";

		public SerializedVariable Variable { get; private set; }
		public VariableDefinition Definition { get; private set; }

		private VariableControl _control;
		private ChangeTriggerControl<string> _binding;

		public SerializedVariableField(string label, VariableDefinition definition) : base(label, null)
		{
			Definition = definition;
		}

		public void Setup(SerializedProperty property)
		{
			var variable = property.GetObject<SerializedVariable>();

			if (variable != null)
			{
				Setup(variable, property.serializedObject.targetObject);

				var binding = property
					.FindPropertyRelative(SerializedVariable.BindingProperty)
					.FindPropertyRelative(SerializedDataItem.ContentProperty);

				var data = binding.GetParentObject<SerializedDataItem>();

				_binding = new ChangeTriggerControl<string>(binding, (oldValue, newValue) =>
				{
					using (var reader = new SerializedDataReader(data))
						base.value = VariableHandler.Load(reader);
				});
			}
			else
			{
				Debug.LogErrorFormat(_invalidBindingError, property.propertyPath, property.propertyPath, property.propertyType);
			}
		}

		public void Setup(SerializedVariable variable, Object owner)
		{
			Variable = variable;

			_control = new VariableControl(variable.Variable, Definition, owner);
			_control.AddToClassList(InputUssClassName);
			_control.RegisterCallback<ChangeEvent<Variable>>(evt => base.value = evt.newValue);

			labelElement.AddToClassList(LabelUssClassName);

			this.SetVisualInput(_control);
			AddToClassList(UssClassName);
			SetValueWithoutNotify(variable.Variable);
		}

		public override void SetValueWithoutNotify(Variable newValue)
		{
			base.SetValueWithoutNotify(newValue);

			if (Variable != null)
				Variable.Variable = newValue;

			_control.SetValueWithoutNotify(newValue);
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (this.TryGetPropertyBindEvent(evt, out var property))
				Setup(property);
		}
	}
}
