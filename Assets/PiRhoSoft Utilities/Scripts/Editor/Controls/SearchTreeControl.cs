using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	// Adapted from Unity's AddComponentWindow as linked to in the following thread:
	// https://forum.unity.com/threads/custom-add-component-like-button.439730/

	public class SearchTreeControl : EditorWindow
	{
		private class TreeItem
		{
			public int ItemIndex;
			public string Path;
			public string SearchName;
			public string Name;
			public GUIContent Content;

			public TreeItem Parent;
			public List<TreeItem> Children;

			public int SelectedIndex = -1;
			public Vector2 ScrollPosition = Vector2.zero;

			public bool IsBranch => Children != null;
			public TreeItem SelectedItem => IsBranch && SelectedIndex >= 0 && SelectedIndex < Children.Count ? Children[SelectedIndex] : null;

			public static TreeItem Leaf(int index, string path, GUIContent content, TreeItem parent)
			{
				var item = new TreeItem
				{
					ItemIndex = index,
					Path = path,
					SearchName = content.text.Replace(" ", string.Empty).ToLower(),
					Name = content.text,
					Content = content,
					Parent = parent,
				};

				if (item.Parent != null)
					item.Parent.Children.Add(item);

				return item;
			}

			public static TreeItem Branch(string path, GUIContent content, TreeItem parent)
			{
				var item = Leaf(-1, path, content, parent);
				item.Children = new List<TreeItem>();
				return item;
			}
		}

		private static readonly StaticStyle Header = new StaticStyle(() => new GUIStyle("In BigTitle") { font = EditorStyles.boldLabel.font });
		private static readonly StaticStyle ItemButton = new StaticStyle(() => new GUIStyle("PR Label") { alignment = TextAnchor.MiddleLeft, fixedHeight = _itemHeight });
		private static readonly StaticStyle Background = new StaticStyle(() => new GUIStyle("grey_border"));
		private static readonly StaticStyle RightArrow = new StaticStyle(() => new GUIStyle("AC RightArrow"));
		private static readonly StaticStyle LeftArrow = new StaticStyle(() => new GUIStyle("AC LeftArrow"));
		private static readonly StaticStyle SearchText = new StaticStyle(() => new GUIStyle("SearchTextField"));
		private static readonly StaticStyle SearchCancelEmpty = new StaticStyle(() => new GUIStyle("SearchCancelButtonEmpty"));
		private static readonly StaticStyle SearchCancel = new StaticStyle(() => new GUIStyle("SearchCancelButton"));
		private static readonly IconButton FolderIcon = new IconButton("FolderEmpty Icon");
		private static readonly IconButton DefaultTypeIcon = new IconButton("cs Script Icon");

		private const string _invalidTreeWarning = "(UCSTCIT) Unable to show search tree: invalid tabs and content";

		private const int _headerHeight = 25;
		private const int _itemHeight = 20;
		private const int _visibleItemCount = 14;
		private const int _windowHeight = 2 * _headerHeight + _itemHeight * _visibleItemCount;

		private static SearchTreeControl _instance;

		public int _currentTab = 0;

		private List<TreeItem> _roots = new List<TreeItem>();
		private List<List<TreeItem>> _searchList = new List<List<TreeItem>>();

		private TreeItem _targetBranch;
		private TreeItem _currentBranch;
		private TreeItem _searchRoot;
		
		private TreeItem _activeBranch => _hasSearch ? _searchRoot : _currentBranch;
		private List<TreeItem> _currentLeaves => _searchList[_currentTab];

		private Action<int, int> _onSelected;
		private float _animation = 1.0f;
		private int _animationTarget = 1;
		private long _lastTime;
		private bool _scrollToSelected;
		private string _delayedSearch;
		private string _search = string.Empty;

		private bool _isAnimating => _animation != _animationTarget;
		private bool _hasSearch => !string.IsNullOrEmpty(_search);

		public static void Show(Rect rect, List<GUIContent[]> trees, List<GUIContent> tabs, Action<int, int> onSelected, int currentTab, int currentIndex = 0)
		{
			if (trees.Count != tabs.Count)
				Debug.LogWarning(_invalidTreeWarning);

			if (_instance == null)
				_instance = CreateInstance<SearchTreeControl>();

			_instance.Init(rect, trees, tabs, onSelected, currentIndex, currentTab);
		}

		private void Init(Rect rect, List<GUIContent[]> trees, List<GUIContent> tabs, Action<int, int> onSelected, int currentTab, int currentIndex)
		{
			_onSelected = onSelected;
			_currentTab = currentTab;

			CreateTrees(trees, tabs, currentIndex);
			ShowAsDropDown(new Rect(GUIUtility.GUIToScreenPoint(rect.position), rect.size), new Vector2(rect.width, _windowHeight));
			Focus();

			wantsMouseMove = true;
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

		private void CreateTrees(List<GUIContent[]> trees, List<GUIContent> tabs, int selection)
		{
			for (var t = 0; t < trees.Count; t++)
			{
				var tree = trees[t];
				var tab = tabs[t];

				var root = TreeItem.Branch(string.Empty, tab, null);
				var rootPath = tab.text + "/";
				var leaves = new List<TreeItem>();

				for (var index = 0; index < tree.Length; index++)
				{
					var node = tree[index];
					var fullPath = rootPath + node.text;
					var submenus = fullPath.Split('/');

					var path = rootPath;
					var child = root;

					for (var i = 1; i < submenus.Length - 1; i++)
					{
						var menu = submenus[i];
						path += menu + "/";

						var previousChild = child;
						child = GetChild(child, path);

						if (child == null)
							child = TreeItem.Branch(path, new GUIContent(menu, FolderIcon.Content.image), previousChild);
					}

					leaves.Add(TreeItem.Leaf(index, path, new GUIContent(submenus.Last(), node.image ?? DefaultTypeIcon.Content.image), child));
				}

				_roots.Add(root);
				_searchList.Add(leaves);
			}

			_searchRoot = TreeItem.Branch("Search", new GUIContent("Search"), null);
			_currentBranch = _currentLeaves[selection].Parent;

			RebuildSearch();
		}

		private void RebuildSearch()
		{
			_searchRoot.Children.Clear();
			_searchRoot.SelectedIndex = -1;

			if (!_hasSearch)
			{
				_animationTarget = 1;
				_lastTime = DateTime.Now.Ticks;
			}
			else
			{
				var subwords = _search.ToLower().Split(' ').Where(subword => !string.IsNullOrEmpty(subword)).ToArray();
				var starts = new List<TreeItem>();
				var contains = new List<TreeItem>();

				foreach (var item in _currentLeaves)
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

				_searchRoot.Children.AddRange(starts);
				_searchRoot.Children.AddRange(contains);
			}
		}


		void OnGUI()
		{
			HandleKeyboard();

			var backgroundRect = new Rect(Vector2.zero, position.size);

			GUI.Label(backgroundRect, GUIContent.none, Background.Style);
			GUI.SetNextControlName("Search");
			EditorGUI.FocusTextInControl("Search");
			RectHelper.TakeVerticalSpace(ref backgroundRect);

			var searchRect = RectHelper.Inset(RectHelper.TakeHeight(ref backgroundRect, _headerHeight), 8, 8, RectHelper.VerticalSpace, RectHelper.VerticalSpace);
			var cancelRect = RectHelper.TakeTrailingWidth(ref searchRect, RectHelper.IconWidth);
			var search = GUI.TextField(searchRect, _delayedSearch ?? _search, SearchText.Style);
			var buttonStyle = string.IsNullOrEmpty(_search) ? SearchCancelEmpty.Style : SearchCancel.Style;

			if (GUI.Button(cancelRect, GUIContent.none, buttonStyle))
			{
				GUI.FocusControl(null);
				search = string.Empty;
			}

			if (search != _search || _delayedSearch != null)
			{
				if (!_isAnimating)
				{
					_search = _delayedSearch ?? search;
					_delayedSearch = null;

					RebuildSearch();
				}
				else
				{
					_delayedSearch = search;
				}
			}

			DrawHeader(backgroundRect, _activeBranch, _animation);

			if (_animation < 1.0f && _targetBranch != null)
				DrawHeader(backgroundRect, _targetBranch, _animation + 1.0f);

			if (_isAnimating && Event.current.type == EventType.Repaint)
			{
				var ticks = DateTime.Now.Ticks;
				var speed = (ticks - _lastTime) / 0.25E+07f;

				_lastTime = ticks;
				_animation = Mathf.MoveTowards(_animation, _animationTarget, speed);

				if (_animationTarget == 0 && _animation == 0.0f)
				{
					_animation = 1.0f;
					_animationTarget = 1;
					_currentBranch = _targetBranch;
					_targetBranch = null;
				}

				Repaint();
			}
		}

		private void DrawHeader(Rect rect, TreeItem parent, float animation)
		{
			if (parent != null)
			{
				animation = Mathf.Floor(animation) + Mathf.SmoothStep(0.0f, 1.0f, Mathf.Repeat(animation, 1.0f));

				rect.x = rect.width * (1 - animation);

				var headerRect = RectHelper.Inset(RectHelper.TakeHeight(ref rect, _headerHeight), 1, 1, 0, 0);

				GUI.Label(headerRect, parent.Content, Header.Style);

				if (parent.Parent != null)
				{
					var arrowRect = RectHelper.AdjustHeight(RectHelper.TakeLeadingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);

					if (GUI.Button(arrowRect, GUIContent.none, LeftArrow.Style))
						GoToParent();
				}
				else if (_roots.Count > 1)
				{
					var leftRect = RectHelper.AdjustHeight(RectHelper.TakeLeadingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);
					var rightRect = RectHelper.AdjustHeight(RectHelper.TakeTrailingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);

					if (GUI.Button(leftRect, GUIContent.none, LeftArrow.Style))
						ChangeTab(-1);

					if (GUI.Button(rightRect, GUIContent.none, RightArrow.Style))
						ChangeTab(1);
				}

				DrawChildren(rect, parent);
			}
		}

		private void DrawChildren(Rect rect, TreeItem parent)
		{
			if (parent.Children.Count > 0)
			{
				var width = parent.Children.Count > _visibleItemCount ? rect.width - RectHelper.IconWidth : rect.width;
				var area = new Rect(Vector2.zero, new Vector2(width, _itemHeight * parent.Children.Count));
				var selectedRect = area;

				using (var scroll = new GUI.ScrollViewScope(rect, parent.ScrollPosition, area))
				{
					parent.ScrollPosition = scroll.scrollPosition;

					for (var i = 0; i < parent.Children.Count; i++)
					{
						var item = parent.Children[i];
						var itemRect = RectHelper.TakeHeight(ref area, _itemHeight);

						if ((Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDown) && parent.SelectedIndex != i && itemRect.Contains(Event.current.mousePosition))
						{
							parent.SelectedIndex = i;
							Repaint();
						}

						var selected = i == parent.SelectedIndex;

						if (selected)
							selectedRect = itemRect;

						if (Event.current.type == EventType.Repaint)
						{
							ItemButton.Style.Draw(itemRect, item.Content, false, false, selected, selected);

							if (item.IsBranch)
							{
								var arrowRect = RectHelper.TakeTrailingIcon(ref itemRect);

								RightArrow.Style.Draw(arrowRect, false, false, false, false);
							}
						}

						if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
						{
							Event.current.Use();
							parent.SelectedIndex = i;
							SelectChild();
						}
					}
				}

				if (_scrollToSelected && Event.current.type == EventType.Repaint)
				{
					_scrollToSelected = false;

					if (selectedRect.yMax - rect.height > parent.ScrollPosition.y)
					{
						parent.ScrollPosition.y = selectedRect.yMax - rect.height;
						Repaint();
					}

					if (selectedRect.y < parent.ScrollPosition.y)
					{
						parent.ScrollPosition.y = selectedRect.y;
						Repaint();
					}
				}
			}
		}

		private void HandleKeyboard()
		{
			var current = Event.current;
			if (current.type == EventType.KeyDown)
			{
				if (current.keyCode == KeyCode.DownArrow)
				{
					_activeBranch.SelectedIndex = Mathf.Min(_activeBranch.SelectedIndex + 1, _activeBranch.Children.Count - 1);
					_scrollToSelected = true;
					current.Use();
				}

				if (current.keyCode == KeyCode.UpArrow)
				{
					_activeBranch.SelectedIndex = Mathf.Max(_activeBranch.SelectedIndex - 1, 0);
					_scrollToSelected = true;
					current.Use();
				}

				if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
				{
					SelectChild();
					current.Use();
				}

				if (!_hasSearch) // Keep this in the check because it will otherwise override the search text
				{
					if (current.keyCode == KeyCode.LeftArrow || current.keyCode == KeyCode.Backspace)
					{
						GoToParent();
						current.Use();
					}
					if (current.keyCode == KeyCode.RightArrow)
					{
						SelectChild();
						current.Use();
					}
					if (current.keyCode == KeyCode.Escape)
					{
						Close();
						current.Use();
					}
				}
			}
		}

		private void ChangeTab(int increment)
		{
			_currentTab += increment;

			if (_currentTab < 0)
				_currentTab = _roots.Count - 1;
			else if (_currentTab >= _roots.Count)
				_currentTab = 0;

			if (increment < 0)
				AnimateLeft(_roots[_currentTab]);
			else if (increment > 0)
				AnimateRight(_roots[_currentTab]);

			_targetBranch.SelectedIndex = -1;
			_activeBranch.SelectedIndex = -1;
		}

		private void AnimateLeft(TreeItem target)
		{
			_lastTime = DateTime.Now.Ticks;
			_animationTarget = 0;
			_targetBranch = target;
		}

		private void AnimateRight(TreeItem target)
		{
			if (_animationTarget == 0)
			{
				_animationTarget = 1;
				_targetBranch = target;
			}
			else if (_animation == 1.0f)
			{
				_animation = 0.0f;
				_targetBranch = _activeBranch;
				_currentBranch = target;
			}

			_lastTime = DateTime.Now.Ticks;
		}

		private void GoToParent()
		{
			if (_activeBranch.Parent != null)
				AnimateLeft(_activeBranch.Parent);
		}

		private void SelectChild()
		{
			var selection = _activeBranch.SelectedItem;

			if (selection != null)
			{
				if (selection.IsBranch)
				{
					AnimateRight(selection);
				}
				else
				{
					_onSelected(_currentTab, selection.ItemIndex);
					Close();
				}
			}
		}
	}
}
