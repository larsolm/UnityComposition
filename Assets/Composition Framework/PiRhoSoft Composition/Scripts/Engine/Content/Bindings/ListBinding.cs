﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "list-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/List Binding")]
	public class ListBinding : VariableBinding
	{
		private const string _missingTemplateWarning = "(CBLBMT) Unable to create item for List Binding '{0}': the Template is null";

		[Tooltip("The variable holding the list of items to show on this object")]
		public VariableReference Variable = new VariableReference();
		
		[Tooltip("The object to duplicate for each item in the bound list")]
		public BindingRoot Template;

		private List<VariableBinding> _bindings = new List<VariableBinding>();
		private List<BindingRoot> _items = new List<BindingRoot>();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Template != null)
			{
				if (Resolve(variables, Variable, out IVariableList list))
				{
					SyncItems(list.Count);

					for (var i = 0; i < list.Count; i++)
					{
						var item = list.GetVariable(i);
						SetItem(i, item);
					}
				}
				else if (Resolve(variables, Variable, out IVariableStore store))
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
