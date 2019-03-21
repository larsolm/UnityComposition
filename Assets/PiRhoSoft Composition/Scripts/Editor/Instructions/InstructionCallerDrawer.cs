using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class InstructionCallerControl : ObjectControl<InstructionCaller>
	{
		private readonly static IconButton _refreshButton = new IconButton(IconButton.Refresh, "Refresh the list of inputs and outputs");
		private readonly static Label _inputsLabel = new Label(typeof(InstructionCaller), "_inputs");
		private readonly static Label _outputsLabel = new Label(typeof(InstructionCaller), "_outputs");
		private readonly static GUIContent[] _inputTypeOptions = new GUIContent[] { new GUIContent("Value/Boolean"), new GUIContent("Value/Integer"), new GUIContent("Value/Number"), new GUIContent("Value/String"), new GUIContent("Value/Object"), new GUIContent(), new GUIContent("Reference") };

		private InstructionCaller _caller;
		private ObjectListControl _inputs = new ObjectListControl();
		private ObjectListControl _outputs = new ObjectListControl();

		public override void Setup(InstructionCaller target, SerializedProperty property, FieldInfo fieldInfo, PropertyAttribute attribute)
		{
			_caller = target;

			Refresh();

			_inputs.MakeDrawable(DrawInput).MakeHeaderButton(_refreshButton, (rect) => Refresh(), Color.black);
			_outputs.MakeDrawable(DrawOutput).MakeHeaderButton(_refreshButton, (rect) => Refresh(), Color.black);
		}

		private void Refresh()
		{
			_caller.UpdateVariables();
			_inputs.Setup(_caller.Inputs as IList);
			_outputs.Setup(_caller.Outputs as IList);
		}

		private void DrawInstruction(Rect rect, GUIContent label)
		{
			var instruction = InstructionDrawer.Draw(rect, label, _caller.Instruction);
			if (_caller.Instruction != instruction)
			{
				_caller.Instruction = instruction;
				Refresh();
			}
		}

		private void DrawInput(Rect rect, IList list, int index)
		{
			var labelWidth = rect.width * 0.25f;
			var typeWidth = rect.width * 0.25f;

			var input = _caller.Inputs[index];
			var labelRect = RectHelper.TakeWidth(ref rect, labelWidth);
			var typeRect = RectHelper.TakeWidth(ref rect, typeWidth);
			RectHelper.TakeHorizontalSpace(ref rect);

			EditorGUI.LabelField(labelRect, input.Definition.Name);

			if (input.Definition.Type == VariableType.Empty)
			{
				var typeIndex = input.Type == InstructionInputType.Reference ? 6 : GetIndexForType(input.Value.Type);
				var newIndex = EditorGUI.Popup(typeRect, typeIndex, _inputTypeOptions);

				if (newIndex != typeIndex)
				{
					input.Type = newIndex == 6 ? InstructionInputType.Reference : InstructionInputType.Value;
					input.Value = VariableValue.Create(GetTypeFromIndex(newIndex));
				}
			}
			else
			{
				input.Type = (InstructionInputType)EditorGUI.EnumPopup(typeRect, input.Type);
			}

			switch (input.Type)
			{
				case InstructionInputType.Reference: VariableReferenceControl.Draw(rect, input.Reference, GUIContent.none); break;
				case InstructionInputType.Value: input.Value = VariableValueDrawer.Draw(rect, GUIContent.none, input.Value, input.Definition); break;
			}
		}

		private int GetIndexForType(VariableType type)
		{
			return (int)type - 1;
		}

		private VariableType GetTypeFromIndex(int index)
		{
			return (VariableType)(index + 1);
		}

		private void DrawOutput(Rect rect, IList list, int index)
		{
			var labelWidth = rect.width * 0.25f;
			var typeWidth = rect.width * 0.25f;

			var output = _caller.Outputs[index];
			var labelRect = RectHelper.TakeWidth(ref rect, labelWidth);
			var typeRect = RectHelper.TakeWidth(ref rect, typeWidth);
			RectHelper.TakeHorizontalSpace(ref rect);

			EditorGUI.LabelField(labelRect, output.Definition.Name);
			output.Type = (InstructionOutputType)EditorGUI.EnumPopup(typeRect, output.Type);

			switch (output.Type)
			{
				case InstructionOutputType.Ignore: break;
				case InstructionOutputType.Reference: VariableReferenceControl.Draw(rect, output.Reference, GUIContent.none); break;
			}
		}

		public override float GetHeight(GUIContent label)
		{
			var height = InstructionDrawer.GetHeight();

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
			var instructionHeight = InstructionDrawer.GetHeight();
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
	public class InstructionCallerDrawer : ControlDrawer<InstructionCallerControl>
	{
	}
}
