using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableDictionaryControl : DictionaryControl
	{
		public VariableDictionaryControl(VariableDictionaryProxy proxy) : base(proxy)
		{
		}
	}

	public class VariableDictionaryProxy : IDictionaryProxy
	{
		public string Label => "Variables";
		public string Tooltip => "The list of variables in this collection";
		public string EmptyLabel => "No variables exist in this collection";
		public string EmptyTooltip => "Add variables to the collection to edit them";
		public string AddPlaceholder => "(Add variable)";
		public string AddTooltip => "Add a variable to this collection";
		public string RemoveTooltip => "Remove this variable from the collection";
		public string ReorderTooltip => "Reorder this variable in the collection";

		public virtual bool AllowAdd => true;
		public virtual bool AllowRemove => true;
		public virtual bool AllowReorder => true;

		public int KeyCount => Variables.VariableNames.Count;

		public VariableDictionary Variables { get; private set; }
		public Object Owner { get; private set; }

		public void Setup(VariableDictionary variables, Object owner)
		{
			Variables = variables;
			Owner = owner;
		}

		public virtual VisualElement CreateField(int index)
		{
			return new VariableControl(Variables.GetVariable(index), new VariableDefinition(), Owner) { userData = index };
		}

		public bool NeedsUpdate(VisualElement item, int index)
		{
			// Definition !=, index !=, name !=
			return !(item.userData is int i) || i != index;
		}

		public void AddItem(string key)
		{
			Variables.AddVariable(key, Variable.Empty);
		}

		public void RemoveItem(int index)
		{
			var name = Variables.VariableNames[index];
			Variables.RemoveVariable(name);
		}

		public void ReorderItem(int from, int to)
		{
			var fromVariable = Variables.GetVariable(from);
			var toVariable = Variables.GetVariable(to);

			Variables.SetVariable(from, fromVariable);
			Variables.SetVariable(to, toVariable);
		}

		public virtual bool CanAdd(string key)
		{
			return !Variables.VariableNames.Contains(key);
		}

		public virtual bool CanRemove(int index)
		{
			return true;
		}

		public virtual bool CanReorder(int from, int to)
		{
			return true;
		}
	}
}
