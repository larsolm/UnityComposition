using System;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "menu")]
	[AddComponentMenu("PiRho Soft/Interface/Menu")]
	public class Menu : MonoBehaviour
	{
		public Action<MenuItem> OnItemAdded;
		public Action<MenuItem> OnItemRemoved;
		public Action<MenuItem, int> OnItemMoved;
		public Action<MenuItem> OnItemBlurred;
		public Action<MenuItem> OnItemFocused;
		public Action<MenuItem> OnItemSelected;
		public Action OnCancelled;

		public List<MenuItem> Items { get; } = new List<MenuItem>();
		public MenuItem FocusedItem { get; private set; }
		public int FocusedIndex { get; private set; }

		public void SetFocusedItem(MenuItem item)
		{
			if (FocusedItem)
				FocusedItem.Focused = false;

			var from = FocusedItem;
			FocusedItem = item;
			FocusedIndex = item ? item.Index : -1;

			if (FocusedItem)
				FocusedItem.Focused = true;

			if (from)
				ItemBlurred(from);

			if (item)
				ItemFocused(item);
		}

		public void SelectFocusedItem()
		{
			if (FocusedItem)
				ItemSelected(FocusedItem);
		}

		public void Cancel()
		{
			Cancelled();
		}

		#region Item Maintenance

		private class SiblingIndexComparer : IComparer<MenuItem>
		{
			public int Compare(MenuItem x, MenuItem y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex());
		}

		private static SiblingIndexComparer _comparer = new SiblingIndexComparer();

		internal void AddItem(MenuItem item)
		{
			Items.Add(item);

			RefreshItems();
			ItemAdded(item);
		}

		internal void RemoveItem(MenuItem item)
		{
			Items.Remove(item);

			RefreshItems();
			ItemRemoved(item);
		}

		internal void MoveItem(MenuItem item)
		{
			var from = Items.IndexOf(item);
			RefreshItems();
			var to = Items.IndexOf(item);

			if (from != to)
				ItemMoved(item, from);
		}

		private void RefreshItems()
		{
			Items.Sort(_comparer);

			for (var i = 0; i < Items.Count; i++)
				RefreshItem(Items[i], i);
		}

		private void RefreshItem(MenuItem item, int index)
		{
			item.Index = index;
		}

		#endregion

		#region Virtual Interface

		protected virtual void ItemAdded(MenuItem item)
		{
			OnItemAdded?.Invoke(item);
		}

		protected virtual void ItemRemoved(MenuItem item)
		{
			OnItemRemoved?.Invoke(item);
		}

		protected virtual void ItemMoved(MenuItem item, int from)
		{
			OnItemMoved?.Invoke(item, from);
		}
		
		protected virtual void ItemFocused(MenuItem item)
		{
			OnItemFocused?.Invoke(item);
		}

		protected virtual void ItemBlurred(MenuItem item)
		{
			OnItemBlurred?.Invoke(item);
		}

		protected virtual void ItemSelected(MenuItem item)
		{
			OnItemSelected?.Invoke(item);
		}

		protected virtual void Cancelled()
		{
			OnCancelled?.Invoke();
		}

		#endregion
	}
}
