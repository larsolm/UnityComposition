using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionCallerElement : VisualElement
	{
		private const string _styleSheetPath = Composition.StylePath + "Instructions/InstructionCaller/InstructionCallerElement.uss";
		private const string _ussBaseClass = "pargon-instruction-caller";
		private const string _ussItemClass = "definition-item";
		private const string _ussLabelClass = "definition-label";
		private const string _ussDropdownClass = "definition-type";
		private const string _ussReferenceClass = "definition-reference";
		private const string _ussValueClass = "definition-value";

		private Object _owner;
		private InstructionCaller _caller;

		private ListElement _inputs;
		private ListElement _outputs;

		public InstructionCallerElement(SerializedProperty property, string tooltip)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);

			var container = ElementHelper.CreatePropertyContainer(property.displayName, tooltip);

			_owner = property.serializedObject.targetObject;
			_caller = PropertyHelper.GetObject<InstructionCaller>(property);

			var picker = new ObjectPicker(property.FindPropertyRelative("_instruction"));
			picker.Setup(typeof(Instruction), _caller.Instruction);
			picker.RegisterCallback<ChangeEvent<Object>>(e => UpdateVariables());

			Add(container);
			container.Add(picker);

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
				//var proxy = new ListProxy<InstructionInput>(_caller.Inputs, CreateInput);
				//_inputs = new ListElement(proxy, "Inputs", ElementHelper.GetTooltip(typeof(InstructionCaller), "_inputs"), false, false, false);
				//Add(_inputs);
			}

			if (_caller.Outputs.Count == 0)
			{
				//var proxy = new ListProxy<InstructionOutput>(_caller.Outputs, CreateOutput);
				//_outputs = new ListElement(proxy, "Outputs", ElementHelper.GetTooltip(typeof(InstructionCaller), "_outputs"), false, false, false);
				//Add(_outputs);
			}
		}

		private VisualElement CreateInput(InstructionInput input, int index)
		{
			var label = new Label(input.Name);
			label.AddToClassList(_ussLabelClass);

			var referenceElement = new VariableReferenceElement();
			referenceElement.Setup(_owner, input.Reference, null); // TODO: Add an autocompletesource
			referenceElement.AddToClassList(_ussReferenceClass);

			var valueElement = new VariableValueElement(_owner, () => input.Value, value => input.Value = value, () => _caller.GetInputDefinition(input).Definition);
			valueElement.AddToClassList(_ussValueClass);

			var typeElement = new EnumDropdown<InstructionInputType>(input.Type, _owner, () => (int)input.Type, value => input.Type = (InstructionInputType)value);
			typeElement.AddToClassList(_ussDropdownClass);
			typeElement.RegisterCallback<ChangeEvent<int>>(e =>
			{
				var t = (InstructionInputType)e.newValue;

				ElementHelper.SetVisible(referenceElement, t == InstructionInputType.Reference);
				ElementHelper.SetVisible(valueElement, t == InstructionInputType.Value);
			});

			ElementHelper.SetVisible(referenceElement, input.Type == InstructionInputType.Reference);
			ElementHelper.SetVisible(valueElement, input.Type == InstructionInputType.Value);

			var container = new VisualElement();
			container.AddToClassList(_ussItemClass);
			container.Add(label);
			container.Add(typeElement);
			container.Add(referenceElement);
			container.Add(valueElement);

			return container;
		}

		private VisualElement CreateOutput(InstructionOutput output, int index)
		{
			var label = new Label(output.Name);
			label.AddToClassList(_ussLabelClass);

			var reference = new VariableReferenceElement();
			reference.Setup(_owner, output.Reference, null); // TODO: Add an autocompletesource
			reference.AddToClassList(_ussReferenceClass);

			var type = new EnumDropdown<InstructionOutputType>(output.Type, _owner, () => (int)output.Type, value => output.Type = (InstructionOutputType)value);
			type.AddToClassList(_ussDropdownClass);
			type.RegisterCallback<ChangeEvent<int>>(e => ElementHelper.SetVisible(reference, (InstructionOutputType)e.newValue == InstructionOutputType.Reference));

			ElementHelper.SetVisible(reference, output.Type == InstructionOutputType.Reference);

			var container = new VisualElement();
			container.AddToClassList(_ussItemClass);
			container.Add(label);
			container.Add(type);
			container.Add(reference);

			return container;
		}
	}
}
