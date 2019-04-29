using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	public enum PrimaryAxis
	{
		Column,
		Row
	}

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
			HandleButtons();
			HandlePointer();
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

		private void OnItemMoved(MenuItem item)
		{
			RefreshLayout();
		}

		#endregion

		#region Input

		private void HandleButtons()
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
		}

		private void HandlePointer()
		{
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

		private void FocusWithMouse()
		{
			var item = GetItem(Input.mousePosition);

			if (item != null)
			{
				ChangeFocus(item.Column, item.Row);

				if (NextLeft) NextLeft.Leave();
				if (NextRight) NextRight.Leave();
				if (NextUp) NextUp.Leave();
				if (NextDown) NextDown.Leave();
			}
		}

		private void SelectWithMouse()
		{
			var item = GetItem(Input.mousePosition);

			if (item != null)
				_menu.SelectItem(item);
		}

		#endregion

		#region Focus

		public void EnterFromBeginning()
		{
			if (_menu.Items.Count > 0)
				ChangeFocus(_menu.Items[0]);
		}

		public void EnterFromEnd()
		{
			if (_menu.Items.Count > 0)
				ChangeFocus(_menu.Items[_menu.Items.Count - 1]);
		}

		public void EnterFromLeft(int fromRow)
		{
			var item = GetItem(0, fromRow);

			if (item != null)
				ChangeFocus(item);
			else
				EnterFromEnd();
		}

		public void EnterFromRight(int fromRow)
		{
			var item = GetItem(_columnCount - 1, fromRow);

			if (item != null)
				ChangeFocus(item);
			else
				EnterFromEnd();
		}

		public void EnterFromTop(int fromColumn)
		{
			var item = GetItem(fromColumn, 0);

			if (item != null)
				ChangeFocus(item);
			else
				EnterFromEnd();
		}

		public void EnterFromBottom(int fromColumn)
		{
			var item = GetItem(fromColumn, _rowCount - 1);

			if (item != null)
				ChangeFocus(item);
			else
				EnterFromEnd();
		}

		public void Leave()
		{
			_menu.FocusedItem = null;
		}

		public void MoveFocusUp(int amount)
		{
			if (_menu.FocusedItem == null)
			{
				EnterFromEnd();
			}
			else if (!ChangeFocus(_menu.FocusedItem.Column, _menu.FocusedItem.Row - amount))
			{
				if (_menu.FocusedItem.Row > amount)
				{
					MoveFocusUp(amount + 1);
				}
				else if (NextUp && NextUp._menu.Items.Count > 0)
				{
					NextUp.EnterFromBottom(_menu.FocusedItem.Column);
					Leave();
				}
			}
		}

		public void MoveFocusDown(int amount)
		{
			if (_menu.FocusedItem == null)
			{
				EnterFromBeginning();
			}
			else if (!ChangeFocus(_menu.FocusedItem.Column, _menu.FocusedItem.Row + amount))
			{
				if (_menu.FocusedItem.Row < (_rowCount - amount))
				{
					MoveFocusDown(amount + 1);
				}
				else if (NextDown && NextDown._menu.Items.Count > 0)
				{
					NextDown.EnterFromTop(_menu.FocusedItem.Column);
					Leave();
				}
			}
		}

		public void MoveFocusLeft(int amount)
		{
			if (_menu.FocusedItem == null)
			{
				EnterFromEnd();
			}
			else if (!ChangeFocus(_menu.FocusedItem.Column - amount, _menu.FocusedItem.Row))
			{
				if (_menu.FocusedItem.Column > amount)
				{
					MoveFocusLeft(amount + 1);
				}
				else if (NextLeft && NextLeft._menu.Items.Count > 0)
				{
					NextLeft.EnterFromRight(_menu.FocusedItem.Row);
					Leave();
				}
			}
		}

		public void MoveFocusRight(int amount)
		{
			if (_menu.FocusedItem == null)
			{
				EnterFromBeginning();
			}
			else if (!ChangeFocus(_menu.FocusedItem.Column + amount, _menu.FocusedItem.Row))
			{
				if (_menu.FocusedItem.Column < (_columnCount - amount))
				{
					MoveFocusRight(amount + 1);
				}
				else if (NextRight && NextRight._menu.Items.Count > 0)
				{
					NextRight.EnterFromLeft(_menu.FocusedItem.Row);
					Leave();
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
			return ChangeFocus(_menu.FocusedItem.Column, 0);
		}

		private bool MoveFocusToBottom()
		{
			return ChangeFocus(_menu.FocusedItem.Column, _rowCount - 1);
		}

		private bool MoveFocusToLeft()
		{
			return ChangeFocus(0, _menu.FocusedItem.Row);
		}

		private bool MoveFocusToRight()
		{
			return ChangeFocus(_columnCount - 1, _menu.FocusedItem.Row);
		}

		private bool ChangeFocus(int column, int row)
		{
			var item = GetItem(column, row);
			return ChangeFocus(item);
		}

		private bool ChangeFocus(MenuItem item)
		{
			if (item != null && item.enabled)
			{
				_menu.FocusedItem = item;
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

			for (var column = 0; column < _columnCount; column++)
			{
				for (var row = 0; row < _rowCount; row++)
				{
					var item = GetItem(column, row);
					if (item)
					{
						item.Column = column;
						item.Row = row;
					}
				}
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

		public MenuItem GetItem(Vector2 screenPoint)
		{
			var canvas = GetComponentInParent<Canvas>();
			var camera = canvas != null ? canvas.worldCamera : null;

			for (var column = 0; column < _columnCount; column++)
			{
				for (var row = 0; row < _rowCount; row++)
				{
					var item = GetItem(column, row);

					if (item?.transform is RectTransform rect)
					{
						if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, camera))
							return item;
					}
				}
			}

			return null;
		}

		public void ScrollToItem(MenuItem item)
		{
			var itemRect = item.transform as RectTransform;
			var scroller = item.GetComponentInParent<ScrollRect>();

			if (scroller && scroller.horizontal)
			{
				var left = itemRect.offsetMin.x - ScrollPadding;
				var right = itemRect.offsetMax.x + ScrollPadding;

				scroller.horizontalNormalizedPosition = GetScrollPosition(scroller.horizontalNormalizedPosition, left, right, scroller.content.rect.width, scroller.viewport.rect.width);
			}

			if (scroller && scroller.vertical)
			{
				var bottom = Math.Abs(itemRect.offsetMin.y - ScrollPadding);
				var top = Math.Abs(itemRect.offsetMax.y + ScrollPadding);

				scroller.verticalNormalizedPosition = 1.0f - GetScrollPosition(1.0f - scroller.verticalNormalizedPosition, top, bottom, scroller.content.rect.height, scroller.viewport.rect.height);
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
