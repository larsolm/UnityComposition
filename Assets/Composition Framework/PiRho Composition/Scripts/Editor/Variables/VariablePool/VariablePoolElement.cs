using PiRhoSoft.Composition.Engine;
using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition.Editor
{
	public class VariablePoolElement : VisualElement
	{
		private const string _styleSheetPath = Engine.Composition.StylePath + "Variables/VariablePool/VariablePoolElement.uss";
		private const string _ussBaseClass = "pargon-variable-pool";
		private const string _ussItemClass = "definition-item";
		private const string _ussEditClass = "definition-edit";
		private const string _ussVariableContainerClass = "definition-variable-container";
		private const string _ussValueContainerClass = "definition-value-container";
		private const string _ussLabelClass = "definition-label";
		private const string _ussValueClass = "definition-value";

		public VariablePoolElement(SerializedProperty property) : this(property.serializedObject.targetObject, PropertyHelper.GetObject<VariablePool>(property))
		{
		}

		public VariablePoolElement(Object owner, VariablePool variables)
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			AddToClassList(_ussBaseClass);

			if (owner is ISchemaOwner schemaOwner)
				schemaOwner.SetupSchema();

			Add(new ListElement(new VariablesProxy(owner, variables), "Variables", "The variables defined by this variable pool"));
		}

		private class VariablesProxy : ListProxy
		{
			public VariablePool Variables;

			private readonly Object _owner;

			private bool _definitionVisible = false;

			public override int Count => Variables.Variables.Count;
			
			public VariablesProxy(Object owner, VariablePool variables)
			{
				_owner = owner;
				Variables = variables;
			}

			public override VisualElement CreateElement(int index)
			{
				var label = new TextField() { value = Variables.Names[index], isDelayed = true };
				label.AddToClassList(_ussLabelClass);
				label.RegisterValueChangedCallback(evt =>
				{
					if (!Variables.Map.ContainsKey(evt.newValue))
						Variables.ChangeName(index, evt.newValue);
					else
						label.SetValueWithoutNotify(evt.previousValue);
				});

				ElementHelper.Bind(label, label, _owner, () => Variables.Names[index], value =>
				{
					if (!Variables.Map.ContainsKey(value))
						Variables.ChangeName(index, value);
					else
						label.SetValueWithoutNotify(Variables.Names[index]);
				});

				var valueElement = new VariableValueElement(_owner, () => Variables.Variables[index], value => Variables.SetVariable(index, value), () => Variables.Definitions[index]);
				valueElement.AddToClassList(_ussValueClass);

				var definition = new ValueDefinitionElement(_owner, () => Variables.Definitions[index], value =>
				{
					Variables.ChangeDefinition(index, value);
					valueElement.Setup(Variables.Variables[index], Variables.Definitions[index]);
				}, () => VariableInitializerType.None, () => null, false);

				var edit = new Image { image = Icon.Settings.Content, tooltip = "Show/hide the definition of this variable" };
				edit.AddToClassList(_ussEditClass);
				edit.AddManipulator(new Clickable(() =>
				{
					_definitionVisible = !_definitionVisible;
					ElementHelper.SetVisible(definition, _definitionVisible);
				}));

				var variableContainer = new VisualElement();
				variableContainer.AddToClassList(_ussVariableContainerClass);

				var valueContainer = new VisualElement();
				valueContainer.AddToClassList(_ussValueContainerClass);

				var container = new VisualElement();
				container.AddToClassList(_ussItemClass);
				container.Add(edit);
				container.Add(variableContainer);
				variableContainer.Add(valueContainer);
				variableContainer.Add(definition);
				valueContainer.Add(label);
				valueContainer.Add(valueElement);

				ElementHelper.SetVisible(definition, _definitionVisible);

				return container;
			}

			public override void AddItem()
			{
				// TODO: make sure this is viable
				Variables.AddVariable("NewVariable", VariableValue.Empty);
			}

			public override void RemoveItem(int index)
			{
				Variables.RemoveVariable(index);
			}

			public override void MoveItem(int from, int to)
			{
				Variables.VariableMoved(from, to);
			}
		}
	}
}
