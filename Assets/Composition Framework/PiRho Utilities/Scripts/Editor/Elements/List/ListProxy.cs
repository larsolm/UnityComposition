using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IListProxy
	{
		string Label { get; }
		string Tooltip { get; }
		string EmptyLabel { get; }
		string EmptyTooltip { get; }
		string AddTooltip { get; }
		string RemoveTooltip { get; }
		string ReorderTooltip { get; }

		int ItemCount { get; }
		bool AllowAdd { get; }
		bool AllowRemove { get; }
		bool AllowReorder { get; }

		VisualElement CreateElement(int index);
		bool NeedsUpdate(VisualElement item, int index);
		void AddItem();
		void RemoveItem(int index);
		void ReorderItem(int from, int to);
	}

	public abstract class ListProxy : IListProxy
	{
		public const string DefaultEmptyLabel = "The list is empty";
		public const string DefaultAddTooltip = "Add an item to this list";
		public const string DefaultRemoveTooltip = "Remove this item from the list";
		public const string DefaultReorderTooltip = "Move this item within the list";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = DefaultEmptyLabel;
		public string EmptyTooltip { get; set; }
		public string AddTooltip { get; set; } = DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DefaultRemoveTooltip;
		public string ReorderTooltip { get; set; } = DefaultReorderTooltip;

		public abstract int ItemCount { get; }
		public virtual bool AllowAdd { get; set; } = true;
		public virtual bool AllowRemove { get; set; } = true;
		public virtual bool AllowReorder { get; set; } = true;

		public abstract VisualElement CreateElement(int index);
		public abstract bool NeedsUpdate(VisualElement item, int index);
		public abstract void AddItem();
		public abstract void RemoveItem(int index);
		public abstract void ReorderItem(int from, int to);
	}
}