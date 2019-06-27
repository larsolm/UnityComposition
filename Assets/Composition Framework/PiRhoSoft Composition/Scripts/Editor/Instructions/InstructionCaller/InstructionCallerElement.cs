using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionCallerElement : VisualElement
	{
		private Object _owner;
		private InstructionCaller _caller;

		private ListElement _inputs;
		private ListElement _outputs;

		public InstructionCallerElement(SerializedProperty property)
		{
			_owner = property.serializedObject.targetObject;
			_caller = PropertyHelper.GetObject<InstructionCaller>(property);

			var picker = new ObjectPicker(property.FindPropertyRelative(nameof(InstructionCaller.Instruction)));
			picker.Setup(typeof(Instruction), _caller.Instruction);
			picker.RegisterCallback<ChangeEvent<Object>>(e => UpdateVariables());

			Add(picker);

			UpdateVariables();
		}

		private void UpdateVariables()
		{
			_caller.UpdateVariables();

			if (_inputs != null)
			{
				Remove(_inputs);
				_inputs = null;
			}

			if (_outputs != null)
			{
				Remove(_outputs);
				_outputs = null;
			}

			if (_caller.Inputs.Count > 0)
			{
				var proxy = new ListProxy<InstructionInput>(_caller.Inputs, CreateInput);
				_inputs = new ListElement(proxy, "Inputs", ElementHelper.GetTooltip(typeof(InstructionCaller), "_inputs"), false, false, false);
				Add(_inputs);
			}

			if (_caller.Outputs.Count == 0)
			{
				var proxy = new ListProxy<InstructionOutput>(_caller.Outputs, CreateOutput);
				_outputs = new ListElement(proxy, "Outputs", ElementHelper.GetTooltip(typeof(InstructionCaller), "_outputs"), false, false, false);
				Add(_outputs);
			}
		}

		private VisualElement CreateInput(InstructionInput input, int index)
		{
			var definition = _caller.GetInputDefinition(input);
			var container = new VisualElement();
			var label = new Label(input.Name);

			var typeElement = new EnumDropdown<InstructionInputType>(input.Type, _owner, () => (int)input.Type, value => input.Type = (InstructionInputType)value);

			var referenceElement = new VariableReferenceElement();
			referenceElement.Setup(_owner, input.Reference, null); // TODO: Add an autocompletesource

			var valueElement = new VariableValueElement(_owner, () => input.Value, value => input.Value = value, () => _caller.GetInputDefinition(input).Definition );

			typeElement.RegisterCallback<ChangeEvent<int>>(e =>
			{
				var t = (InstructionInputType)e.newValue;

				ElementHelper.SetVisible(referenceElement, t == InstructionInputType.Reference);
				ElementHelper.SetVisible(valueElement, t == InstructionInputType.Value);
			});
			
			container.Add(label);
			container.Add(typeElement);
			container.Add(referenceElement);
			container.Add(valueElement);

			return container;
		}

		private VisualElement CreateOutput(InstructionOutput output, int index)
		{
			var definition = _caller.GetOutputDefinition(output);
			var container = new VisualElement();
			var label = new Label(definition.Name);

			var type = new EnumDropdown<InstructionOutputType>(output.Type, _owner, () => (int)output.Type, value => output.Type = (InstructionOutputType)value);

			var reference = new VariableReferenceElement();
			reference.Setup(_owner, output.Reference, null); // TODO: Add an autocompletesource

			type.RegisterCallback<ChangeEvent<int>>(e => ElementHelper.SetVisible(reference, (InstructionOutputType)e.newValue == InstructionOutputType.Reference));

			container.Add(label);
			container.Add(type);
			container.Add(reference);

			return container;
		}
	}
}
