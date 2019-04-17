using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "menu-item")]
	[AddComponentMenu("PiRho Soft/Interface/Menu Item")]
	public class MenuItem : BindingRoot
	{
		public string ItemName = "item";

		public int Index { get => _data.Index; set => _data.Index = value; }
		public string Label { get => _data.Label; set => _data.Label = value; }
		public bool Focused { get => _data.Focused; set => _data.Focused = value; }

		private Menu _parent;
		private Data _data = new Data();

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

		public void Move(int index)
		{
			transform.SetSiblingIndex(index);

			if (_parent)
				_parent.MoveItem(this);
		}

		public override IEnumerable<string> GetVariableNames()
		{
			return base.GetVariableNames().Concat(Enumerable.Repeat(ItemName, 1));
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
			public string Label;
			public bool Focused;

			public IEnumerable<string> GetVariableNames()
			{
				return new List<string> { nameof(Index), nameof(Label), nameof(Focused) };
			}

			public VariableValue GetVariable(string name)
			{
				switch (name)
				{
					case nameof(Index): return VariableValue.Create(Index);
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
