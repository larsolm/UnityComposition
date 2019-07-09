using PiRhoSoft.Utilities.Engine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[Serializable]
	public class MenuItemTemplate
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
		[Conditional(nameof(Source), (int)ObjectSource.Scene)]
		public string Name;

		[Tooltip("The prefab to instantiate when showing this item on a SelectionControl")]
		[Conditional(nameof(Source), (int)ObjectSource.Asset)]
		public MenuItem Template;

		[Tooltip("The label used to identify the item")]
		[Conditional(nameof(Source), (int)ObjectSource.Asset)]
		public string Label;

		[Tooltip("If Variables is a List and this is set, this selection will be duplicated for each of the items in the list")]
		[Conditional(nameof(Source), (int)ObjectSource.Asset)]
		public bool Expand = false;

		public string Id => Source == ObjectSource.Scene ? Name : Label;
	}

	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "menu-item")]
	[AddComponentMenu("PiRho Soft/Interface/Menu Item")]
	public class MenuItem : BindingRoot
	{
		public string ItemName = "Item";

		public int Index { get => _data.Index; internal set => _data.Index = value; }
		public int Column { get => _data.Column; internal set => _data.Column = value; }
		public int Row { get => _data.Row; internal set => _data.Row = value; }
		public string Label { get => _data.Label; internal set => _data.Label = value; }
		public bool Focused { get => _data.Focused; internal set => _data.Focused = value; }

		public MenuItemTemplate Template { get; private set; }
		public bool Generated { get; private set; }

		private Menu _parent;
		private Data _data = new Data();

		private readonly string[] _names = new string[] { string.Empty, string.Empty };

		protected override void Awake()
		{
			base.Awake();

			_parent = GetComponentInParent<Menu>();

			if (_parent)
				_parent.AddItem(this);
		}

		protected virtual void OnDestroy()
		{
			if (_parent)
				_parent.RemoveItem(this);
		}

		public void Setup(MenuItemTemplate template, bool generated)
		{
			Template = template;
			Generated = generated;
		}

		public void Move(int index)
		{
			transform.SetSiblingIndex(index);

			if (_parent)
				_parent.MoveItem(this);
		}

		public override IList<string> GetVariableNames()
		{
			_names[0] = ValueName;
			_names[1] = ItemName;
			return _names;
		}

		public override VariableValue GetVariable(string name)
		{
			if (name == ItemName) return VariableValue.Create(_data);
			else return base.GetVariable(name);
		}

		public override SetVariableResult SetVariable(string name, VariableValue value)
		{
			return SetVariableResult.ReadOnly;
		}

		private class Data : IVariableStore
		{
			public int Index;
			public int Column;
			public int Row;
			public string Label;
			public bool Focused;

			private static List<string> _variableNames = new List<string> { nameof(Index), nameof(Column), nameof(Row), nameof(Label), nameof(Focused) };

			public IList<string> GetVariableNames()
			{
				return _variableNames;
			}

			public VariableValue GetVariable(string name)
			{
				switch (name)
				{
					case nameof(Index): return VariableValue.Create(Index);
					case nameof(Column): return VariableValue.Create(Column);
					case nameof(Row): return VariableValue.Create(Row);
					case nameof(Label): return VariableValue.Create(Label);
					case nameof(Focused): return VariableValue.Create(Focused);
					default: return VariableValue.Empty;
				}
			}

			public SetVariableResult SetVariable(string name, VariableValue value)
			{
				return SetVariableResult.ReadOnly;
			}
		}
	}
}
