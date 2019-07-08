using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "selection-control")]
	[RequireComponent(typeof(Menu))]
	public class SelectionControl : InterfaceControl
	{
		private const string _invalidExpandError = "(CSCIE) Failed to expand item '{0}' on SelectionControl '{1}': the variable '{2}' is not a List";
		private const string _missingItemError = "(CSCMI) Failed to create item '{0}' on SelectionControl '{1}': the variable '{2}' could not be found";
		private const string _invalidItemError = "(CSCII) Failed to create item '{0}' on SelectionControl '{1}': the variable '{2}' is not a Store or List";
		private const string _missingTemplateError = "(CSCMT) Failed to create item '{0}' on SelectionControl '{1}': the template has not been assigned";
		private const string _missingChildError = "(CSCMC) Failed to create item '{0}' on SelectionControl '{1}': the control does not have a child with the specified name";

		public bool IsRunning { get; private set; } = false;
		public bool IsSelectionRequired { get; private set; } = false;
		public bool IsClosing { get; private set; } = false;

		public bool HasFocusedItem => _menu.FocusedItem != null;
		public bool HasSelectedItem => _selectedItem != null;

		public MenuItem FocusedItem => _menu.FocusedItem;
		public int FocusedIndex => _menu.FocusedIndex;
		public VariableValue FocusedValue => _menu.FocusedItem == null ? VariableValue.Empty : _menu.FocusedItem.Value;

		public MenuItem SelectedItem => _selectedItem;
		public int SelectedIndex => _selectedItem == null ? -1 : _selectedItem.Index;
		public VariableValue SelectedValue => _selectedItem == null ? VariableValue.Empty : _selectedItem.Value;

		private Menu _menu;
		private MenuItem _selectedItem;

		protected override void Awake()
		{
			base.Awake();
			_menu = GetComponent<Menu>();
			_menu.AcceptsInput = false;
		}

		public void Show(IVariableStore variables, IEnumerable<MenuItemTemplate> items, bool isSelectionRequired, bool resetIndex)
		{
			Activate();
			StopAllCoroutines();
			CleanupItems();
			CreateItems(variables, items);
			Initialize(isSelectionRequired, resetIndex);
			StartCoroutine(Run_());
		}

		private IEnumerator Run_()
		{
			IsRunning = true;

			_menu.AcceptsInput = true;
			_menu.OnItemSelected += Select;
			_menu.OnCancelled += Close;

			yield return Run();

			_menu.OnItemSelected -= Select;
			_menu.OnCancelled -= Close;
			_menu.AcceptsInput = false;

			IsRunning = false;
		}

		private void Initialize(bool isSelectionRequired, bool resetIndex)
		{
			IsSelectionRequired = isSelectionRequired;
			IsClosing = false;

			_selectedItem = null;

			if (resetIndex && _menu.Items.Count > 0)
				_menu.FocusedItem = _menu.Items[0];

			OnInitialize();

			VariableBinding.UpdateBinding(gameObject, null, null);
		}

		public void Select(MenuItem item)
		{
			_selectedItem = item;
			Close();
		}

		public void Close()
		{
			if (!IsSelectionRequired || _selectedItem != null)
				IsClosing = true;
		}

		#region Item Management

		private void CleanupItems()
		{
			foreach (var item in _menu.Items)
			{
				if (item.Generated)
					Destroy(item.gameObject);
			}
		}

		private void CreateItems(IVariableStore variables, IEnumerable<MenuItemTemplate> items)
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
						Debug.LogErrorFormat(this, _missingItemError, item.Id, name, item.Variables);
					else
						Debug.LogErrorFormat(this, _invalidItemError, item.Id, name, item.Variables);
				}
				else
				{
					CreateStoreItem(item, VariableValue.Empty, ref index);
				}
			}

			OnCreate();
		}

		private void CreateStoreItem(MenuItemTemplate item, VariableValue value, ref int index)
		{
			if (item.Source == MenuItemTemplate.ObjectSource.Asset)
			{
				if (item.Template == null)
					Debug.LogErrorFormat(this, _missingTemplateError, item.Label, name);
				else if (item.Expand)
					Debug.LogErrorFormat(this, _invalidExpandError, item.Label, name, item.Variables);
				else
					AddItem(item, null, value, index++);
			}
			else if (item.Source == MenuItemTemplate.ObjectSource.Scene)
			{
				CreateSceneItem(item, value, ref index);
			}
		}

		private void CreateListItem(MenuItemTemplate item, VariableValue value, ref int index)
		{
			if (item.Source == MenuItemTemplate.ObjectSource.Asset)
			{
				if (item.Template == null)
					Debug.LogErrorFormat(this, _missingTemplateError, item.Label, name);
				else if (item.Expand)
					CreateExpandedItems(item, value, ref index);
				else
					AddItem(item, null, value, index++);
			}
			else if (item.Source == MenuItemTemplate.ObjectSource.Scene)
			{
				CreateSceneItem(item, value, ref index);
			}
		}

		private void CreateExpandedItems(MenuItemTemplate item, VariableValue value, ref int index)
		{
			var list = value.List;

			for (var i = 0; i < list.Count; i++)
				AddItem(item, null, list.GetVariable(i), index++);
		}

		private void CreateSceneItem(MenuItemTemplate item, VariableValue value, ref int index)
		{
			var obj = transform.Find(item.Name);
			var menu = obj != null ? obj.GetComponent<MenuItem>() : null;

			if (menu)
				AddItem(item, menu, value, index++);
			else
				Debug.LogErrorFormat(this, _missingChildError, item.Name, name);
		}

		private void AddItem(MenuItemTemplate item, MenuItem existing, VariableValue value, int index)
		{
			var parent = GetItemParent();
			var obj = existing == null ? Instantiate(item.Template, parent) : existing;

			obj.Setup(item, existing == null);
			obj.Move(index);

			obj.Index = index;
			obj.Label = item.Id;
			obj.Value = value;
			obj.Focused = false;
		}

		#endregion

		#region Virtual Interface

		protected virtual void OnInitialize()
		{
		}

		protected virtual void OnCreate()
		{
		}

		protected virtual Transform GetItemParent()
		{
			return transform;
		}

		protected virtual IEnumerator Run()
		{
			while (!IsClosing)
				yield return null;
		}

		protected override void Teardown()
		{
			CleanupItems();
		}

		#endregion
	}
}
