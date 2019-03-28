using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class SelectionItem
	{
		public enum ObjectSource
		{
			Scene,
			Asset
		}

		[Tooltip("The variable representing the store to use for bindings")]
		public VariableReference Variables = new VariableReference();

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

		[Tooltip("If Variables is a List and this is set, this selection will be duplicated for each of the items in the list")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)ObjectSource.Asset)]
		public bool Expand = false;

		public string Id => Source == ObjectSource.Scene ? Name : Label;
	}
}
