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

		[Tooltip("The variable representing the store to use for bindings")]
		public VariableReference Variables = new VariableReference("this");

		[Tooltip("The location to retrieve the object from")]
		public ObjectSource Source;

		[Tooltip("The name of the object in the scene to associate with this Item")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Scene)]
		public string Name;

		[Tooltip("The prefab to instantiate when showing this item on a SelectionControl")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public GameObject Template;

		[Tooltip("The label used to identify the item")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public string Label;

		[Tooltip("If Item is an IIndexedVariableStore and this is set, this selection will be duplicated for each of the children")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public bool Expand = false;

		public string Id => Source == ObjectSource.Scene ? Name : Label;

		internal IVariableStore Store { get; set; }

		public VariableValue GetVariable(string name)
		{
			if (name == nameof(Label)) return VariableValue.Create(Label);
			else return Store.GetVariable(name);
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			if (name == nameof(Label)) return SetVariableResult.ReadOnly;
			else return Store.SetVariable(name, value);
		}

		public IEnumerable<string> GetVariableNames()
		{
			return Store.GetVariableNames().Concat(new List<string> { nameof(Label) });
		}
	}
}
