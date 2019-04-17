using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "list-binding")]
	[AddComponentMenu("PiRho Soft/Interface/List Binding")]
	public class ListBinding : VariableBinding
	{
		[Tooltip("The variable holding the list of items to show on this object")]
		public VariableReference Variable = new VariableReference();
		
		[Tooltip("The object to duplicate for each item in the bound list")]
		public BindingRoot Template;

		private List<VariableBinding> _bindings = new List<VariableBinding>();
		private List<BindingRoot> _items = new List<BindingRoot>();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			if (Template != null)
			{
				if (value.TryGetList(out var list))
				{
					SyncItems(list.Count);

					for (var i = 0; i < list.Count; i++)
					{
						var item = list.GetVariable(i);
						SetItem(i, item);
					}
				}
				else if (value.TryGetStore(out var store))
				{
					var names = store.GetVariableNames();

					SyncItems(names.Count);

					for (var i = 0; i < names.Count; i++)
					{
						var item = store.GetVariable(names[i]);
						SetItem(i, item);
					}
				}
			}

			foreach (var item in _items)
				UpdateBinding(item.gameObject, string.Empty, status, _bindings);
		}

		private void SyncItems(int count)
		{
			while (_items.Count < count)
				_items.Add(null);

			if (_items.Count > count)
				_items.RemoveRange(count, _items.Count - count);
		}

		private void SetItem(int index, VariableValue item)
		{
			var equal = _items[index] != null ? VariableHandler.IsEqual(_items[index].Value, item) : null;

			if (!equal.HasValue || !equal.Value)
			{
				var binding = Instantiate(Template, transform);
				binding.transform.SetSiblingIndex(i);
				binding.Value = item;

				_items[index] = binding;
			}
		}
	}
}
