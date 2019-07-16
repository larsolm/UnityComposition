using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "menu")]
	[AddComponentMenu("PiRho Soft/Interface/Menu")]
	public class Menu : MonoBehaviour, IVariableStore
	{
		public Action<MenuItem> OnItemBlurred;
		public Action<MenuItem> OnItemFocused;
		public Action<MenuItem> OnItemSelected;
		public Action OnCancelled;
		public Action OnItemsChanged;

		public List<MenuItem> Items { get; } = new List<MenuItem>();

		private MenuItem _focusedItem;
		private int _focusedIndex = -1;
		private int _removedFocus = -1;
		private bool _itemsDirty = false;

		public bool AcceptsInput { get; set; } = true;
		public MenuItem FocusedItem { get => _focusedItem; set => SetFocusedItem(value); }
		public int FocusedIndex { get => _focusedItem != null ? _focusedItem.Index : -1; set => SetFocusedIndex(value); }

		private void SetFocusedItem(MenuItem item)
		{
			if (_focusedItem)
				_focusedItem.Focused = false;

			var from = _focusedItem;
			_focusedItem = item;
			_focusedIndex = item ? item.Index : -1;
			_removedFocus = -1;

			if (_focusedItem)
				_focusedItem.Focused = true;

			if (from)
				ItemBlurred(from);

			if (item)
				ItemFocused(item);
		}

		private void SetFocusedIndex(int index)
		{
			SetFocusedItem(index >= 0 && index < Items.Count ? Items[index] : null);
		}

		public void SelectItem(MenuItem item)
		{
			if (item)
				ItemSelected(item);
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
			SetItemsDirty();
		}

		internal void RemoveItem(MenuItem item)
		{
			Items.Remove(item);
			SetItemsDirty();

			if (item.Focused)
				_removedFocus = item.Index;
		}

		internal void MoveItem(MenuItem item)
		{
			SetItemsDirty();
		}

		private static WaitForEndOfFrame _refreshWait = new WaitForEndOfFrame();

		private IEnumerator RefreshItems()
		{
			yield return _refreshWait;

			Items.Sort(_comparer);

			for (var i = 0; i < Items.Count; i++)
				Items[i].Index = i;

			if (_removedFocus >= 0)
			{
				SetFocusedIndex(_removedFocus >= Items.Count ? Items.Count - 1 : _removedFocus);
				_removedFocus = -1;
			}

			_itemsDirty = false;
			OnItemsChanged?.Invoke();
		}

		private void SetItemsDirty()
		{
			if (!_itemsDirty && gameObject.activeInHierarchy)
			{
				StartCoroutine(RefreshItems());
				_itemsDirty = true;
			}
		}

		#endregion

		#region Virtual Interface

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

		#region IVariableStore Implementation

		private static string[] _variableNames = new string[] { nameof(FocusedItem), nameof(FocusedIndex) };

		public IList<string> GetVariableNames()
		{
			return _variableNames;
		}

		public VariableValue GetVariable(string name)
		{
			switch (name)
			{
				case nameof(FocusedItem): return VariableValue.Create((Object)FocusedItem);
				case nameof(FocusedIndex): return VariableValue.Create(FocusedIndex);
				default: return VariableValue.Empty;
			}
		}

		public SetVariableResult SetVariable(string name, VariableValue value)
		{
			switch (name)
			{
				case nameof(FocusedItem):
				{
					if (value.TryGetReference(out MenuItem item))
					{
						FocusedItem = item;
						return SetVariableResult.Success;
					}

					return SetVariableResult.TypeMismatch;
				}
				case nameof(FocusedIndex):
				{
					if (value.TryGetInt(out var index))
					{
						FocusedIndex = index;
						return SetVariableResult.Success;
					}

					return SetVariableResult.TypeMismatch;
				}
				default: return SetVariableResult.NotFound;
			}
		}

		#endregion
	}
}
