using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public enum PrimaryAxis
	{
		Column,
		Row
	}

	[HelpURL(Composition.DocumentationUrl + "input-selection")]
	[AddComponentMenu("PiRho Soft/Interface/Input Selection")]
	public class InputSelection : SelectionControl
	{
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

		protected int _columnCount;
		protected int _rowCount;
		protected int _columnIndex = 0;
		protected int _rowIndex = 0;

		protected override void OnCreate()
		{
			VariableBinding.UpdateBinding(gameObject, string.Empty, null);

			DetermineLayout();
		}

		protected override IEnumerator Run()
		{
			while (!HasSelectedItem && !IsClosing)
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
				else if (accept) Select();
				else if (cancel) Close();
			}
		}

		#region Item Management

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

		public override void MoveFocus(int index)
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

		protected virtual void MoveFocusUp(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-amount, VerticalWrapping, GetRowCount(column), 0, ref row, ref column, ref row);
		}

		protected virtual void MoveFocusDown(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(amount, VerticalWrapping, GetRowCount(column), 0, ref row, ref column, ref row);
		}

		protected virtual void MoveFocusLeft(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-amount, HorizontalWrapping, GetColumnCount(row), 0, ref column, ref column, ref row);
		}

		protected virtual void MoveFocusRight(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(amount, HorizontalWrapping, GetColumnCount(row), 0, ref column, ref column, ref row);
		}

		protected virtual bool MoveFocusToStart()
		{
			return MoveFocusToLocation(0, 0);
		}

		protected virtual bool MoveFocusToEnd()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowCount - 1);
		}

		protected virtual bool MoveFocusToTop()
		{
			return MoveFocusToLocation(_columnIndex, 0);
		}

		protected virtual bool MoveFocusToBottom()
		{
			return MoveFocusToLocation(_columnIndex, _rowCount - 1);
		}

		protected virtual bool MoveFocusToLeft()
		{
			return MoveFocusToLocation(0, _rowIndex);
		}

		protected virtual bool MoveFocusToRight()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowIndex);
		}

		protected bool MoveFocusToLocation(int column, int row)
		{
			if (IsLocationFocusable(column, row))
			{
				SetFocus(column, row, false);
				return true;
			}

			return false;
		}

		private bool SetFocusToValidLocation(int startingColumn, int startingRow)
		{
			if (PrimaryAxis == PrimaryAxis.Column)
				return SetFocusToValidRow(startingColumn, startingRow);
			else if (PrimaryAxis == PrimaryAxis.Row)
				return SetFocusToValidColumn(startingColumn, startingRow);
			else
				return false;
		}

		private void MoveFocus(int change, bool wrap, int count, int depth, ref int index, ref int column, ref int row)
		{
			index += change;

			if (index < 0) index = wrap ? count + index : 0;
			else if (index >= count) index = wrap ? index - count : count - 1;

			var focusable = IsLocationFocusable(column, row);

			if (focusable)
				SetFocus(column, row, false);
			else if (depth < count) // No need to recurse more than once through the column or row
				MoveFocus(change, wrap, count, depth + 1, ref index, ref column, ref row);
		}

		private void SetFocus(int column, int row, bool force)
		{
			if (force || column != _columnIndex || row != _rowIndex)
			{
				_columnIndex = column;
				_rowIndex = row;

				var item = GetItem(column, row);
				
				MoveFocus(_focusedItem.Index);
			}
		}

		private bool SetFocusToValidColumn(int startingColumn, int startingRow)
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

		private bool SetFocusToValidColumnInRow(int startingColumn, int row)
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

		private bool SetFocusToValidRow(int startingColumn, int startingRow)
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

		private bool SetFocusToValidRowInColumn(int column, int startingRow)
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

		private bool IsLocationFocusable(int column, int row)
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
