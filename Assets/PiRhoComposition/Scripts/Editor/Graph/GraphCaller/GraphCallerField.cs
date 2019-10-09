using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerField : BindableElement
	{
		public static readonly string UssClassName = "pirho-graph-caller-field";

		public GraphCaller Value { get; private set; }

		private InputsProxy _inputsProxy;
		private OutputsProxy _outputsProxy;
		private ListField _inputsList;
		private ListField _outputsList;

		public GraphCallerField(SerializedProperty property, string tooltip)
		{
			Setup(property, tooltip);
		}

		private void Setup(SerializedProperty property, string tooltip)
		{
			Value = property.GetObject<GraphCaller>();

			bindingPath = property.propertyPath;

			var graphProperty = property.FindPropertyRelative("_graph");
			var inputsProperty = property.FindPropertyRelative("_inputs");
			var outputsProperty = property.FindPropertyRelative("_outputs");

			var graphField = new ObjectPickerField(property.displayName, Value.Graph, property.serializedObject.targetObject, typeof(Graph));
			graphField.RegisterValueChangedCallback(evt => Refresh());

			var graph = graphField.ConfigureProperty(graphProperty, tooltip);

			_inputsProxy = new InputsProxy(Value, inputsProperty) { Label = "Inputs", Tooltip = "The input values to set for the Graph" };
			_outputsProxy = new OutputsProxy(Value, outputsProperty) { Label = "Outputs", Tooltip = "The output values to resolve from this Graph" };
			_inputsList = new ListField();
			_outputsList = new ListField();

			_inputsList.Setup(inputsProperty, _inputsProxy);
			_outputsList.Setup(outputsProperty, _outputsProxy);

			Add(graph);
			Add(_inputsList);
			Add(_outputsList);

			AddToClassList(UssClassName);

			Refresh();
		}

		private void Refresh()
		{
			Value.UpdateVariables();

			_inputsList.SetDisplayed(Value.Inputs.Count > 0);
			_outputsList.SetDisplayed(Value.Outputs.Count > 0);
		}

		private abstract class GraphCallerProxy<T> : ListProxy
		{
			public SerializedProperty Property;
			public GraphCaller Caller;

			public override bool AllowAdd => false;
			public override bool AllowRemove => false;
			public override bool AllowReorder => false;

			public GraphCallerProxy(GraphCaller caller, SerializedProperty property)
			{
				Property = property;
				Caller = caller;
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
			public override int ItemCount => Caller.Inputs.Count;

			public InputsProxy(GraphCaller caller, SerializedProperty property) : base(caller, property)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var property = Property.GetArrayElementAtIndex(index);
				var nameProperty = property.FindPropertyRelative(nameof(GraphInput.Name));
				var typeProperty = property.FindPropertyRelative(nameof(GraphInput.Type));
				var referenceProperty = property.FindPropertyRelative(nameof(GraphInput.Reference));
				var valueProperty = property.FindPropertyRelative(nameof(GraphInput.Value));
				var input = Caller.Inputs[index];

				var label = new TextElement { text = input.Name, bindingPath = nameProperty.propertyPath };

				var referenceField = new VariableReferenceField(null, input.Reference, Autocomplete.GetItem(Caller.Graph)) { bindingPath = referenceProperty.propertyPath };

				/// TODO - Make this the field that Adam made ///
				var valueField = new VariableControl(input.Value.Variable, Caller.GetInputDefinition(input), property.serializedObject.targetObject);
				valueField.RegisterCallback<ChangeEvent<Variable>>(evt =>
				{
					input.Value.Variable = evt.newValue;
				});

				var typeField = new EnumField(input.Type) { bindingPath = typeProperty.propertyPath };
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
			public override int ItemCount => Caller.Outputs.Count;

			public OutputsProxy(GraphCaller caller, SerializedProperty property) : base(caller, property)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var property = Property.GetArrayElementAtIndex(index);
				var nameProperty = property.FindPropertyRelative(nameof(GraphOutput.Name));
				var typeProperty = property.FindPropertyRelative(nameof(GraphOutput.Type));
				var referenceProperty = property.FindPropertyRelative(nameof(GraphOutput.Reference));
				var output = Caller.Outputs[index];

				var label = new TextElement { text = output.Name, bindingPath = nameProperty.propertyPath };

				var referenceField = new VariableReferenceField(null, output.Reference, Autocomplete.GetItem(Caller.Graph)) { bindingPath = referenceProperty.propertyPath };

				var typeField = new EnumField(output.Type) { bindingPath = typeProperty.propertyPath };
				typeField.RegisterValueChangedCallback(evt =>
				{
					var type = (GraphOutputType)evt.newValue;
					referenceField.SetDisplayed(type == GraphOutputType.Reference);
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
