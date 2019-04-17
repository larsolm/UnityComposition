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

			if (value.TryGetList(out var list) && Template != null)
			{
				while (_items.Count < list.Count)
					_items.Add(null);

				if (_items.Count > list.Count)
					_items.RemoveRange(list.Count, _items.Count - list.Count);

				for (var i = 0; i < list.Count; i++)
				{
					var item = list.GetVariable(i);
					var equal = _items[i] != null ? VariableHandler.IsEqual(_items[i].Value, item) : null;

					if (!equal.HasValue || !equal.Value)
					{
						var binding = Instantiate(Template, transform);
						binding.transform.SetSiblingIndex(i);
						binding.Value = item;

						_items[i] = binding;
					}
				}
			}

			foreach (var item in _items)
				UpdateBinding(item.gameObject, string.Empty, status, _bindings);
		}
	}
}
