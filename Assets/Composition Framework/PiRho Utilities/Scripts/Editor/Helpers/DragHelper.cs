using UnityEditor;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Utilities.Editor
{
	public enum DragState
	{
		Idle,
		Ready,
		Dragging
	}

	public interface IDraggable
	{
		DragState DragState { get; set; }
		string DragText { get; }
		Object[] DragObjects { get; }
	}

	public static class DragHelper
	{
		public static void MakeDraggable<Draggable>(Draggable draggable) where Draggable : VisualElement, IDraggable
		{
			draggable.DragState = DragState.Idle;
			draggable.RegisterCallback<MouseDownEvent>(OnMouseDown);
			draggable.RegisterCallback<MouseMoveEvent>(OnMouseMove);
			draggable.RegisterCallback<MouseUpEvent>(OnMouseUp);
		}

		private static void OnMouseDown(MouseDownEvent evt)
		{
			if (evt.target is IDraggable draggable && evt.button == (int)MouseButton.LeftMouse)
				draggable.DragState = DragState.Ready;
		}

		private static void OnMouseMove(MouseMoveEvent evt)
		{
			if (evt.target is IDraggable draggable && draggable.DragState == DragState.Ready)
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = draggable.DragObjects;
				DragAndDrop.StartDrag(draggable.DragText);

				draggable.DragState = DragState.Dragging;
			}
		}

		private static void OnMouseUp(MouseUpEvent evt)
		{
			if (evt.target is IDraggable draggable && draggable.DragState == DragState.Dragging && evt.button == (int)MouseButton.LeftMouse)
				draggable.DragState = DragState.Idle;
		}
	}
}
