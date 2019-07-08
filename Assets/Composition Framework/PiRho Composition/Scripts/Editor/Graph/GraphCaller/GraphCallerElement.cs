using PiRhoSoft.Composition.Engine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class GraphCallerElement : VisualElement
	{
		private const string _styleSheetPath = Engine.Composition.StylePath + "Graph/GraphCaller/GraphCallerElement.uss";
		private const string _ussBaseClass = "pargon-graph-caller";
		private const string _ussItemClass = "definition-item";
		private const string _ussLabelClass = "definition-label";
		private const string _ussDropdownClass = "definition-type";
		private const string _ussReferenceClass = "definition-reference";
		private const string _ussValueClass = "definition-value";

		private Object _owner;
		private GraphCaller _caller;

		private ListElement _inputs;
		private ListElement _outputs;

		public GraphCallerElement(SerializedProperty property, string tooltip)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);

			var container = ElementHelper.CreatePropertyContainer(property.displayName, tooltip);

			_owner = property.serializedObject.targetObject;
			_caller = PropertyHelper.GetObject<GraphCaller>(property);

			var picker = new ObjectPicker(property.FindPropertyRelative("_graph"));
			picker.Setup(typeof(Graph), _caller.Graph);
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
				var proxy = new ListProxy<GraphInput>(_caller.Inputs, CreateInput);
				_inputs = new ListElement(proxy, "Inputs", ElementHelper.GetTooltip(typeof(GraphCaller), "_inputs"), false, false, false);
				Add(_inputs);
			}

			if (_caller.Outputs.Count == 0)
			{
				var proxy = new ListProxy<GraphOutput>(_caller.Outputs, CreateOutput);
				_outputs = new ListElement(proxy, "Outputs", ElementHelper.GetTooltip(typeof(GraphCaller), "_outputs"), false, false, false);
				Add(_outputs);
			}
		}

		private VisualElement CreateInput(GraphInput input, int index)
		{
			var label = new Label(input.Name);
			label.AddToClassList(_ussLabelClass);

			var referenceElement = new VariableReferenceElement();
			referenceElement.Setup(_owner, input.Reference, null); // TODO: Add an autocompletesource
			referenceElement.AddToClassList(_ussReferenceClass);

			var valueElement = new VariableValueElement(_owner, () => input.Value, value => input.Value = value, () => _caller.GetInputDefinition(input).Definition);
			valueElement.AddToClassList(_ussValueClass);

			var typeElement = new EnumDropdown<GraphInputType>(input.Type, _owner, () => (int)input.Type, value => input.Type = (GraphInputType)value);
			typeElement.AddToClassList(_ussDropdownClass);
			typeElement.RegisterCallback<ChangeEvent<int>>(e =>
			{
				var t = (GraphInputType)e.newValue;

				ElementHelper.SetVisible(referenceElement, t == GraphInputType.Reference);
				ElementHelper.SetVisible(valueElement, t == GraphInputType.Value);
			});

			ElementHelper.SetVisible(referenceElement, input.Type == GraphInputType.Reference);
			ElementHelper.SetVisible(valueElement, input.Type == GraphInputType.Value);

			var container = new VisualElement();
			container.AddToClassList(_ussItemClass);
			container.Add(label);
			container.Add(typeElement);
			container.Add(referenceElement);
			container.Add(valueElement);

			return container;
		}

		private VisualElement CreateOutput(GraphOutput output, int index)
		{
			var label = new Label(output.Name);
			label.AddToClassList(_ussLabelClass);

			var reference = new VariableReferenceElement();
			reference.Setup(_owner, output.Reference, null); // TODO: Add an autocompletesource
			reference.AddToClassList(_ussReferenceClass);

			var type = new EnumDropdown<GraphOutputType>(output.Type, _owner, () => (int)output.Type, value => output.Type = (GraphOutputType)value);
			type.AddToClassList(_ussDropdownClass);
			type.RegisterCallback<ChangeEvent<int>>(e => ElementHelper.SetVisible(reference, (GraphOutputType)e.newValue == GraphOutputType.Reference));

			ElementHelper.SetVisible(reference, output.Type == GraphOutputType.Reference);

			var container = new VisualElement();
			container.AddToClassList(_ussItemClass);
			container.Add(label);
			container.Add(type);
			container.Add(reference);

			return container;
		}
	}
}
