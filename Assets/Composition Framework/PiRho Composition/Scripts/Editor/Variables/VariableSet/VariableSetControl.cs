﻿using PiRhoSoft.Utilities.Editor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableSetControl : VisualElement
	{
		public ConstrainedStore Value { get; private set; }

		private VariablesProxy _proxy;
		private ListControl _list;

		public VariableSetControl(ConstrainedStore value)
		{
			Value = value;

			Refresh();

			_proxy = new VariablesProxy(value);
			_list = new ListControl(_proxy);

			Add(_list);
		}

		public void SetValueWithoutNotify(ConstrainedStore value)
		{
			Value = value;
			_proxy.Variables = Value;

			Refresh();
		}

		private void Refresh()
		{
			_list.Refresh();
		}

		private class VariablesProxy : IListProxy
		{
			public ConstrainedStore Variables;

			public string Label => "Variables";
			public string Tooltip => "The list of variables defined by this variable set";
			public string EmptyLabel => "No variables exist in this set";
			public string EmptyTooltip => "Add variables to the set to edit them";
			public string AddTooltip => "Not allowed for variable sets";
			public string RemoveTooltip => "Not allowed for variable sets";
			public string ReorderTooltip => "Not allowed for variable sets";

			public int ItemCount => Variables.VariableCount;

			public bool AllowAdd => false;
			public bool AllowRemove => false;
			public bool AllowReorder => false;

			public bool CanAdd() => false;
			public bool CanRemove(int index) => false;
			public bool CanReorder(int from, int to) => false;

			public VariablesProxy(ConstrainedStore variables)
			{
				Variables = variables;
			}

			public VisualElement CreateElement(int index)
			{
				var container = new VisualElement();
				var entry = Variables.Schema.GetEntry(index);
				var value = Variables.GetVariable(index);
				
				if (entry != null && Variables.Owner != null)
				{
					var field = new VariableField(entry.Definition.Name, value, entry.Definition);
					field.RegisterCallback<ChangeEvent<Variable>>(evt => {	Variables.SetVariable(index, value); });
					container.Add(field);
					
					var refreshButton = new IconButton(Icon.Refresh.Texture, "Re-compute this variable based on the schema initializer", () =>
					{
						var newValue = entry.GenerateVariable(Variables.Owner);
						field.value = newValue;
					});
					
					container.Add(refreshButton);
				}
				
				return container;
			}

			public bool NeedsUpdate(VisualElement item, int index) => true;
			public void AddItem() { }
			public void RemoveItem(int index) { }
			public void ReorderItem(int from, int to) { }
		}
	}
}
