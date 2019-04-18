using PiRhoSoft.UtilityEngine;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	public enum MenuInputPointerAction
	{
		None,
		Focus,
		Select
	}

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

		[Tooltip("The action to perform when hovering over a menu item")]
		public MenuInputPointerAction HoverAction = MenuInputPointerAction.Focus;

		[Tooltip("The action to perform when clicking a menu item")]
		public MenuInputPointerAction ClickAction = MenuInputPointerAction.Focus;

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

		[Tooltip("The menu to transfer input to when moving past the left of this menu")]
		public MenuInput NextLeft;

		[Tooltip("The menu to transfer input to when moving past the right of this menu")]
		public MenuInput NextRight;

		[Tooltip("The menu to transfer input to when moving past the top of this menu")]
		public MenuInput NextUp;

		[Tooltip("The menu to transfer input to when moving past the bottom of this menu")]
		public MenuInput NextDown;

		[Tooltip("Set this to enable input for this menu when it first loads")]
		public bool FocusOnLoad = true;

		[Tooltip("The distance between the edge of the scroll viewport and the menu item when scrolling to the menu item")]
		public float ScrollPadding = 10.0f;

		private bool _shouldFocus;
		private int _rowCount;
		private int _columnCount;
		private int _rowIndex = -1;
		private int _columnIndex = -1;

		private Menu _menu;

		void Awake()
		{
			_menu = GetComponent<Menu>();
			_menu.OnItemAdded += OnItemAdded;
			_menu.OnItemRemoved += OnItemRemoved;
			_menu.OnItemMoved += OnItemMoved;

			_shouldFocus = FocusOnLoad;
		}

		void Update()
		{
			if (_menu.FocusedItem != null)
			{
				var left = !string.IsNullOrEmpty(HorizontalAxis) ? InputHelper.GetWasAxisPressed(HorizontalAxis, -0.25f) : false;
				var right = !string.IsNullOrEmpty(HorizontalAxis) ? InputHelper.GetWasAxisPressed(HorizontalAxis, 0.25f) : false;
				var up = !string.IsNullOrEmpty(VerticalAxis) ? InputHelper.GetWasAxisPressed(VerticalAxis, 0.25f) : false;
				var down = !string.IsNullOrEmpty(VerticalAxis) ? InputHelper.GetWasAxisPressed(VerticalAxis, -0.25f) : false;
				var select = !string.IsNullOrEmpty(SelectButton) ? InputHelper.GetWasButtonPressed(SelectButton) : false;
				var cancel = !string.IsNullOrEmpty(CancelButton) ? InputHelper.GetWasButtonPressed(CancelButton) : false;

				if (left) MoveFocusLeft(1);
				else if (right) MoveFocusRight(1);
				else if (up) MoveFocusUp(1);
				else if (down) MoveFocusDown(1);
				else if (select) _menu.SelectItem(_menu.FocusedItem);
				else if (cancel) _menu.Cancel();
			}

			switch (HoverAction)
			{
				case MenuInputPointerAction.Focus: FocusWithMouse(); break;
				case MenuInputPointerAction.Select: SelectWithMouse(); break;
			}

			if (InputHelper.GetWasPointerPressed())
			{
				switch (ClickAction)
				{
					case MenuInputPointerAction.Focus: FocusWithMouse(); break;
					case MenuInputPointerAction.Select: SelectWithMouse(); break;
				}
			}
		}

		#region Items

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

		#region Input

		private void FocusWithMouse()
		{
			var rect = (transform as RectTransform);
			var local = rect.InverseTransformPoint(Input.mousePosition);

			if (rect.rect.Contains(local))
			{
				var item = GetItem(local, out var column, out var row);
				if (item != null)
				{
					ChangeFocus(column, row);

					if (NextLeft) NextLeft.Leave();
					if (NextRight) NextRight.Leave();
					if (NextUp) NextUp.Leave();
					if (NextDown) NextDown.Leave();
				}
			}
		}

		private void SelectWithMouse()
		{
			var rect = (transform as RectTransform);
			var local = rect.InverseTransformPoint(Input.mousePosition);

			if (rect.rect.Contains(local))
			{
				var item = GetItem(local, out var column, out var row);
				if (item != null)
					_menu.SelectItem(item);
			}
		}

		#endregion

		#region Focus

		private void EnterLeft(int fromRow)
		{
			_columnIndex = 0;
			_rowIndex = fromRow;

			MoveFocusUp(0);
		}

		private void EnterRight(int fromRow)
		{
			_columnIndex = _columnCount - 1;
			_rowIndex = fromRow;

			MoveFocusUp(0);
		}

		private void EnterTop(int fromColumn)
		{
			_columnIndex = fromColumn;
			_rowIndex = 0;

			MoveFocusLeft(0);
		}

		private void EnterBottom(int fromColumn)
		{
			_columnIndex = fromColumn;
			_rowIndex = _rowCount - 1;

			MoveFocusLeft(0);
		}

		public void Leave()
		{
			_menu.SetFocusedItem(null);
		}

		public void MoveFocusUp(int amount)
		{
			if (!ChangeFocus(_columnIndex, _rowIndex - amount))
			{
				if (_rowIndex > amount)
				{
					MoveFocusUp(amount + 1);
				}
				else if (NextUp && NextUp._menu.Items.Count > 0)
				{
					Leave();
					NextUp.EnterBottom(_columnIndex);
				}
			}
		}

		public void MoveFocusDown(int amount)
		{
			if (!ChangeFocus(_columnIndex, _rowIndex + amount))
			{
				if (_rowIndex < (_rowCount - amount))
				{
					MoveFocusDown(amount + 1);
				}
				else if (NextDown && NextDown._menu.Items.Count > 0)
				{
					Leave();
					NextDown.EnterTop(_columnIndex);
				}
			}
		}

		private void MoveFocusLeft(int amount)
		{
			if (!ChangeFocus(_columnIndex - amount, _rowIndex))
			{
				if (_columnIndex > amount)
				{
					MoveFocusLeft(amount + 1);
				}
				else if (NextLeft && NextLeft._menu.Items.Count > 0)
				{
					Leave();
					NextLeft.EnterRight(_rowIndex);
				}
			}
		}

		private void MoveFocusRight(int amount)
		{
			if (!ChangeFocus(_columnIndex + amount, _rowIndex))
			{
				if (_columnIndex < (_columnCount - amount))
				{
					MoveFocusRight(amount + 1);
				}
				else if (NextRight && NextRight._menu.Items.Count > 0)
				{
					Leave();
					NextRight.EnterLeft(_rowIndex);
				}
			}
		}

		private bool MoveFocusToStart()
		{
			return ChangeFocus(0, 0);
		}

		private bool MoveFocusToEnd()
		{
			return ChangeFocus(_columnCount - 1, _rowCount - 1);
		}

		private bool MoveFocusToTop()
		{
			return ChangeFocus(_columnIndex, 0);
		}

		private bool MoveFocusToBottom()
		{
			return ChangeFocus(_columnIndex, _rowCount - 1);
		}

		private bool MoveFocusToLeft()
		{
			return ChangeFocus(0, _rowIndex);
		}

		private bool MoveFocusToRight()
		{
			return ChangeFocus(_columnCount - 1, _rowIndex);
		}

		private bool ChangeFocus(int column, int row)
		{
			var item = GetItem(column, row);

			if (item != null && item.enabled)
			{
				_columnIndex = column;
				_rowIndex = row;
				_menu.SetFocusedItem(item);

				ScrollToItem(item);

				return true;
			}

			return false;
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

			if (_shouldFocus && _menu.Items.Count > 0)
			{
				ChangeFocus(0, 0);
				_shouldFocus = false;
			}
		}

		private MenuItem GetItem(int column, int row)
		{
			if (column >= 0 && row >= 0 && column < _columnCount && row < _rowCount)
			{
				var mainIndex = PrimaryAxis == PrimaryAxis.Column ? column : row;
				var crossIndex = PrimaryAxis == PrimaryAxis.Column ? row : column;
				var crossCount = PrimaryAxis == PrimaryAxis.Column ? _rowCount : _columnCount;

				var index = mainIndex * crossCount + crossIndex;
				return index >= 0 && index < _menu.Items.Count ? _menu.Items[index] : null;
			}
			else
			{
				return null;
			}
		}

		public MenuItem GetItem(Vector2 position, out int column, out int row)
		{
			for (column = 0; column < _columnCount; column++)
			{
				for (row = 0; row < _rowCount; row++)
				{
					var item = GetItem(column, row);

					if (item?.transform is RectTransform rect)
					{
						if (rect.offsetMin.x < position.x && rect.offsetMin.y < position.y && rect.offsetMax.x > position.x && rect.offsetMax.y > position.y)
							return item;
					}
				}
			}

			row = 0;
			return null;
		}

		public void ScrollToItem(MenuItem item)
		{
			var itemRect = item.transform as RectTransform;
			var scroller = item.GetComponentInParent<ScrollRect>();

			if (scroller.horizontal)
			{
				var left = itemRect.offsetMin.x - ScrollPadding;
				var right = itemRect.offsetMax.x + ScrollPadding;

				scroller.horizontalNormalizedPosition = GetScrollPosition(scroller.horizontalNormalizedPosition, left, right, scroller.content.rect.width, scroller.viewport.rect.width);
			}

			if (scroller.vertical)
			{
				var top = itemRect.offsetMin.y - ScrollPadding;
				var bottom = itemRect.offsetMax.y + ScrollPadding;

				scroller.verticalNormalizedPosition = GetScrollPosition(scroller.verticalNormalizedPosition, top, bottom, scroller.content.rect.height, scroller.viewport.rect.height);
			}
		}

		private float GetScrollPosition(float current, float min, float max, float content, float viewport)
		{
			var size = (content - viewport);
			var visibleMin = current * size;
			var visibleMax = visibleMin + viewport;

			// do right/top first without an else so items that are too wide/tall are positioned at the top left

			if (max > visibleMax)
				current = Mathf.Clamp01((max - viewport) / size);

			if (min < visibleMin)
				current = Mathf.Clamp01(min / size);

			return current;
		}

		#endregion
	}
}
