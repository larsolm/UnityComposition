using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	public abstract class BasePicker<T> : VisualElement where T : class
	{
		private const string _styleSheetPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Picker/BasePicker/BasePicker.uss";
		private const string _uxmlPath = "Assets/PargonUtilities/Scripts/Editor/Controls/Picker/BasePicker/BasePicker.uxml";

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

			public Button Button;
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

		private VisualElement _baseContainer;
		private VisualElement _activeContainer;
		private VisualElement _animatedContainer;
		private Image _activeHeaderIcon;
		private Label _activeHeaderLabel;
		private Image _animatedHeaderIcon;
		private Label _animatedHeaderLabel;
		private Image _activeParentButton;
		private Image _animatedParentButton;
		private ScrollView _activeView;
		private ScrollView _animatedView;
		private ToolbarSearchField _search;
		private VisualElement _searchText;

		private Button _currentItem;

		private TreeItem _activeBranch;
		private TreeItem _animatedBranch;
		private TreeItem _searchBranch;

		private bool _isAnimating = false;

		private TreeItem _currentBranch => string.IsNullOrEmpty(_search.value) ? _activeBranch : _searchBranch;
		private TreeItem _root;
		private List<TreeItem> _searchList = new List<TreeItem>();

		public BasePicker()
		{
			ElementHelper.AddStyleSheet(this, _styleSheetPath);
			ElementHelper.AddVisualTree(this, _uxmlPath);

			focusable = true;

			AddToClassList("base-picker");
			_baseContainer = this.Query(className: "base-container");
			_activeContainer = _baseContainer.Query(name: "active-container");
			_animatedContainer = _baseContainer.Query(name: "animated-container");
			_activeHeaderIcon = _activeContainer.Query<Image>(className: "icon");
			_activeHeaderLabel = _activeContainer.Query<Label>(className: "label");
			_animatedHeaderIcon = _animatedContainer.Query<Image>(className: "icon");
			_animatedHeaderLabel = _animatedContainer.Query<Label>(className: "label");
			_activeParentButton = _activeContainer.Query<Image>(className: "arrow");
			_activeParentButton.image = Icon.LeftArrow.Content;
			_activeParentButton.AddManipulator(new Clickable(GoToParent));
			_animatedParentButton = _animatedContainer.Query<Image>(className: "arrow");
			_animatedParentButton.image = Icon.LeftArrow.Content;
			_activeView = _activeContainer.Query<ScrollView>(name: "active-items");
			_activeView.verticalScroller.slider.focusable = false;
			_animatedView = _animatedContainer.Query<ScrollView>(name: "animated-items");
			_search = this.Query<ToolbarSearchField>();
			_search.RegisterValueChangedCallback(e => RebuildSearch(e.newValue));
			_search.RegisterCallback<KeyDownEvent>(e => HandleKeyboard(e, true), TrickleDown.TrickleDown);
			_searchText = _search.Query<TextField>().First().Children().First();
		}

		public void FocusSearch()
		{
			_searchText.Focus();
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

			RebuildSearch(_search.value);
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
			if (parent.Parent != null)
				parentButton.RemoveFromClassList("hidden");
			else
				parentButton.AddToClassList("hidden");

			header.text = parent.Label;
			headerIcon.image = parent.Icon;

			container.Clear();

			for (var i = 0; i < parent.Children.Count; i++)
			{
				var child = parent.Children[i];
				child.BranchIndex = i;
				child.Button = new Button { userData = child };
				child.Button.clickable.clicked += () => SelectChild(child.Button.userData as TreeItem);
				child.Button.AddToClassList("item");
				child.Button.RegisterCallback<MouseMoveEvent>(e => ButtonFocused(child.Button));

				var labelContainer = new VisualElement();
				labelContainer.AddToClassList("container");
				child.Button.Add(labelContainer);

				var icon = new Image { image = child.Icon };
				icon.AddToClassList("icon");
				labelContainer.Add(icon);

				var label = new Label(child.Label);
				label.AddToClassList("label");
				labelContainer.Add(label);

				if (child.IsBranch)
				{
					var arrow = new Image { image = Icon.RightArrow.Content };
					arrow.AddToClassList("arrow");
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
				ButtonFocused(_currentBranch.SelectedItem.Button);
		}

		private void ButtonFocused(Button button)
		{
			if (_currentItem != null)
				_currentItem.RemoveFromClassList("focused");

			button.AddToClassList("focused");

			_currentItem = button;

			var item = button.userData as TreeItem;
			_currentBranch.SelectedIndex = item.BranchIndex;

			if (_activeView.contentContainer.Children().Contains(button))
				_activeView.ScrollTo(button);
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
			if (!_isAnimating && string.IsNullOrEmpty(_search.value))
			{
				_isAnimating = true;
				_animatedBranch = _activeBranch;
				_activeBranch = target;

				RebuildBranch(_activeParentButton, _activeHeaderLabel, _activeHeaderIcon, _activeView, _activeBranch);
				RebuildBranch(_animatedParentButton, _animatedHeaderLabel, _animatedHeaderIcon, _animatedView, _animatedBranch);
				SetSelectedIndex(_activeBranch.SelectedIndex);

				_animatedContainer.RemoveFromClassList("hidden");

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
						_animatedContainer.AddToClassList("hidden");
						_isAnimating = false;
					}

				}).Every(0).Until(() => !_isAnimating);
			}
		}

		#endregion
	}
}
