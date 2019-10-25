using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerField : BindableElement
	{
		public const string Stylesheet = "Graph/GraphCaller/GraphCallerStyle.uss";
		public const string UssClassName = "pirho-graph-caller";
		public const string InputsUssClassName = UssClassName + "__inputs";
		public const string OutputsUssClassName = UssClassName + "__outputs";
		public const string NoInputsUssClassName = UssClassName + "--no-inputs";
		public const string NoOutputsUssClassName = UssClassName + "--no-outputs";
		public const string NameUssClassName = UssClassName + "__name";
		public const string ValueUssClassName = UssClassName + "__value";

		public GraphCallerField(SerializedProperty property)
		{
			var value = property.GetObject<GraphCaller>();
			value.UpdateVariables();
			property.serializedObject.Update();

			var graphProperty = property.FindPropertyRelative(GraphCaller.GraphField);
			var inputsProperty = property.FindPropertyRelative(GraphCaller.InputsField);
			var outputsProperty = property.FindPropertyRelative(GraphCaller.OutputsField);

			var inputsProxy = new InputsProxy(inputsProperty, value) { Label = "Inputs", Tooltip = "The input values to set for the Graph" };
			var outputsProxy = new OutputsProxy(outputsProperty, value) { Label = "Outputs", Tooltip = "The output values to resolve from this Graph" };
			var inputsList = new ListField();
			var outputsList = new ListField();

			inputsList.Setup(inputsProperty, inputsProxy);
			outputsList.Setup(outputsProperty, outputsProxy);

			var graphField = new ObjectPickerField(graphProperty, typeof(Graph));
			graphField.RegisterValueChangedCallback(evt =>
			{
				value.UpdateVariables();

				inputsProperty.serializedObject.Update();
				outputsProperty.serializedObject.Update();

				inputsList.Control.Refresh();
				outputsList.Control.Refresh();

				EnableInClassList(NoInputsUssClassName, value.Inputs.Count == 0);
				EnableInClassList(NoOutputsUssClassName, value.Outputs.Count == 0);
			});

			Add(graphField);
			Add(inputsList);
			Add(outputsList);

			EnableInClassList(NoInputsUssClassName, value.Inputs.Count == 0);
			EnableInClassList(NoOutputsUssClassName, value.Outputs.Count == 0);

			AddToClassList(UssClassName);
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
		}

		private abstract class GraphCallerProxy<T> : ListProxy
		{
			protected SerializedProperty _property;
			protected GraphCaller _caller;

			public override bool AllowAdd => false;
			public override bool AllowRemove => false;
			public override bool AllowReorder => false;

			public GraphCallerProxy(SerializedProperty property, GraphCaller caller)
			{
				_property = property;
				_caller = caller;
			}

			public override bool NeedsUpdate(VisualElement item, int index)
			{
				return !(item.userData is int i) || i != index;
			}

			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void ReorderItem(int from, int to) { }
		}

		private class InputsProxy : GraphCallerProxy<GraphInput>
		{
			public override int ItemCount => _caller.Inputs.Count;

			public InputsProxy(SerializedProperty property, GraphCaller caller) : base(property, caller)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var inputProperty = _property.GetArrayElementAtIndex(index);
				var nameProperty = inputProperty.FindPropertyRelative(nameof(GraphInput.Name));
				var typeProperty = inputProperty.FindPropertyRelative(nameof(GraphInput.Type));
				var referenceProperty = inputProperty.FindPropertyRelative(nameof(GraphInput.Reference));
				var valueProperty = inputProperty.FindPropertyRelative(nameof(GraphInput.Value));
				var input = _caller.Inputs[index];
				var definition = _caller.GetInputDefinition(input);

				var label = new Label();
				label.AddToClassList(NameUssClassName);
				label.BindProperty(nameProperty);

				var referenceField = new VariableReferenceField(referenceProperty, Autocomplete.GetItem(_caller.Graph));
				referenceField.SetLabel(null);

				var valueField = new SerializedVariableField(valueProperty, definition);
				valueField.AddToClassList(ValueUssClassName);
				
				var typeField = new EnumButtonsField(typeProperty);
				typeField.SetLabel(null);
				typeField.RegisterValueChangedCallback(evt =>
				{
					var type = (GraphInputType)evt.newValue;
				
					referenceField.SetDisplayed(type == GraphInputType.Reference);
					valueField.SetDisplayed(type == GraphInputType.Value);
				});
				
				referenceField.SetDisplayed(input.Type == GraphInputType.Reference);
				valueField.SetDisplayed(input.Type == GraphInputType.Value);
				
				var container = new VisualElement { userData = index };
				container.Add(label);
				container.Add(typeField);
				container.Add(referenceField);
				container.Add(valueField);

				return container;
			}
		}

		private class OutputsProxy : GraphCallerProxy<GraphOutput>
		{
			public override int ItemCount => _caller.Outputs.Count;

			public OutputsProxy(SerializedProperty property, GraphCaller caller) : base(property, caller)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var outputProperty = _property.GetArrayElementAtIndex(index);
				var nameProperty = outputProperty.FindPropertyRelative(nameof(GraphOutput.Name));
				var typeProperty = outputProperty.FindPropertyRelative(nameof(GraphOutput.Type));
				var referenceProperty = outputProperty.FindPropertyRelative(nameof(GraphOutput.Reference));
				var output = _caller.Outputs[index];

				var label = new Label();
				label.AddToClassList(NameUssClassName);
				label.BindProperty(nameProperty);

				var referenceField = new VariableReferenceField(referenceProperty, Autocomplete.GetItem(_caller.Graph));
				referenceField.SetLabel(null);
				
				var typeField = new EnumButtonsField(typeProperty);
				typeField.SetLabel(null);
				typeField.RegisterValueChangedCallback(evt =>
				{
					referenceField.SetDisplayed((GraphOutputType)evt.newValue == GraphOutputType.Reference);
				});
				
				referenceField.SetDisplayed(output.Type == GraphOutputType.Reference);
				
				var container = new VisualElement() { userData = index };
				container.Add(label);
				container.Add(typeField);
				container.Add(referenceField);

				return container;
			}
		}
	}
}
