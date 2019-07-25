using PiRhoSoft.Utilities.Editor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerControl : VisualElement
	{
		public GraphCaller Value { get; private set; }

		private InputsProxy _inputsProxy;
		private OutputsProxy _outputsProxy;

		private ListControl _inputsList;
		private ListControl _outputsList;

		public GraphCallerControl(GraphCaller value)
		{
			Value = value;

			var picker = new ObjectPickerControl(Value.Graph, typeof(Graph)) { tooltip = "The Graph to execute when this is called" };
			picker.RegisterCallback<ChangeEvent<Object>>(evt =>
			{
				Value.Graph = evt.newValue as Graph;
				Refresh();
			});

			_inputsProxy = new InputsProxy(Value) { Label = "Inputs", Tooltip = "The input values to set for the Graph" };
			_outputsProxy = new OutputsProxy(Value) { Label = "Outputs", Tooltip = "The output values to resolve from this Graph" };

			_inputsList = new ListControl(_inputsProxy);
			_outputsList = new ListControl(_outputsProxy);

			Add(picker);
			Add(_inputsList);
			Add(_outputsList);

			Refresh();
		}

		public void SetValueWithoutNotify(GraphCaller newValue)
		{
			Value = newValue;
			Refresh();
		}

		private void Refresh()
		{
			Value.UpdateVariables();

			_inputsProxy.Caller = Value;
			_outputsProxy.Caller = Value;

			_inputsList.Refresh();
			_outputsList.Refresh();

			_inputsList.SetDisplayed(Value.Inputs.Count > 0);
			_outputsList.SetDisplayed(Value.Outputs.Count > 0);
		}

		private abstract class CallerProxy<T> : ListProxy
		{
			public GraphCaller Caller;

			public override bool AllowAdd => false;
			public override bool AllowRemove => false;
			public override bool AllowReorder => false;

			public CallerProxy(GraphCaller caller)
			{
				Caller = caller;
			}

			public override bool NeedsUpdate(VisualElement item, int index) => true;
			public override void AddItem() { }
			public override void RemoveItem(int index) { }
			public override void ReorderItem(int from, int to) { }
		}

		private class InputsProxy : CallerProxy<GraphInput>
		{
			public override int ItemCount => Caller.Inputs.Count;

			public InputsProxy(GraphCaller caller) : base(caller)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var input = Caller.Inputs[index];
				var label = new Label(input.Name);

				var referenceControl = new VariableReferenceControl(input.Reference, null); // TODO: Add an autocompletesource

				//var valueControl = new VariableValueControl(input.Value, Caller.GetInputDefinition(input).Definition);
				//valueControl.RegisterCallback<ChangeEvent<VariableValue>>(evt =>
				//{
				//	input.Value = evt.newValue;
				//});

				var typeField = new EnumField(input.Type);
				typeField.RegisterValueChangedCallback(evt => 
				{
					var type = (GraphInputType)evt.newValue;
				
					referenceControl.SetDisplayed(type == GraphInputType.Reference);
					referenceControl.SetDisplayed(type == GraphInputType.Value);
				});

				referenceControl.SetDisplayed(input.Type == GraphInputType.Reference);
				referenceControl.SetDisplayed(input.Type == GraphInputType.Value);

				var container = new VisualElement();
				container.Add(label);
				container.Add(typeField);
				container.Add(referenceControl);
				//container.Add(valueControl);

				return container;
			}
		}

		private class OutputsProxy : CallerProxy<GraphOutput>
		{
			public override int ItemCount => Caller.Outputs.Count;

			public OutputsProxy(GraphCaller caller) : base(caller)
			{
			}

			public override VisualElement CreateElement(int index)
			{
				var output = Caller.Outputs[index];
				var label = new Label(output.Name);

				var referenceControl = new VariableReferenceControl(output.Reference, null); // TODO: Add an autocompletesource

				var typeField = new EnumField(output.Type);
				typeField.RegisterValueChangedCallback(evt =>
				{
					var type = (GraphOutputType)evt.newValue;
					referenceControl.SetDisplayed(type == GraphOutputType.Reference);
				});

				referenceControl.SetDisplayed(output.Type == GraphOutputType.Reference);

				var container = new VisualElement();
				container.Add(label);
				container.Add(typeField);
				container.Add(referenceControl);

				return container;

			}
		}
	}
}
