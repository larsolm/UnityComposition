using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariablePoolControl : VisualElement
	{
		public VariablePool Value { get; private set; }

		private VariablesProxy _proxy;
		private DictionaryControl _dictionary;

		public VariablePoolControl(VariablePool value)
		{
			Value = value;

			_proxy = new VariablesProxy(value);
			_dictionary = new DictionaryControl(_proxy);

			Refresh();

			Add(_dictionary);
		}

		public void SetValueWithoutNotify(VariablePool value)
		{
			Value = value;
			_proxy.Variables = Value;

			Refresh();
		}

		private void Refresh()
		{
			_dictionary.Refresh();
		}

		private class VariablesProxy : IDictionaryProxy
		{
			public VariablePool Variables;
			
			public string Label => "Variables";
			public string Tooltip => "The variables defined by this variable pool";
			public string EmptyLabel => "No variables are currently in this pool";
			public string EmptyTooltip => "Use the add button to add variables to this pool";
			public string AddPlaceholder => "(Add Variable)";
			public string AddTooltip => "Add a variable to the pool";
			public string RemoveTooltip => "Remove this variable from the pool";
			public string ReorderTooltip => "Move this variable in the pool";

			public int ItemCount => Variables.Variables.Count;
			public bool AllowAdd => true;
			public bool AllowRemove => true;
			public bool AllowReorder => true;

			public VariablesProxy(VariablePool variables)
			{
				Variables = variables;
			}

			public VisualElement CreateField(int index)
			{
				var name = Variables.Names[index];
				var value = Variables.Variables[index];
				var definition = Variables.Definitions[index];

				var rollout = new RolloutControl(false);

				var label = new TextField() { value = name, isDelayed = true };
				label.RegisterValueChangedCallback(evt =>
				{
					if (!Variables.Map.ContainsKey(evt.newValue))
						Variables.ChangeName(index, evt.newValue);
					else
						label.SetValueWithoutNotify(evt.previousValue);
				});

				var valueControl = new VariableValueControl(value, definition);
				valueControl.RegisterCallback<ChangeEvent<VariableValue>>(evt => Variables.SetVariable(index, evt.newValue));

				var definitionControl = new ValueDefinitionControl(definition, VariableInitializerType.None, null, false);
				definitionControl.RegisterCallback<ChangeEvent<ValueDefinition>>(evt =>
				{
					Variables.ChangeDefinition(index, evt.newValue);
					valueControl.SetDefinition(evt.newValue, Variables.Variables[index]);
				});

				rollout.Header.Add(label);
				rollout.Header.Add(valueControl);
				rollout.Content.Add(definitionControl);

				return rollout;
			}

			public bool IsKeyValid(string key)
			{
				return !Variables.Map.ContainsKey(key);
			}

			public bool NeedsUpdate(VisualElement item, int index)
			{
				return true;
			}

			public void AddItem(string key)
			{
				Variables.AddVariable(key, VariableValue.Empty);
			}

			public void RemoveItem(int index)
			{
				Variables.RemoveVariable(index);
			}

			public void ReorderItem(int from, int to)
			{
				Variables.VariableMoved(from, to);
			}
		}
	}
}
