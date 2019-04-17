using PiRhoSoft.UtilityEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "menu-input")]
	[AddComponentMenu("PiRho Soft/Interface/Menu Input")]
	[RequireComponent(typeof(Menu))]
	public class MenuInput : MonoBehaviour
	{
		[Tooltip("The input axis to use for horizontal movement")]
		public string HorizontalAxis = "Horizontal";

		[Tooltip("The input axis to use for vertical movement")]
		public string VerticalAxis = "Vertical";

		[Tooltip("The input button to use for accepting the selection")]
		public string SelectButton = "Submit";

		[Tooltip("The input button to use for canceling the selection")]
		public string CancelButton = "Cancel";

		[Tooltip("Specifies the axis along which the items in the menu are ordered")]
		public PrimaryAxis PrimaryAxis = PrimaryAxis.Column;

		[Tooltip("How many rows are in this selection")]
		[ConditionalDisplaySelf(nameof(PrimaryAxis), EnumValue = (int)PrimaryAxis.Row)]
		[Minimum(1)]
		public int RowCount = 1;

		[Tooltip("How many columns are in this selection")]
		[ConditionalDisplaySelf(nameof(PrimaryAxis), EnumValue = (int)PrimaryAxis.Column)]
		[Minimum(1)]
		public int ColumnCount = 1;

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a column")]
		[ConditionalDisplaySelf(nameof(RowCount), IntValue = 1, Invert = true)]
		public bool VerticalWrapping = false;

		[Tooltip("Specifies if focus should wrap when moving the cursor past the beginning or end of a row")]
		[ConditionalDisplaySelf(nameof(ColumnCount), IntValue = 1, Invert = true)]
		public bool HorizontalWrapping = false;

		private int _rowCount;
		private int _columnCount;
		private int _rowIndex = 0;
		private int _columnIndex = 0;

		private Menu _menu;

		void Awake()
		{
			_menu = GetComponent<Menu>();
			_menu.OnItemAdded += OnItemAdded;
			_menu.OnItemRemoved += OnItemRemoved;
			_menu.OnItemMoved += OnItemMoved;
		}

		void Update()
		{
			var left = InputHelper.GetWasAxisPressed(HorizontalAxis, -0.25f);
			var right = InputHelper.GetWasAxisPressed(HorizontalAxis, 0.25f);
			var up = InputHelper.GetWasAxisPressed(VerticalAxis, 0.25f);
			var down = InputHelper.GetWasAxisPressed(VerticalAxis, -0.25f);
			var select = InputHelper.GetWasButtonPressed(SelectButton);
			var cancel = InputHelper.GetWasButtonPressed(CancelButton);
			
			if (left) MoveFocusLeft();
			else if (right) MoveFocusRight();
			else if (up) MoveFocusUp();
			else if (down) MoveFocusDown();
			else if (select) _menu.SelectFocusedItem();
			else if (cancel) _menu.Cancel();
		}

		#region Item Management

		private void OnItemAdded(MenuItem item)
		{
			RefreshLayout();
		}

		private void OnItemRemoved(MenuItem item)
		{
			RefreshLayout();
		}

		private void OnItemMoved(MenuItem item, int from)
		{
			RefreshLayout();
		}

		#endregion

		#region Focus Management

		private void MoveFocusUp(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-amount, VerticalWrapping, _rowCount, 0, ref row, ref column, ref row);
		}

		private void MoveFocusDown(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(amount, VerticalWrapping, _rowCount, 0, ref row, ref column, ref row);
		}

		private void MoveFocusLeft(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(-amount, HorizontalWrapping, _columnCount, 0, ref column, ref column, ref row);
		}

		private void MoveFocusRight(int amount = 1)
		{
			var row = _rowIndex;
			var column = _columnIndex;

			MoveFocus(amount, HorizontalWrapping, _columnCount, 0, ref column, ref column, ref row);
		}

		private bool MoveFocusToStart()
		{
			return MoveFocusToLocation(0, 0);
		}

		private bool MoveFocusToEnd()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowCount - 1);
		}

		private bool MoveFocusToTop()
		{
			return MoveFocusToLocation(_columnIndex, 0);
		}

		private bool MoveFocusToBottom()
		{
			return MoveFocusToLocation(_columnIndex, _rowCount - 1);
		}

		private bool MoveFocusToLeft()
		{
			return MoveFocusToLocation(0, _rowIndex);
		}

		private bool MoveFocusToRight()
		{
			return MoveFocusToLocation(_columnCount - 1, _rowIndex);
		}

		private bool MoveFocusToLocation(int column, int row)
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

				if (item != null)
					_menu.SetFocusedItem(item);
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
			return item != null && item.gameObject.activeInHierarchy;
		}

		#endregion

		#region Layout

		public void RefreshLayout()
		{
			if (PrimaryAxis == PrimaryAxis.Column)
			{
				_columnCount = ColumnCount;
				_rowCount = Mathf.CeilToInt(_menu.Items.Count / (float)_columnCount);
			}
			else if (PrimaryAxis == PrimaryAxis.Row)
			{
				_rowCount = RowCount;
				_columnCount = Mathf.CeilToInt(_menu.Items.Count / (float)_rowCount);
			}

			SetFocusToValidLocation(_columnIndex, _rowIndex);
		}

		private MenuItem GetItem(int column, int row)
		{
			var mainIndex = PrimaryAxis == PrimaryAxis.Column ? column : row;
			var crossIndex = PrimaryAxis == PrimaryAxis.Column ? row : column;
			var crossCount = PrimaryAxis == PrimaryAxis.Column ? _rowCount : _columnCount;

			var index = mainIndex * crossCount + crossIndex;
			return index >= 0 && index < _menu.Items.Count ? _menu.Items[index] : null;
		}

		#endregion
	}
}
