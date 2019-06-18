using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionCallerElement : VisualElement
	{
		private InstructionCaller _caller;

		private ListElement _inputs;
		private ListElement _outputs;

		public void Setup(SerializedProperty property)
		{
			var caller = PropertyHelper.GetObject<InstructionCaller>(property);
		}

		public void Setup(InstructionCaller caller)
		{
			_caller = caller;
			_caller.UpdateVariables();

			var picker = new ObjectPickerButton();
			picker.Setup(typeof(Instruction), _caller.Instruction);

			_inputs = new ListElement { AllowAdd = false, AllowRemove = false, AllowMove = false };
			_inputs.Setup("Inputs", new IListListProxy<InstructionInput>(_caller.Inputs, CreateInput));

			_outputs = new ListElement() { AllowAdd = false, AllowRemove = false, AllowMove = false };
			_outputs.Setup("Outputs", new IListListProxy<InstructionOutput>(_caller.Outputs, CreateOutput));

			Add(picker);
			Add(_inputs);
			Add(_outputs);

			if (_caller.Inputs.Count == 0)
				ElementHelper.SetVisible(_inputs, false);

			if (_caller.Outputs.Count == 0)
				ElementHelper.SetVisible(_outputs, false);
		}

		private VisualElement CreateInput(InstructionInput input)
		{
			var definition = _caller.GetInputDefinition(input);
			var container = new VisualElement();
			var label = new Label(definition.Name);

			//input.Type = (InstructionInputType)EditorGUI.EnumPopup(typeRect, input.Type);

			//switch (input.Type)
			//{
			//	case InstructionInputType.Reference: VariableReferenceControl.Draw(rect, input.Reference, GUIContent.none); break;
			//	case InstructionInputType.Value: input.Value = VariableValueDrawer.Draw(rect, GUIContent.none, input.Value, definition.Definition, true); break;
			//}

			container.Add(label);

			return container;
		}

		private VisualElement CreateOutput(InstructionOutput output)
		{
			var definition = _caller.GetOutputDefinition(output);
			var container = new VisualElement();
			var label = new Label(definition.Name);

			//output.Type = (InstructionOutputType)EditorGUI.EnumPopup(typeRect, output.Type);
			//
			//switch (output.Type)
			//{
			//	case InstructionOutputType.Ignore: break;
			//	case InstructionOutputType.Reference: VariableReferenceControl.Draw(rect, output.Reference, GUIContent.none); break;
			//}

			container.Add(label);

			return container;
		}
	}
}
