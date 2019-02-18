using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class SelectionItem : IVariableStore
	{
		[Tooltip("The label used to identify the item")] public string Label;
		[Tooltip("The variable representing the item to use for bindings")] public VariableReference Item = new VariableReference();
		[Tooltip("If Item has children and this is set, this selection will be duplicated for each of the children")] public bool Expand = false;
		[Tooltip("The prefab to instantiate when showing this item on a SelectionControl")] public GameObject Template;

		internal IVariableStore Variables { get; set; }

		public VariableValue GetVariable(string name)
		{
			if (name == nameof(Label)) return VariableValue.Create(Label);
			else return Variables.GetVariable(name);
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (name == nameof(Label)) return SetVariableResult.ReadOnly;
			else return Variables.SetVariable(name, value);
		}
	}
}
