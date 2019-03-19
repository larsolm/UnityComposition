using PiRhoSoft.UtilityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum PrimaryAxis
	{
		Column,
		Row
	}

	[HelpURL(Composition.DocumentationUrl + "selection-control")]
	[AddComponentMenu("PiRho Soft/Interface/Selection Control")]
	public class SelectionControl : InterfaceControl
	{
		private const string _invalidExpandWarning = "(CSCIE) Failed to expand item {0}: the variable '{1}' is not an IIndexedVariableStore";
		private const string _missingItemError = "(CSCMI) Failed to create item {0}: the variable '{1}' could not be found";
		private const string _invalidItemError = "(CSCII) Failed to create item {0}: the variable '{1}' is not an IVariableStore";
		private const string _missingTemplateError = "(ISCMT) Failed to create item {0}: the object template has not been assigned";
		private const string _missingChildError = "(ISCMC) Failed to create item {0}: SelectionControl '{1}' does not have a child with the specified name";
		private const string _missingBindingError = "(CSCMB) Failed to initialize item {0}: the template '{1}' does not have a Binding Root";
		private const string _invalidBindingError = "(CSCMB) Failed to bind index {0} of item {1}: the binding is not an IVariableStore";

		[Tooltip("The input axis to use for horizontal movement")]
		public string HorizontalAxis = "Horizontal";

		[Tooltip("The input axis to use for vertical movement")]
		public string VerticalAxis = "Vertical";

		[Tooltip("The input button to use for accepting the selection")]
		public string AcceptButton = "Submit";

		[Tooltip("The input button to use for canceling the selection")]
		public string CancelButton = "Cancel";

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a column")]
		public bool VerticalWrapping = false;

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a row")]
		public bool HorizontalWrapping = false;

		[Tooltip("Specifies the axis in which the items in the selection are ordered")]
		public PrimaryAxis PrimaryAxis = PrimaryAxis.Column;

		[Tooltip("How many columns are in this selection")]
		[ConditionalDisplaySelf(nameof(PrimaryAxis), EnumValue = (int)PrimaryAxis.Column)]
		[Minimum(1)]
		public int ColumnCount = 1;

		[Tooltip("How many rows are in this selection")]
		[ConditionalDisplaySelf(nameof(PrimaryAxis), EnumValue = (int)PrimaryAxis.Row)]
		[Minimum(1)]
		public int RowCount = 1;

		public SelectionItem FocusedItem => _focusedItem?.Item;
		public IVariableStore FocusedVariables => _focusedItem?.Variables;

		public SelectionItem SelectedItem => _selectedItem?.Item;
		public IVariableStore SelectedVariables => _selectedItem?.SelectedVariables;

		protected int _columnCount;
		protected int _rowCount;
		protected int _columnIndex = 0;
		protected int _rowIndex = 0;

		protected List<MenuItem> _items = new List<MenuItem>();
		protected MenuItem _focusedItem;
		protected MenuItem _selectedItem;

		private bool _isSelectionRequired = false;
		private bool _isClosing = false;

		public IEnumerator MakeSelection(IVariableStore variables, IEnumerable<SelectionItem> items, bool isSelectionRequired)
		{
			Activate();

			_isSelectionRequired = isSelectionRequired;
			_isClosing = false;
			_selectedItem = null;

			CreateItems(variables, items);
			VariableBinding.UpdateBinding(gameObject, string.Empty, null);
			DetermineLayout();

			if (IsLocationFocusable(_columnIndex, _rowIndex))
				SetFocus(_columnIndex, _rowIndex, true);
			else
				SetFocusToValidLocation(0, 0);

			while (_selectedItem == null && !_isClosing)
			{
				yield return null; // wait one frame for input

				var left = InputHelper.GetWasAxisPressed(HorizontalAxis, -0.25f);
				var right = InputHelper.GetWasAxisPressed(HorizontalAxis, 0.25f);
				var up = InputHelper.GetWasAxisPressed(VerticalAxis, 0.25f);
				var down = InputHelper.GetWasAxisPressed(VerticalAxis, -0.25f);
				var accept = InputHelper.GetWasButtonPressed(AcceptButton);
				var cancel = InputHelper.GetWasButtonPressed(CancelButton);

				if (up) MoveFocusUp();
				else if (down) MoveFocusDown();
				else if (left) MoveFocusLeft();
				else if (right) MoveFocusRight();
				else if (accept && _focusedItem != null) _selectedItem = _focusedItem;
				else if (cancel) Close();
			}

			Deactivate();
		}

		public void Close()
		{
			if (!_isSelectionRequired)
				_isClosing = true;
		}

		protected override void Teardown()
		{
			foreach (var item in _items)
			{
				if (item.Generated)
					Destroy(item.Object);
			}

			_items.Clear();

			base.Teardown();
		}

		#region Item Management

		protected class MenuItem
		{
			public SelectionItem Item;
			public IVariableStore Variables;
			public IVariableStore SelectedVariables;
			public GameObject Object;
			public bool Generated;
			public FocusIndicator Indicator;
			public ItemSelector Selector;
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
				var store = GetStore(variables, item);
				if (store != null)
				{
					if (item.Source == SelectionItem.ObjectSource.Asset)
					{
						if (item.Template == null)
						{
							Debug.LogErrorFormat(this, _missingTemplateError, item.Label);
						}
						else if (item.Expand)
						{
							if (store is IIndexedVariableStore indexed)
							{
								for (var i = 0; i < indexed.Count; i++)
								{
									var indexedItem = indexed.GetItem(i) as IVariableStore;
									if (indexedItem != null)
										AddItem(item, null, indexedItem, indexedItem, index++);
									else
										Debug.LogWarningFormat(this, _invalidBindingError, i, item.Name);
								}
							}
							else
							{
								AddItem(item, null, store, store, index++);
								Debug.LogWarningFormat(this, _invalidExpandWarning, item.Label, item.Variables);
							}
						}
						else
						{
							item.Store = store;
							AddItem(item, null, item, store, index++);
						}
					}
					else if (item.Source == SelectionItem.ObjectSource.Scene)
					{
						var obj = transform.Find(item.Name);

						if (obj == null)
							Debug.LogErrorFormat(this, _missingChildError, item.Name, name);
						else
							AddItem(item, obj.gameObject, item, store, index++);
					}
				}
			}
		}

		private IVariableStore GetStore(IVariableStore variables, SelectionItem item)
		{
			if (!item.Variables.IsAssigned)
			{
				return variables;
			}
			else
			{
				var value = item.Variables.GetValue(variables);

				if (value.Type == VariableType.Empty)
				{
					Debug.LogErrorFormat(this, _missingItemError, item.Id, item.Variables);
					return null;
				}
				else if (!value.TryGetStore(out var store))
				{
					Debug.LogErrorFormat(this, _invalidItemError, item.Id, item.Variables);
					return null;
				}
				else
				{
					return store;
				}
			}
		}

		private void AddItem(SelectionItem item, GameObject child, IVariableStore variables, IVariableStore selectedVariables, int index)
		{
			if (index < _items.Count)
			{
				if (_items[index].Item == item && _items[index].Variables == variables)
					return;

				_items.RemoveRange(index, _items.Count - index);
			}

			var parent = GetItemParent();
			var obj = child == null ? Instantiate(item.Template, parent) : child; // Don't null coalesce
			obj.transform.SetSiblingIndex(index);

			var binding = obj.GetComponent<BindingRoot>();
			var indicator = obj.GetComponentInChildren<FocusIndicator>(true);
			var selector = obj.GetComponentInChildren<ItemSelector>();

			if (binding != null)
				binding.Variables = variables;
			else if (item.Source == SelectionItem.ObjectSource.Asset)
				Debug.LogErrorFormat(this, _missingBindingError, item.Name, item.Template.name);

			if (selector)
			{
				selector.Selection = this;
				selector.Index = index;
			}

			_items.Add(new MenuItem
			{
				Item = item,
				Variables = variables,
				SelectedVariables = selectedVariables,
				Object = obj,
				Generated = child == null,
				Indicator = indicator,
				Selector = selector
			});
		}

		public void SelectItem(int index)
		{
			_selectedItem = index >= 0 && index < _items.Count ? _items[index] : null;
		}

		protected MenuItem GetItem(int column, int row)
		{
			var mainIndex = PrimaryAxis == PrimaryAxis.Column ? column : row;
			var crossIndex = PrimaryAxis == PrimaryAxis.Column ? row : column;
			var crossCount = PrimaryAxis == PrimaryAxis.Column ? _rowCount : _columnCount;

			var index = mainIndex * crossCount + crossIndex;
			return index >= 0 && index < _items.Count ? _items[index] : null;
		}

		#endregion

		#region Focus Management

		protected virtual void FocusItem(MenuItem item)
		{
			if (item.Indicator)
				item.Indicator.SetFocused(true);
		}

		protected virtual void BlurItem(MenuItem item)
		{
			if (item.Indicator)
				item.Indicator.SetFocused(false);
		}

		public void MoveFocus(int index)
		{
			if (PrimaryAxis == PrimaryAxis.Column)
			{
				var column = index / (_items.Count / _columnCount);
				var row = index % _rowCount;

				MoveFocusToLocation(column, row);
			}
			else if (PrimaryAxis == PrimaryAxis.Row)
			{
				var column = index % _columnCount;
				var row = index / (_items.Count / _rowCount);

				MoveFocusToLocation(column, row);
			}
		}

		public virtual void MoveFocusUp()
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-1, VerticalWrapping, GetRowCount(column), 0, ref row, ref column, ref row);
		}

		public virtual void MoveFocusDown()
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(1, VerticalWrapping, GetRowCount(column), 0, ref row, ref column, ref row);
		}

		public virtual void MoveFocusLeft()
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-1, HorizontalWrapping, GetColumnCount(row), 0, ref column, ref column, ref row);
		}

		public virtual void MoveFocusRight()
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(1, HorizontalWrapping, GetColumnCount(row), 0, ref column, ref column, ref row);
		}

		public bool MoveFocusToStart()
		{
			return MoveFocusToLocation(0, 0);
		}

		public bool MoveFocusToEnd()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowCount - 1);
		}

		public bool MoveFocusToTop()
		{
			return MoveFocusToLocation(_columnIndex, 0);
		}

		public bool MoveFocusToBottom()
		{
			return MoveFocusToLocation(_columnIndex, _rowCount - 1);
		}

		public bool MoveFocusToLeft()
		{
			return MoveFocusToLocation(0, _rowIndex);
		}

		public bool MoveFocusToRight()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowIndex);
		}

		public bool MoveFocusToLocation(int column, int row)
		{
			if (IsLocationFocusable(column, row))
			{
				SetFocus(column, row, false);
				return true;
			}

			return false;
		}

		public bool SetFocusToValidLocation(int startingColumn, int startingRow)
		{
			if (PrimaryAxis == PrimaryAxis.Column)
				return SetFocusToValidRow(startingColumn, startingRow);
			else if (PrimaryAxis == PrimaryAxis.Row)
				return SetFocusToValidColumn(startingColumn, startingRow);
			else
				return false;
		}

		protected void MoveFocus(int change, bool wrap, int count, int depth, ref int index, ref int column, ref int row)
		{
			index += change;

			if (index < 0) index = wrap ? count - 1 : 0;
			else if (index >= count) index = wrap ? 0 : count - 1;

			var focusable = IsLocationFocusable(column, row);

			if (focusable)
				SetFocus(column, row, false);
			else if (depth < count) // No need to recurse more than once through the column or row
				MoveFocus(change, wrap, count, depth + 1, ref index, ref column, ref row);
		}

		protected void SetFocus(int column, int row, bool force)
		{
			if (force || column != _columnIndex || row != _rowIndex)
			{
				if (_focusedItem != null)
					BlurItem(_focusedItem);

				_columnIndex = column;
				_rowIndex = row;
				_focusedItem = GetItem(column, row);

				if (_focusedItem != null)
					FocusItem(_focusedItem);
			}
		}

		protected bool SetFocusToValidColumn(int startingColumn, int startingRow)
		{
			for (var row = startingRow; row < _rowCount; row++)
			{
				if (SetFocusToValidColumnInRow(startingColumn, row))
					return true;
			}

			for (var row = 0; row < startingRow; row++)
			{
				if (SetFocusToValidColumnInRow(startingColumn, row))
					return true;
			}

			return false;
		}

		protected bool SetFocusToValidColumnInRow(int startingColumn, int row)
		{
			for (var column = startingColumn; column < _columnCount; column++)
			{
				if (IsLocationFocusable(column, row))
				{
					SetFocus(column, row, true);
					return true;
				}
			}

			for (var column = 0; column < startingColumn; column++)
			{
				if (IsLocationFocusable(column, row))
				{
					SetFocus(column, row, true);
					return true;
				}
			}

			return false;
		}

		protected bool SetFocusToValidRow(int startingColumn, int startingRow)
		{
			for (var column = startingColumn; column < _columnCount; column++)
			{
				if (SetFocusToValidRowInColumn(column, startingRow))
					return true;
			}

			for (var column = 0; column < startingColumn; column++)
			{
				if (SetFocusToValidRowInColumn(column, startingRow))
					return true;
			}

			return false;
		}

		protected bool SetFocusToValidRowInColumn(int column, int startingRow)
		{
			for (var row = startingRow; row < _rowCount; row++)
			{
				if (IsLocationFocusable(column, row))
				{
					SetFocus(column, row, true);
					return true;
				}
			}

			for (var row = 0; row < startingRow; row++)
			{
				if (IsLocationFocusable(column, row))
				{
					SetFocus(column, row, true);
					return true;
				}
			}

			return false;
		}

		protected bool IsLocationFocusable(int column, int row)
		{
			var item = GetItem(column, row);
			return item != null && item.Object.activeInHierarchy;
		}

		#endregion

		#region Layout

		private void DetermineLayout()
		{
			if (PrimaryAxis == PrimaryAxis.Column)
			{
				_columnCount = ColumnCount;
				_rowCount = GetCrossSize(_columnCount);
			}
			else if (PrimaryAxis == PrimaryAxis.Row)
			{
				_rowCount = RowCount;
				_columnCount = GetCrossSize(_rowCount);
			}
		}

		private int GetCrossSize(int count)
		{
			return Mathf.CeilToInt(_items.Count / (float)count);
		}

		private int GetColumnCount(int row)
		{
			if (PrimaryAxis == PrimaryAxis.Row)
			{
				if (row < GetRowCount(_columnCount - 1))
					return _columnCount;
				else
					return _columnCount - 1;
			}
			else
			{
				if (row == _rowCount - 1)
					return _columnCount - (_columnCount * _rowCount - _items.Count);
				else
					return _columnCount;
			}
		}

		private int GetRowCount(int column)
		{
			if (PrimaryAxis == PrimaryAxis.Column)
			{
				if (column == _columnCount - 1)
					return _rowCount - (_rowCount * _columnCount - _items.Count);
				else
					return _rowCount;
			}
			else
			{
				if (column < GetColumnCount(_rowCount - 1))
					return _rowCount;
				else
					return _rowCount - 1;
			}
		}

		#endregion
	}
}
