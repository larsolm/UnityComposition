using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class BasePicker<T> : VisualElement where T : class
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Controls/Picker/BasePicker/BasePicker.uss";
		private const string _uxmlPath = Utilities.AssetPath + "Controls/Picker/BasePicker/BasePicker.uxml";

		private const string _ussBaseContainerName = "base-container";
		private const string _ussActiveContainerName = "active-container";
		private const string _ussAnimatedContainerName = "animated-container";
		private const string _ussActiveItemsName = "active-items";
		private const string _ussAnimatedItemsName = "animated-items";

		private const string _ussBaseClass = "pargon-base-picker";
		private const string _ussIconClass = "icon";
		private const string _ussLabelClass = "label";
		private const string _ussArrowClass = "arrow";
		private const string _ussItemClass = "item";
		private const string _ussContainerClass = "container";

		private static readonly Icon _folderIcon = Icon.BuiltIn("FolderEmpty Icon");
		private static readonly Icon _defaultTypeIcon = Icon.BuiltIn("cs Script Icon");

		private class TreeItem
		{
			public T Item;

			public int ItemIndex;
			public int BranchIndex;
			public string Path;
			public string SearchName;
			public string Label;

			public Texture Icon;

			public TreeItem Parent;
			public List<TreeItem> Children;

			public VisualElement Button;
			public int SelectedIndex = -1;

			public bool IsBranch => Children != null;
			public TreeItem SelectedItem => IsBranch && SelectedIndex >= 0 && SelectedIndex < Children.Count ? Children[SelectedIndex] : null;

			public static TreeItem Leaf(T item, int index, string path, string label, Texture icon, TreeItem parent)
			{
				var leaf = new TreeItem
				{
					Item = item,
					ItemIndex = index,
					Path = path,
					SearchName = label.Replace(" ", string.Empty).ToLower(),
					Label = label,
					Icon = icon,
					Parent = parent,
				};

				if (leaf.Parent != null)
					leaf.Parent.Children.Add(leaf);

				return leaf;
			}

			public static TreeItem Branch(string path, string label, TreeItem parent)
			{
				var item = Leaf(null, -1, path, label, _folderIcon.Content, parent);
				item.Children = new List<TreeItem>();
				return item;
			}
		}

		public Action<T> OnSelected;

		private readonly VisualElement _baseContainer;
		private readonly VisualElement _activeContainer;
		private readonly VisualElement _animatedContainer;
		private readonly Image _activeHeaderIcon;
		private readonly Label _activeHeaderLabel;
		private readonly Image _animatedHeaderIcon;
		private readonly Label _animatedHeaderLabel;
		private readonly Image _activeParentButton;
		private readonly Image _animatedParentButton;
		private readonly ScrollView _activeView;
		private readonly ScrollView _animatedView;
		private readonly SearchBar _search;

		private VisualElement _currentItem;

		private TreeItem _activeBranch;
		private TreeItem _animatedBranch;
		private TreeItem _searchBranch;

		private bool _isAnimating = false;

		private TreeItem _currentBranch => string.IsNullOrEmpty(_search.Text) ? _activeBranch : _searchBranch;
		private TreeItem _root;
		private List<TreeItem> _searchList = new List<TreeItem>();

		public BasePicker()
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			ElementHelper.AddVisualTree(this, _uxmlPath);

			focusable = true;

			AddToClassList(_ussBaseClass);

			_baseContainer = this.Q(_ussBaseContainerName);
			_activeContainer = _baseContainer.Q(_ussActiveContainerName);
			_animatedContainer = _baseContainer.Q(_ussAnimatedContainerName);
			_activeHeaderIcon = _activeContainer.Q<Image>(className: _ussIconClass);
			_activeHeaderLabel = _activeContainer.Q<Label>(className: _ussLabelClass);
			_animatedHeaderIcon = _animatedContainer.Q<Image>(className: _ussIconClass);
			_animatedHeaderLabel = _animatedContainer.Q<Label>(className: _ussLabelClass);
			_activeParentButton = _activeContainer.Q<Image>(className: _ussArrowClass);
			_activeParentButton.image = Icon.LeftArrow.Content;
			_activeParentButton.AddManipulator(new Clickable(GoToParent));
			_animatedParentButton = _animatedContainer.Q<Image>(className: _ussArrowClass);
			_animatedParentButton.image = Icon.LeftArrow.Content;
			_activeView = _activeContainer.Q<ScrollView>(_ussActiveItemsName);
			_activeView.verticalScroller.slider.focusable = false;
			_animatedView = _animatedContainer.Q<ScrollView>(_ussAnimatedItemsName);
			_search = this.Q<SearchBar>();
			_search.RegisterCallback<ChangeEvent<string>>(evt => RebuildSearch(evt.newValue));
			_search.RegisterCallback<KeyDownEvent>(e => HandleKeyboard(e, true), TrickleDown.TrickleDown);

			ElementHelper.SetVisible(_animatedContainer, false);

			FocusSearch();
		}

		public void FocusSearch()
		{
			_search.FocusText();
		}

		#region Initializiation

		protected void CreateTree(string title, List<string> paths, List<T> items, T initialValue, Func<T, Texture> getIcon)
		{
			var menus = paths.Prepend("None").ToList();
			var values = items.Prepend(null).ToList();

			_root = TreeItem.Branch(string.Empty, title, null);
			_activeBranch = _root;
			_searchList = new List<TreeItem>();

			var selectedIndex = 0;
			var rootPath = title + "/";

			for (var index = 0; index < menus.Count; index++)
			{
				var node = menus[index];
				var item = values[index];
				var fullPath = rootPath + node;
				var submenus = fullPath.Split('/');
				var icon = getIcon(item);

				var path = rootPath;
				var child = _root;

				for (var i = 1; i < submenus.Length - 1; i++)
				{
					var menu = submenus[i];
					path += menu + "/";

					var previousChild = child;
					child = GetChild(child, path);

					if (child == null)
						child = TreeItem.Branch(path, menu, previousChild);
				}

				_searchList.Add(TreeItem.Leaf(item, index, path, submenus.Last(), icon == null ? _defaultTypeIcon.Content : icon, child));

				if (item == initialValue)
					selectedIndex = index;
			}

			RebuildSearch(_search.Text);
			SetSelectedIndex(selectedIndex);
		}

		private TreeItem GetChild(TreeItem parent, string path)
		{
			if (parent.Path == path)
				return parent;

			if (parent.IsBranch)
			{
				foreach (var child in parent.Children)
				{
					var found = GetChild(child, path);
					if (found != null)
						return found;
				}
			}

			return null;
		}

		private void RebuildBranch(Image parentButton, Label header, Image headerIcon, ScrollView container, TreeItem parent)
		{
			ElementHelper.SetVisible(parentButton, parent.Parent != null);

			header.text = parent.Label;
			headerIcon.image = parent.Icon;

			container.Clear();

			for (var i = 0; i < parent.Children.Count; i++)
			{
				var child = parent.Children[i];
				child.BranchIndex = i;
				child.Button = new VisualElement { userData = child };
				child.Button.AddManipulator(new Clickable(() => SelectChild(child.Button.userData as TreeItem)));
				child.Button.AddToClassList(_ussItemClass);
				child.Button.RegisterCallback<MouseMoveEvent>(e => ItemFocused(child.Button));

				var labelContainer = new VisualElement { pickingMode = PickingMode.Ignore };
				labelContainer.AddToClassList(_ussContainerClass);
				child.Button.Add(labelContainer);

				var icon = new Image { image = child.Icon, pickingMode = PickingMode.Ignore };
				icon.AddToClassList(_ussIconClass);
				labelContainer.Add(icon);

				var label = new Label(child.Label) { pickingMode = PickingMode.Ignore };
				label.AddToClassList(_ussLabelClass);
				labelContainer.Add(label);

				if (child.IsBranch)
				{
					var arrow = new Image { image = Icon.RightArrow.Content, pickingMode = PickingMode.Ignore };
					arrow.AddToClassList(_ussArrowClass);
					child.Button.Add(arrow);
				}

				container.Add(child.Button);
			}
		}

		private void RebuildSearch(string newValue)
		{
			_searchBranch = TreeItem.Branch("Search", "Search", null);
			_searchBranch.SelectedIndex = -1;

			if (string.IsNullOrEmpty(newValue))
			{
				RebuildBranch(_activeParentButton, _activeHeaderLabel, _activeHeaderIcon, _activeView, _activeBranch);
				SetSelectedIndex(_activeBranch.SelectedIndex);
			}
			else
			{
				var subwords = newValue.ToLower().Split(' ').Where(subword => !string.IsNullOrEmpty(subword)).ToArray();
				var starts = new List<TreeItem>();
				var contains = new List<TreeItem>();

				foreach (var item in _searchList)
				{
					if (subwords.Length > 0 && item.SearchName.StartsWith(subwords.First()))
					{
						starts.Add(item);
					}
					else
					{
						foreach (var subword in subwords)
						{
							if (item.SearchName.Contains(subword))
								contains.Add(item);
						}
					}
				}

				_searchBranch.Children.AddRange(starts);
				_searchBranch.Children.AddRange(contains);

				RebuildBranch(_activeParentButton, _activeHeaderLabel, _activeHeaderIcon, _activeView, _searchBranch);
			}
		}

		#endregion

		#region Input

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);

			if (evt.eventTypeId == KeyDownEvent.TypeId())
			{
				HandleKeyboard(evt as KeyDownEvent, false);
				evt.StopPropagation();
			}
		}

		private void HandleKeyboard(KeyDownEvent evt, bool fromSearch)
		{
			var key = evt.keyCode;
			if (key == KeyCode.DownArrow || key == KeyCode.UpArrow || key == KeyCode.Return || key == KeyCode.KeypadEnter)
			{
				Focus();

				evt.PreventDefault();
				evt.StopPropagation();

				if (key == KeyCode.DownArrow)
					SetSelectedIndex(_currentBranch.SelectedIndex + 1);
				else if (key == KeyCode.UpArrow)
					SetSelectedIndex(_currentBranch.SelectedIndex - 1);
				else if (key == KeyCode.Return || key == KeyCode.KeypadEnter)
					SelectChild(_currentBranch.SelectedItem);
			}

			if (!fromSearch)
			{
				if (key == KeyCode.LeftArrow)
					GoToParent();
				else if (key == KeyCode.RightArrow)
					SelectChild(_currentBranch.SelectedItem);
			}
		}

		private void SetSelectedIndex(int index)
		{
			_currentBranch.SelectedIndex = Mathf.Clamp(index, 0, _currentBranch.Children.Count - 1);

			if (_currentBranch.SelectedItem != null)
				ItemFocused(_currentBranch.SelectedItem.Button);
		}

		private void ItemFocused(VisualElement element)
		{
			if (_currentItem != null)
				_currentItem.RemoveFromClassList("focused");

			element.AddToClassList("focused");

			_currentItem = element;

			var item = element.userData as TreeItem;
			_currentBranch.SelectedIndex = item.BranchIndex;

			if (_activeView.contentContainer.Children().Contains(element))
				_activeView.ScrollTo(element);
		}

		#endregion

		#region Animation

		private void GoToParent()
		{
			if (_activeBranch.Parent != null)
				AnimatePrevious(_activeBranch.Parent);
		}

		private void SelectChild(TreeItem item)
		{
			if (item != null)
			{
				if (item.IsBranch)
					AnimateNext(item);
				else
					OnSelected?.Invoke(item.Item);
			}
		}

		private void AnimatePrevious(TreeItem target)
		{
			StartAnimation(target, 1);
		}

		private void AnimateNext(TreeItem target)
		{
			StartAnimation(target, -1);
		}

		private void StartAnimation(TreeItem target, float direction)
		{
			if (!_isAnimating && string.IsNullOrEmpty(_search.Text))
			{
				_isAnimating = true;
				_animatedBranch = _activeBranch;
				_activeBranch = target;

				RebuildBranch(_activeParentButton, _activeHeaderLabel, _activeHeaderIcon, _activeView, _activeBranch);
				RebuildBranch(_animatedParentButton, _animatedHeaderLabel, _animatedHeaderIcon, _animatedView, _animatedBranch);
				SetSelectedIndex(_activeBranch.SelectedIndex);

				ElementHelper.SetVisible(_animatedContainer, true);

				var progress = 0.0f;

				schedule.Execute(() =>
				{
					progress = Mathf.MoveTowards(progress, direction, 0.075f);

					var width = _baseContainer.layout.width;
					var offset = progress * width;

					_activeContainer.style.left = new StyleLength(offset - (width * direction));
					_activeContainer.style.right = new StyleLength((width * direction) - offset);

					_animatedContainer.style.left = new StyleLength(offset);
					_animatedContainer.style.right = new StyleLength(-offset);

					if (progress == direction)
					{
						ElementHelper.SetVisible(_animatedContainer, false);
						_isAnimating = false;
					}

				}).Every(0).Until(() => !_isAnimating);
			}
		}

		#endregion
	}
}
