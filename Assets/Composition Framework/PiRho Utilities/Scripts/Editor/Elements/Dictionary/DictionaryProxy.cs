using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	public interface IDictionaryProxy
	{
		string Label { get; }
		string Tooltip { get; }
		string EmptyLabel { get; }
		string EmptyTooltip { get; }
		string AddTooltip { get; }
		string RemoveTooltip { get; }

		int ItemCount { get; }
		bool AllowAdd { get; }
		bool AllowRemove { get; }

		VisualElement CreateField(int index);
		bool NeedsUpdate(VisualElement item, int index);
		void AddItem(string key);
		void RemoveItem(int index);
	}

	public abstract class DictionaryProxy : IDictionaryProxy
	{
		public const string DefaultEmptyLabel = "The dictionary is empty";
		public const string DefaultAddTooltip = "Add an item to this dictionary";
		public const string DefaultRemoveTooltip = "Remove this item from the dictionary";

		public string Label { get; set; }
		public string Tooltip { get; set; }
		public string EmptyLabel { get; set; } = DefaultEmptyLabel;
		public string EmptyTooltip { get; set; }
		public string AddTooltip { get; set; } = DefaultAddTooltip;
		public string RemoveTooltip { get; set; } = DefaultRemoveTooltip;

		public abstract int ItemCount { get; }
		public virtual bool AllowAdd { get; set; } = true;
		public virtual bool AllowRemove { get; set; } = true;

		public abstract VisualElement CreateField(int index);
		public abstract bool NeedsUpdate(VisualElement item, int index);
		public abstract void AddItem(string key);
		public abstract void RemoveItem(int index);
	}
}