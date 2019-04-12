using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionCallerControl : ObjectControl<InstructionCaller>
	{
		private readonly static Label _refreshButton = new Label(Icon.BuiltIn(Icon.Refresh), "", "Refresh the list of inputs and outputs");
		private readonly static Label _inputsLabel = new Label(typeof(InstructionCaller), "_inputs");
		private readonly static Label _outputsLabel = new Label(typeof(InstructionCaller), "_outputs");

		private InstructionCaller _caller;
		private ObjectListControl _inputs = new ObjectListControl();
		private ObjectListControl _outputs = new ObjectListControl();

		public override void Setup(InstructionCaller target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_caller = target;

			Refresh();

			_inputs
				.MakeDrawable(DrawInput)
				.MakeCustomHeight(GetInputHeight)
				.MakeHeaderButton(_refreshButton, (rect) => Refresh(), Color.black);

			_outputs
				.MakeDrawable(DrawOutput)
				.MakeCustomHeight(GetOutputHeight)
				.MakeHeaderButton(_refreshButton, (rect) => Refresh(), Color.black);
		}

		private void Refresh()
		{
			_caller.UpdateVariables();
			_inputs.Setup(_caller.Inputs as IList);
			_outputs.Setup(_caller.Outputs as IList);
		}

		private void DrawInstruction(Rect rect, GUIContent label)
		{
			var instruction = AssetDisplayDrawer.Draw(rect, label, _caller.Instruction, true, true, AssetLocation.Selectable, null);

			if (_caller.Instruction != instruction)
			{
				_caller.Instruction = instruction;
				Refresh();
			}
		}

		private float GetInputHeight(int index)
		{
			var input = _caller.Inputs[index];
			var definition = _caller.GetInputDefinition(input);

			switch (input.Type)
			{
				case InstructionInputType.Reference: return VariableReferenceControl.GetHeight();
				case InstructionInputType.Value: return VariableValueDrawer.GetHeight(input.Value, definition.Definition, true);
				default: return EditorGUIUtility.singleLineHeight;
			}
		}

		private void DrawInput(Rect rect, IList list, int index)
		{
			var labelWidth = rect.width * 0.25f;
			var typeWidth = rect.width * 0.25f;

			var input = _caller.Inputs[index];
			var definition = _caller.GetInputDefinition(input);
			var labelRect = RectHelper.TakeWidth(ref rect, labelWidth);
			var typeRect = RectHelper.TakeWidth(ref rect, typeWidth);
			RectHelper.TakeHorizontalSpace(ref rect);

			EditorGUI.LabelField(labelRect, definition.Name);

			if (definition.Definition.Type != VariableType.Empty)
			{
				input.Type = (InstructionInputType)EditorGUI.EnumPopup(typeRect, input.Type);

				switch (input.Type)
				{
					case InstructionInputType.Reference: VariableReferenceControl.Draw(rect, input.Reference, GUIContent.none); break;
					case InstructionInputType.Value: input.Value = VariableValueDrawer.Draw(rect, GUIContent.none, input.Value, definition.Definition, true); break;
				}
			}
		}

		private float GetOutputHeight(int index)
		{
			var output = _caller.Outputs[index];
			var definition = _caller.GetOutputDefinition(output);

			switch (output.Type)
			{
				case InstructionOutputType.Reference: return VariableReferenceControl.GetHeight();
				default: return EditorGUIUtility.singleLineHeight;
			}
		}

		private void DrawOutput(Rect rect, IList list, int index)
		{
			var labelWidth = rect.width * 0.25f;
			var typeWidth = rect.width * 0.25f;

			var output = _caller.Outputs[index];
			var labelRect = RectHelper.TakeWidth(ref rect, labelWidth);
			var typeRect = RectHelper.TakeWidth(ref rect, typeWidth);
			RectHelper.TakeHorizontalSpace(ref rect);

			EditorGUI.LabelField(labelRect, output.Name);
			output.Type = (InstructionOutputType)EditorGUI.EnumPopup(typeRect, output.Type);

			switch (output.Type)
			{
				case InstructionOutputType.Ignore: break;
				case InstructionOutputType.Reference: VariableReferenceControl.Draw(rect, output.Reference, GUIContent.none); break;
			}
		}

		public override float GetHeight(GUIContent label)
		{
			var height = AssetDisplayDrawer.GetHeight(label);

			if (_caller.Inputs.Count > 0 || _caller.Outputs.Count > 0)
				height += RectHelper.VerticalSpace;

			if (_caller.Inputs.Count > 0)
				height += _inputs.GetHeight();

			if (_caller.Outputs.Count > 0)
				height += _outputs.GetHeight();

			return height;
		}

		public override void Draw(Rect position, GUIContent label)
		{
			var instructionHeight = AssetDisplayDrawer.GetHeight(label);
			var instructionRect = RectHelper.TakeHeight(ref position, instructionHeight);

			DrawInstruction(instructionRect, label);
			RectHelper.TakeVerticalSpace(ref position);

			using (new EditorGUI.IndentLevelScope())
			{
				if (_caller.Inputs.Count > 0)
				{
					var inputsHeight = _inputs.GetHeight();
					var inputsRect = RectHelper.TakeHeight(ref position, inputsHeight);

					_inputs.Draw(inputsRect, _inputsLabel.Content);
				}

				if (_caller.Outputs.Count > 0)
				{
					var outputsHeight = _outputs.GetHeight();
					var outputsRect = RectHelper.TakeHeight(ref position, outputsHeight);

					_outputs.Draw(outputsRect, _outputsLabel.Content);
				}
			}
		}
	}

	[CustomPropertyDrawer(typeof(InstructionCaller))]
	public class InstructionCallerDrawer : PropertyDrawer<InstructionCallerControl>
	{
	}
}
