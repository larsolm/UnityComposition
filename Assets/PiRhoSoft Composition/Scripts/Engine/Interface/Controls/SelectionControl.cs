using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "selection-control")]
	public abstract class SelectionControl : InterfaceControl
	{
		private const string _invalidExpandWarning = "(CSCIE) Failed to expand item {0}: the variable '{1}' is not an IVariableList";
		private const string _missingItemError = "(CSCMI) Failed to create item {0}: the variable '{1}' could not be found";
		private const string _invalidItemError = "(CSCII) Failed to create item {0}: the variable '{1}' is not an IVariableStore or IVariableList";
		private const string _missingTemplateError = "(ISCMT) Failed to create item {0}: the object template has not been assigned";
		private const string _missingChildError = "(ISCMC) Failed to create item {0}: SelectionControl '{1}' does not have a child with the specified name";
		private const string _missingBindingError = "(CSCMB) Failed to initialize item {0}: the template '{1}' does not have a Binding Root";

		public bool IsRunning { get; private set; } = false;
		public bool IsSelectionRequired { get; private set; } = false;
		public bool IsClosing { get; private set; } = false;

		public bool HasFocusedItem => _focusedItem != null;
		public bool HasSelectedItem => _selectedItem != null;

		public SelectionItem FocusedItem => _focusedItem?.Item;
		public int FocusedIndex => _focusedItem == null ? -1 : _selectedItem.Index;
		public VariableValue FocusedValue => _focusedItem == null ? VariableValue.Empty : _focusedItem.Value;

		public SelectionItem SelectedItem => _selectedItem?.Item;
		public int SelectedIndex => _selectedItem == null ? -1 : _selectedItem.Index;
		public VariableValue SelectedValue => _selectedItem == null ? VariableValue.Empty : _selectedItem.Value;

		protected List<MenuItem> _items = new List<MenuItem>();
		protected int _focusedIndex;
		protected MenuItem _focusedItem;
		protected MenuItem _selectedItem;

		public void Show(IVariableStore variables, IEnumerable<SelectionItem> items, bool isSelectionRequired, bool resetIndex)
		{
			Activate();
			StopAllCoroutines();
			Initialize(isSelectionRequired, resetIndex);
			Create(variables, items);
			StartCoroutine(Run_());
		}

		private IEnumerator Run_()
		{
			IsRunning = true;

			yield return Run();

			IsRunning = false;
		}

		private void Initialize(bool isSelectionRequired, bool resetIndex)
		{
			IsSelectionRequired = isSelectionRequired;
			IsClosing = false;

			_selectedItem = null;
			_focusedItem = null;

			if (resetIndex)
				_focusedIndex = 0;

			OnInitialize();
		}

		private void Create(IVariableStore variables, IEnumerable<SelectionItem> items)
		{
			CreateItems(variables, items);
			MoveFocus(_focusedIndex);
			OnCreate();
		}

		public void Close()
		{
			if (!IsSelectionRequired)
				IsClosing = true;
		}

		public void Select()
		{
			if (_focusedItem != null)
				_selectedItem = _focusedItem;
		}

		#region Item Management

		protected class MenuItem : IVariableStore
		{
			public SelectionItem Item;
			public GameObject Object;
			public bool Generated;

			public int Index;
			public bool Focused;
			public string Label;
			public VariableValue Value;

			public VariableValue GetVariable(string name)
			{
				switch (name)
				{
					case nameof(Index): return VariableValue.Create(Index);
					case nameof(Label): return VariableValue.Create(Label);
					case nameof(Focused): return VariableValue.Create(Focused);
					case nameof(Value): return Value;
					default: return VariableValue.Empty;
				}
			}

			public SetVariableResult SetVariable(string name, VariableValue value)
			{
				return SetVariableResult.ReadOnly;
			}

			public IEnumerable<string> GetVariableNames()
			{
				return new List<string> { nameof(Index), nameof(Label), nameof(Focused), nameof(Value) };
			}
		}

		protected virtual Transform GetItemParent()
		{
			return transform;
		}

		private void CreateItems(IVariableStore variables, IEnumerable<SelectionItem> items)
		{
			var index = 0;

			foreach (var item in items)
			{
				if (item.Variables.IsAssigned)
				{
					var value = item.Variables.GetValue(variables);

					if (value.HasList)
						CreateListItem(item, value, ref index);
					else if (value.HasStore)
						CreateStoreItem(item, value, ref index);
					else if (value.IsEmpty)
						Debug.LogErrorFormat(this, _missingItemError, item.Id, item.Variables);
					else
						Debug.LogErrorFormat(this, _invalidItemError, item.Id, item.Variables);
				}
				else
				{
					CreateStoreItem(item, VariableValue.Create(variables), ref index);
				}
			}
		}

		private void CreateStoreItem(SelectionItem item, VariableValue value, ref int index)
		{
			if (item.Source == SelectionItem.ObjectSource.Asset)
			{
				if (item.Template == null)
					Debug.LogErrorFormat(this, _missingTemplateError, item.Label);
				else if (item.Expand)
					Debug.LogWarningFormat(this, _invalidExpandWarning, item.Label, item.Variables);
				else
					AddItem(item, null, value, index++);
			}
			else if (item.Source == SelectionItem.ObjectSource.Scene)
			{
				CreateSceneItem(item, value, ref index);
			}
		}

		private void CreateListItem(SelectionItem item, VariableValue value, ref int index)
		{
			if (item.Source == SelectionItem.ObjectSource.Asset)
			{
				if (item.Template == null)
					Debug.LogErrorFormat(this, _missingTemplateError, item.Label);
				else if (item.Expand)
					CreateExpandedItems(item, value, ref index);
				else
					AddItem(item, null, value, index++);
			}
			else if (item.Source == SelectionItem.ObjectSource.Scene)
			{
				CreateSceneItem(item, value, ref index);
			}
		}

		private void CreateExpandedItems(SelectionItem item, VariableValue value, ref int index)
		{
			var list = value.List;

			for (var i = 0; i < list.Count; i++)
				AddItem(item, null, list.GetVariable(i), index++);
		}

		private void CreateSceneItem(SelectionItem item, VariableValue value, ref int index)
		{
			var obj = transform.Find(item.Name);

			if (obj)
				AddItem(item, obj.gameObject, value, index++);
			else
				Debug.LogErrorFormat(this, _missingChildError, item.Name, name);
		}

		private void AddItem(SelectionItem item, GameObject existing, VariableValue value, int index)
		{
			if (index < _items.Count)
			{
				if (_items[index].Item == item)
					return;

				_items.RemoveRange(index, _items.Count - index);
			}

			var parent = GetItemParent();
			var obj = existing == null ? Instantiate(item.Template, parent) : existing;
			obj.transform.SetSiblingIndex(index);

			var binding = obj.GetComponent<BindingRoot>();

			if (binding)
				binding.Value = value;
			else if (item.Source == SelectionItem.ObjectSource.Asset)
				Debug.LogErrorFormat(this, _missingBindingError, item.Name, item.Template.name);
			
			_items.Add(new MenuItem
			{
				Item = item,
				Object = obj,
				Generated = existing == null,

				Index = index,
				Label = item.Id,
				Value = value,
				Focused = false
			});
		}

		public void SelectItem(int index)
		{
			_selectedItem = index >= 0 && index < _items.Count ? _items[index] : null;
		}

		public int GetIndex(ItemSelector selector)
		{
			foreach (var item in _items)
			{
				if (item.Object.GetComponentInChildren<ItemSelector>() == selector)
					return item.Index;
			}

			return -1;
		}

		protected MenuItem GetItem(int index)
		{
			return index >= 0 && index < _items.Count ? _items[index] : null;
		}

		#endregion

		#region Focus Management

		public virtual void MoveFocus(int index)
		{
			var item = GetItem(index);

			if (_focusedItem != null)
				BlurItem(_focusedItem);

			if (item != null)
				FocusItem(item);
		}

		private void FocusItem(MenuItem item)
		{
			item.Focused = true;
			_focusedIndex = item.Index;
		}

		private void BlurItem(MenuItem item)
		{
			item.Focused = false;
		}

		#endregion

		#region Virtual Interface

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnCreate()
		{
		}

		protected virtual IEnumerator Run()
		{
			yield break;
		}

		protected override void Teardown()
		{
			foreach (var item in _items)
			{
				if (item.Generated)
					Destroy(item.Object);
			}

			_items.Clear();
		}

		#endregion
	}
}
