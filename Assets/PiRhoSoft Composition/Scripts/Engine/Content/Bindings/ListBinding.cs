using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "list-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/List Binding")]
	public class ListBinding : VariableBinding
	{
		private const string _missingTemplateWarning = "(CBLBMT) Unable to create list for list binding '{0}': the object template was null";
		private const string _missingVariableWarning = "(CBLBMV) Unable to create list for list binding '{0}': the variable '{1}' could not be found";
		private const string _invalidVariableWarning = "(CBLBIV) Unable to create list for list binding '{0}': the variable '{1}' is not a color";

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
				else if (!SuppressErrors)
				{
					Debug.LogWarningFormat(this, value.IsEmpty ? _missingVariableWarning : _invalidVariableWarning, name, Variable);
				}
			}
			else
			{
				Debug.LogWarningFormat(this, _missingTemplateWarning, name);
			}

			foreach (var item in _items)
				UpdateBinding(item.gameObject, string.Empty, status, _bindings);
		}

		private void SyncItems(int count)
		{
			while (_items.Count < count)
				_items.Add(null);

			if (count < _items.Count)
			{
				for (var i = count; i < _items.Count; i++)
					Destroy(_items[i].gameObject);

				_items.RemoveRange(count, _items.Count - count);
			}
		}

		private void SetItem(int index, VariableValue item)
		{
			if (_items[index] != null)
			{
				var equal = VariableHandler.IsEqual(_items[index].Value, item);

				if (equal.HasValue && equal.Value)
					return;

				Destroy(_items[index].gameObject);
			}

			var binding = Instantiate(Template, transform);
			binding.transform.SetSiblingIndex(index);
			binding.Value = item;

			_items[index] = binding;
		}
	}
}
