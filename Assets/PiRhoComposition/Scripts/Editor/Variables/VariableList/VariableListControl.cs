using PiRhoSoft.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class VariableListControl : ListControl
	{
		public VariableListControl(VariableListProxy proxy) : base(proxy)
		{
		}
	}

	public class VariableListProxy : ListProxy
	{
		public override bool AllowAdd => true;
		public override bool AllowRemove => true;
		public override bool AllowReorder => true;
		public override int ItemCount => Variables.VariableCount;

		public IVariableList Variables { get; private set; }
		public Object Owner { get; private set; }

		public void Setup(IVariableList variables, Object owner)
		{
			Variables = variables;
			Owner = owner;
		}

		public override VisualElement CreateElement(int index)
		{
			return new VariableControl(Variables.GetVariable(index), new VariableDefinition(), Owner) { userData = index };
		}

		public override bool NeedsUpdate(VisualElement item, int index)
		{
			return !(item.userData is int i) || i != index;
		}

		public override void AddItem()
		{
			Variables.AddVariable(Variable.Empty);
		}

		public override void RemoveItem(int index)
		{
			Variables.RemoveVariable(index);
		}

		public override void ReorderItem(int from, int to)
		{
			var toVariable = Variables.GetVariable(to);
			var fromVariable = Variables.GetVariable(from);

			Variables.SetVariable(to, fromVariable);
			Variables.SetVariable(from, toVariable);
		}
	}
}
