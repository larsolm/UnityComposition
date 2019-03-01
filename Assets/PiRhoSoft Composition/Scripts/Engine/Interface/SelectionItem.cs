using PiRhoSoft.UtilityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class SelectionItem : IVariableStore
	{
		public enum ObjectSource
		{
			Scene,
			Asset
		}

		[Tooltip("The label used to identify the item")]
		public string Label;

		[Tooltip("The variable representing the store to use for bindings")]
		public VariableReference Item = new VariableReference("this");

		[Tooltip("The location to retrieve the object from")]
		public ObjectSource Source;

		[Tooltip("The prefab to instantiate when showing this item on a SelectionControl")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public GameObject Template;

		[Tooltip("If Item is an IIndexedVariableStore and this is set, this selection will be duplicated for each of the children")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public bool Expand = false;

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

		public IEnumerable<string> GetVariableNames()
		{
			return Variables.GetVariableNames().Concat(Enumerable.Repeat(nameof(Label), 1));
		}
	}
}
