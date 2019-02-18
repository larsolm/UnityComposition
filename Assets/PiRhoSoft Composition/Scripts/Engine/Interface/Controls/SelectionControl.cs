﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "selection-control")]
	[AddComponentMenu("Composition/Interface/Selection Control")]
	public class SelectionControl : InterfaceControl
	{
		private const string _invalidExpandWarning = "(ISCIE) Failed to expand item {0}: the variable '{1}' is not an IIndexedVariableStore";
		private const string _missingItemError = "(ISCMI) Failed to create item {0}: the variable '{1}' could not be found";
		private const string _invalidItemError = "(ISCMI) Failed to create item {0}: the variable '{1}' is not an IVariableStore";
		private const string _missingTemplateError = "(ISCMI) Failed to create item {0}: the object template has not been assigned";

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a column")]
		public bool VerticalWrapping = false;

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a row")]
		public bool HorizontalWrapping = false;

		public SelectionItem FocusedItem => _focusedItem?.Item;
		public IVariableStore FocusedVariables => _focusedItem?.Variables;

		public SelectionItem SelectedItem => _selectedItem?.Item;
		public IVariableStore SelectedVariables => _selectedItem?.SelectedVariables;

		protected int _columnCount;
		protected int _rowCount;
		protected bool _columnMajor;

		protected int _columnIndex = 0;
		protected int _rowIndex = 0;

		protected List<MenuItem> _items = new List<MenuItem>();
		protected MenuItem _focusedItem;
		protected MenuItem _selectedItem;

		private bool _isSelectionRequired = false;
		private bool _isClosing = false;

		public override void UpdateBindings(IVariableStore variables, string group)
		{
			base.UpdateBindings(variables, group);

			foreach (var item in _items)
				InterfaceBinding.UpdateBindings(item.Object, item.Variables, group);
		}

		public IEnumerator MakeSelection(IVariableStore variables, IEnumerable<SelectionItem> items, bool isSelectionRequired)
		{
			Activate();

			_isSelectionRequired = isSelectionRequired;
			_isClosing = false;
			_selectedItem = null;

			CreateItems(variables, items);
			UpdateBindings(variables, null);
			DetermineLayout();

			if (IsLocationFocusable(_columnIndex, _rowIndex))
				SetFocus(_columnIndex, _rowIndex, true);
			else
				SetFocusToValidLocation(0, 0);

			while (_selectedItem == null && !_isClosing)
			{
				if (InterfaceManager.Instance.Up.Pressed) MoveFocusUp();
				else if (InterfaceManager.Instance.Down.Pressed) MoveFocusDown();
				else if (InterfaceManager.Instance.Left.Pressed) MoveFocusLeft();
				else if (InterfaceManager.Instance.Right.Pressed) MoveFocusRight();
				else if (InterfaceManager.Instance.Accept.Pressed && _focusedItem != null) _selectedItem = _focusedItem;
				else if (InterfaceManager.Instance.Cancel.Pressed) Close();

				yield return null;
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
				Destroy(item.Object);

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
					if (item.Template == null)
					{
						Debug.LogErrorFormat(this, _missingTemplateError, item.Label);
					}
					else if (item.Expand)
					{
						if (store is IIndexedVariableStore indexed)
						{
							for (var i = 0; i < indexed.ItemCount; i++)
							{
								var indexedItem = indexed.GetItem(i);
								AddItem(item, indexedItem, indexedItem, index++);
							}
						}
						else
						{
							AddItem(item, store, store, index++);
							Debug.LogWarningFormat(this, _invalidExpandWarning, item.Label, item.Item);
						}
					}
					else
					{
						item.Variables = store;
						AddItem(item, item, store, index++);
					}
				}
			}
		}

		private IVariableStore GetStore(IVariableStore variables, SelectionItem item)
		{
			if (!item.Item.IsAssigned)
			{
				return variables;
			}
			else
			{
				var value = item.Item.GetValue(variables);

				if (value.Type == VariableType.Empty)
				{
					Debug.LogErrorFormat(this, _missingItemError, item.Label, item.Item);
					return null;
				}
				else if (!value.TryGetStore(out var store))
				{
					Debug.LogErrorFormat(this, _invalidItemError, item.Label, item.Item);
					return null;
				}
				else
				{
					return store;
				}
			}
		}

		private void AddItem(SelectionItem item, IVariableStore variables, IVariableStore selectedVariables, int index)
		{
			if (index < _items.Count)
			{
				if (_items[index].Item == item && _items[index].Variables == variables)
					return;

				_items.RemoveRange(index, _items.Count - index);
			}

			var parent = GetItemParent();
			var obj = Instantiate(item.Template, parent);
			var indicator = obj.GetComponentInChildren<FocusIndicator>();
			var selector = obj.GetComponentInChildren<ItemSelector>();

			if (selector)
				selector.Index = _items.Count;

			_items.Add(new MenuItem
			{
				Item = item,
				Variables = variables,
				SelectedVariables = selectedVariables,
				Object = obj,
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
			var mainIndex = _columnMajor ? column : row;
			var crossIndex = _columnMajor ? row : column;
			var crossCount = _columnMajor ? _rowCount : _columnCount;

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
			if (_columnMajor)
				return SetFocusToValidRow(startingColumn, startingRow);
			else
				return SetFocusToValidColumn(startingColumn, startingRow);
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
			var columns = 0;
			var rows = 0;

			Canvas.ForceUpdateCanvases();

			if (GetComponent<VerticalLayoutGroup>() != null)
			{
				columns = 1;
				_columnMajor = true;
			}
			else if (GetComponent<HorizontalLayoutGroup>() != null)
			{
				rows = 1;
				_columnMajor = false;
			}
			else
			{
				var grid = GetComponent<GridLayoutGroup>();
				if (grid != null)
				{
					_columnMajor = grid.startAxis == GridLayoutGroup.Axis.Vertical;

					switch (grid.constraint)
					{
						case GridLayoutGroup.Constraint.FixedColumnCount: columns = grid.constraintCount; break;
						case GridLayoutGroup.Constraint.FixedRowCount: rows = grid.constraintCount; break;
					}
				}
				else
				{
					_columnMajor = DetermineAxis();
				}
			}

			if (rows == 0 && columns == 0)
			{
				if (_columnMajor)
					rows = DetermineHeight();
				else
					columns = DetermineWidth();
			}

			_columnCount = columns > 0 ? columns : GetCrossSize(rows);
			_rowCount = rows > 0 ? rows : GetCrossSize(columns);
		}

		private int GetCrossSize(int count)
		{
			return Mathf.CeilToInt(_items.Count / (float)count);
		}

		private int DetermineHeight()
		{
			var height = 0;
			var y = float.MaxValue;

			foreach (var item in _items)
			{
				var itemY = (item.Object.transform as RectTransform).anchoredPosition.y;

				if (itemY > y)
					break;

				y = itemY;
				height++;
			}

			return height;
		}

		private int DetermineWidth()
		{
			var width = 0;
			var x = float.MinValue;

			foreach (var item in _items)
			{
				var itemX = (item.Object.transform as RectTransform).anchoredPosition.x;

				if (itemX < x)
					break;

				x = itemX;
				width++;
			}

			return width;
		}

		private bool DetermineAxis()
		{
			if (_items.Count > 1)
			{
				var item0 = _items[0].Object.transform as RectTransform;
				var item1 = _items[1].Object.transform as RectTransform;

				var x = Mathf.Abs(item0.anchoredPosition.x - item1.anchoredPosition.x);
				var y = Mathf.Abs(item0.anchoredPosition.y - item1.anchoredPosition.y);

				return y > x;
			}
			else
			{
				return true;
			}
		}

		private int GetColumnCount(int row)
		{
			if (_columnMajor)
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
			if (_columnMajor)
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
