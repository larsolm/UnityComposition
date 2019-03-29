using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "scroll-selection")]
	[AddComponentMenu("PiRho Soft/Interface/Scroll Selection")]
	public class ScrollSelection : InputSelection
	{
		[Tooltip("The speed of which to scroll the content")] public float ScrollSpeed = -1;
		[Tooltip("The number of items displayed vertically before scrolling happens")] public int DisplayedVertical = 1;
		[Tooltip("The speed of items displayed horizontalyl before scrolling happens")] public int DisplayedHorizontal = 5;

		private ScrollRect _scroll;

		private int _rowFocus = 0;
		private int _columnFocus = 0;

		private Vector2 _targetPosition;

		protected override void Setup()
		{
			_scroll = GetComponent<ScrollRect>();
			_targetPosition = Vector2.zero;
		}

		protected override Transform GetItemParent()
		{
			return _scroll.content;
		}

		protected override void MoveFocusUp(int amount = 1)
		{
			var count = Mathf.Min(_rowCount, DisplayedVertical);

			if (_rowFocus > 1)
				_rowFocus--;

			base.MoveFocusUp();

			if (_rowIndex == 0)
				_rowFocus = 0;

			if (_rowIndex == _rowCount - 1)
				_rowFocus = count - 1;

			UpdateTargetPosition();
		}

		protected override void MoveFocusDown(int amount = 1)
		{
			var count = Mathf.Min(_rowCount, DisplayedVertical);

			if (_rowFocus < count - 2)
				_rowFocus++;

			base.MoveFocusDown();
	
			if (_rowIndex == _rowCount - 1)
				_rowFocus = count - 1;

			if (_rowFocus > _rowIndex)
				_rowFocus = _rowIndex;

			UpdateTargetPosition();
		}

		protected override void MoveFocusLeft(int amount = 1)
		{
			var count = Mathf.Min(_columnCount, DisplayedHorizontal);

			if (_columnFocus > 1)
				_columnFocus--;

			base.MoveFocusLeft();

			if (_columnIndex == 0)
				_columnFocus = 0;

			if (_columnIndex == _columnCount - 1)
				_columnFocus = count - 1;

			UpdateTargetPosition();
		}

		protected override void MoveFocusRight(int amount = 1)
		{
			var count = Mathf.Min(_columnCount, DisplayedHorizontal);

			if (_columnFocus < count - 2)
				_columnFocus++;

			base.MoveFocusRight();

			if (_columnIndex == _columnCount - 1)
				_columnFocus = count - 1;

			if (_columnFocus > _columnIndex)
				_columnFocus = _columnIndex;

			UpdateTargetPosition();
		}

		private void UpdateTargetPosition()
		{
			var xDiff = _columnCount - DisplayedHorizontal;
			var yDiff = _rowCount - DisplayedVertical;

			var x = xDiff >= 0 ? 1 - ((_columnIndex - _columnFocus) / (float)xDiff) : 0.0f;
			var y = yDiff >= 0 ? 1 - ((_rowIndex - _rowFocus) / (float)yDiff) : 0.0f;

			_scroll.horizontalNormalizedPosition = x;
			_scroll.verticalNormalizedPosition = y;
		}
	}
}
