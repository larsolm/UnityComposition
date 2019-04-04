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
		private class Element
		{
			public int Level;
			public GUIContent Content;
			public string SearchName;

			protected Element(int level, GUIContent name)
			{
				Level = level;
				Content = name;
				SearchName = name.text.Replace(" ", string.Empty).ToLower();
			}
		}

		private class LeafElement : Element
		{
			public int Index;

			public LeafElement(int level, GUIContent name, int index) : base(level, name)
			{
				Index = index;
			}
		}

		[Serializable]
		private class GroupElement : Element
		{
			public Vector2 Scroll;
			public int SelectedIndex;

			public GroupElement(int level, GUIContent name) : base(level, name)
			{
			}
		}

		private class Styles
		{
			public GUIStyle Header = new GUIStyle("In BigTitle");
			public GUIStyle ItemButton = new GUIStyle("PR Label");
			public GUIStyle Background = new GUIStyle("grey_border");
			public GUIStyle RightArrow = new GUIStyle("AC RightArrow");
			public GUIStyle LeftArrow = new GUIStyle("AC LeftArrow");
			public GUIStyle SearchText = new GUIStyle("SearchTextField");
			public GUIStyle SearchCancelEmpty = new GUIStyle("SearchCancelButtonEmpty");
			public GUIStyle SearchCancel = new GUIStyle("SearchCancelButton");

			public Styles()
			{
				Header.font = EditorStyles.boldLabel.font;
				ItemButton.alignment = TextAnchor.MiddleLeft;
				ItemButton.fixedHeight = _itemHeight;
			}
		}

		private const string _invalidTreeWarning = "(UCSTCIT) Unable to show search tree: invalid tabs and content";

		private const int _headerHeight = 25;
		private const int _itemHeight = 20;
		private const int _visibleItemCount = 14;
		private const int _windowHeight = 2 * _headerHeight + _itemHeight * _visibleItemCount;

		public static int Tab = 0;
		public static int Selection = -1;

		private static Styles _styles;
		private static SearchTreeControl _instance;
		private List<List<Element>> _trees = new List<List<Element>>();
		private List<Element> _searchTree = new List<Element>();
		private List<List<GroupElement>> _stacks = new List<List<GroupElement>>();
		private float _animation = 1.0f;
		private int _animationTarget = 1;
		private long _lastTime;
		private bool _scrollToSelected;
		private string _delayedSearch;
		private string _search = string.Empty;

		private bool _isAnimating => _animation != _animationTarget;
		private bool _hasSearch => !string.IsNullOrEmpty(_search);

		private List<Element> _currentTree => _trees[Tab];
		private List<GroupElement> _currentStack => _stacks[Tab];

		private List<Element> _activeTree => _hasSearch ? _searchTree : _currentTree;
		private GroupElement _activeParent => _currentStack[_currentStack.Count - 2 + _animationTarget];

		private Element _activeElement
		{
			get
			{
				if (_activeTree == null)
					return null;

				var children = GetChildren(_activeTree, _activeParent);
				return children.Count == 0 ? null : children[_activeParent.SelectedIndex];
			}
		}

		public static void Show(Rect rect, List<GUIContent[]> trees, List<GUIContent> tabs, int currentIndex)
		{
			if (trees.Count != tabs.Count)
				Debug.LogWarning(_invalidTreeWarning);

			if (_instance == null)
				_instance = CreateInstance<SearchTreeControl>();

			_instance.Init(rect, trees, tabs, currentIndex);
		}

		private void Init(Rect rect, List<GUIContent[]> trees, List<GUIContent> tabs, int currentIndex)
		{
			wantsMouseMove = true;

			Tab = 0;
			Selection = currentIndex;

			var position = GUIUtility.GUIToScreenPoint(rect.position);

			CreateTrees(trees, tabs);
			ShowAsDropDown(new Rect(position, rect.size), new Vector2(rect.width, _windowHeight));
			Focus();
		}

		private void CreateTrees(List<GUIContent[]> trees, List<GUIContent> tabs)
		{
			for (var t = 0; t < trees.Count; t++)
			{
				var tree = trees[t];
				var tab = tabs[t];

				var submenus = new List<string> { tab.text };
				var elements = new List<Element> { new GroupElement(0, tab) };

				for (var i = 0; i < tree.Length; i++)
				{
					var leaf = tree[i];
					var path = tab.text + "/" + leaf.text; // Prepend the top level tab group
					var menus = path.Split(new char[] { '/' });
					var content = new GUIContent(menus.Last(), leaf.image);

					while (menus.Length - 1 < submenus.Count)
						submenus.RemoveAt(submenus.Count - 1);

					while (submenus.Count > 0 && menus[submenus.Count - 1] != submenus.Last())
						submenus.RemoveAt(submenus.Count - 1);

					while (menus.Length - 1 > submenus.Count)
					{
						var menu = menus[submenus.Count];
						elements.Add(new GroupElement(submenus.Count, new GUIContent(menu)));
						submenus.Add(menu);
					}

					elements.Add(new LeafElement(submenus.Count, content, i));
				}
				
				_stacks.Add(new List<GroupElement> { elements.First() as GroupElement });
				_trees.Add(elements);
			}

			RebuildSearch();
		}

		void OnGUI()
		{
			if (_styles == null)
				_styles = new Styles();

			HandleKeyboard();

			var backgroundRect = new Rect(Vector2.zero, position.size);

			GUI.Label(backgroundRect, GUIContent.none, _styles.Background);
			GUI.SetNextControlName("Search");
			EditorGUI.FocusTextInControl("Search");
			RectHelper.TakeVerticalSpace(ref backgroundRect);

			var searchRect = RectHelper.Inset(RectHelper.TakeHeight(ref backgroundRect, _headerHeight), 8, 8, RectHelper.VerticalSpace, RectHelper.VerticalSpace);
			var cancelRect = RectHelper.TakeTrailingWidth(ref searchRect, RectHelper.IconWidth);
			var search = GUI.TextField(searchRect, _delayedSearch ?? _search, _styles.SearchText);
			var buttonStyle = string.IsNullOrEmpty(_search) ? _styles.SearchCancelEmpty : _styles.SearchCancel;

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

			DrawHeader(backgroundRect, _activeTree, _animation, GetElementRelative(0), GetElementRelative(-1));

			if (_animation < 1.0f)
				DrawHeader(backgroundRect, _activeTree, _animation + 1.0f, GetElementRelative(-1), GetElementRelative(-2));

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
					_currentStack.RemoveAt(_currentStack.Count - 1);
				}

				Repaint();
			}
		}

		private void DrawHeader(Rect rect, List<Element> tree, float animation, GroupElement parent, GroupElement grandParent)
		{
			animation = Mathf.Floor(animation) + Mathf.SmoothStep(0.0f, 1.0f, Mathf.Repeat(animation, 1.0f));

			rect.x = rect.width * (1 - animation);

			var headerRect = RectHelper.Inset(RectHelper.TakeHeight(ref rect, _headerHeight), 1, 1, 0, 0);

			GUI.Label(headerRect, parent.Content, _styles.Header);

			if (grandParent != null)
			{
				var arrowRect = RectHelper.AdjustHeight(RectHelper.TakeLeadingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);

				if (GUI.Button(arrowRect, GUIContent.none, _styles.LeftArrow))
					GoToParent();
			}
			else if (_trees.Count > 1)
			{
				var leftRect = RectHelper.AdjustHeight(RectHelper.TakeLeadingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);
				var rightRect = RectHelper.AdjustHeight(RectHelper.TakeTrailingIcon(ref headerRect), RectHelper.IconWidth, RectVerticalAlignment.Middle);

				if (GUI.Button(leftRect, GUIContent.none, _styles.LeftArrow))
					ChangeTab(-1);

				if (GUI.Button(rightRect, GUIContent.none, _styles.RightArrow))
					ChangeTab(1);
			}

			DrawList(rect, tree, parent);
		}

		private void DrawList(Rect rect, List<Element> tree, GroupElement parent)
		{
			var children = GetChildren(tree, parent);
			var width = children.Count > _visibleItemCount ? rect.width - RectHelper.IconWidth : rect.width;
			var area = new Rect(Vector2.zero, new Vector2(width, _itemHeight * children.Count));
			var selectedRect = area;

			using (var scroll = new GUI.ScrollViewScope(rect, parent.Scroll, area))
			{
				parent.Scroll = scroll.scrollPosition;

				for (var i = 0; i < children.Count; i++)
				{
					var element = children[i];
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
						_styles.ItemButton.Draw(itemRect, element.Content, false, false, selected, selected);

						var rightRect = RectHelper.TakeTrailingIcon(ref itemRect);

						if (element is GroupElement)
							_styles.RightArrow.Draw(rightRect, false, false, false, false);
					}

					if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
					{
						Event.current.Use();
						parent.SelectedIndex = i;
						GoToChild(element, true);
					}
				}
			}

			if (_scrollToSelected && Event.current.type == EventType.Repaint)
			{
				_scrollToSelected = false;
			
				if (selectedRect.yMax - rect.height > parent.Scroll.y)
				{
					parent.Scroll.y = selectedRect.yMax - rect.height;
					Repaint();
				}

				if (selectedRect.y < parent.Scroll.y)
				{
					parent.Scroll.y = selectedRect.y;
					Repaint();
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
					_activeParent.SelectedIndex = Mathf.Min(_activeParent.SelectedIndex + 1, GetChildren(_activeTree, _activeParent).Count - 1);
					_scrollToSelected = true;
					current.Use();
				}

				if (current.keyCode == KeyCode.UpArrow)
				{
					_activeParent.SelectedIndex = Mathf.Max(_activeParent.SelectedIndex - 1, 0);
					_scrollToSelected = true;
					current.Use();
				}

				if (current.keyCode == KeyCode.Return || current.keyCode == KeyCode.KeypadEnter)
				{
					GoToChild(_activeElement, true);
					current.Use();
				}

				if (!_hasSearch)
				{
					if (current.keyCode == KeyCode.LeftArrow || current.keyCode == KeyCode.Backspace)
					{
						GoToParent();
						current.Use();
					}
					if (current.keyCode == KeyCode.RightArrow)
					{
						GoToChild(_activeElement, false);
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

		private void RebuildSearch()
		{
			if (!_hasSearch)
			{
				_searchTree.Clear();

				if (_currentStack.Last().Content.text == "Search")
				{
					_currentStack.Clear();
					_currentStack.Add(_currentTree.First() as GroupElement);
				}

				_animationTarget = 1;
				_lastTime = DateTime.Now.Ticks;
			}
			else
			{
				var subwords = _search.ToLower().Split(' ').Where(subword => !string.IsNullOrEmpty(subword)).ToArray();

				var starts = new List<Element>();
				var contains = new List<Element>();

				foreach (var element in _currentTree)
				{
					if (element is LeafElement)
					{
						if (subwords.Length > 0 && element.SearchName.StartsWith(subwords.First()))
							starts.Add(element);

						foreach (var subword in subwords)
						{
							if (element.SearchName.Contains(subword))
								contains.Add(element);
						}
					}
				}

				var search = new GroupElement(0, new GUIContent("Search"));

				_searchTree.Clear();
				_searchTree.Add(search);
				_searchTree.AddRange(starts);
				_searchTree.AddRange(contains);

				_currentStack.Clear();
				_currentStack.Add(search);

				if (GetChildren(_activeTree, _activeParent).Count >= 1)
					_activeParent.SelectedIndex = 0;
				else
					_activeParent.SelectedIndex = -1;
			}
		}

		private GroupElement GetElementRelative(int relative)
		{
			var index = _currentStack.Count + relative - 1;
			return index < 0 ? null : _currentStack[index];
		}

		private void ChangeTab(int increment)
		{
			Tab += increment;

			if (Tab < 0)
				Tab = _trees.Count - 1;
			else if (Tab >= _trees.Count)
				Tab = 0;
		}

		private void GoToParent()
		{
			if (_currentStack.Count > 1)
			{
				_animationTarget = 0;
				_lastTime = DateTime.Now.Ticks;
			}
		}

		private void GoToChild(Element element, bool addIfComponent)
		{
			if (element is LeafElement leaf)
			{
				if (addIfComponent)
				{
					Selection = leaf.Index;
					Close();
				}
			}
			else if (element is GroupElement group && !_hasSearch)
			{
				_lastTime = DateTime.Now.Ticks;

				if (_animationTarget == 0)
				{
					_animationTarget = 1;
				}
				else if (_animation == 1.0f)
				{
					_animation = 0.0f;
					_currentStack.Add(group);
				}
			}
		}

		private List<Element> GetChildren(List<Element> tree, Element parent)
		{
			var children = new List<Element>();
			var level = -1;
			var index = 0;

			for (index = 0; index < tree.Count; index++)
			{
				if (tree[index] == parent)
				{
					level = parent.Level + 1;
					index++;
					break;
				}
			}

			if (level == -1)
				return children;

			for (; index < tree.Count; index++)
			{
				var element = tree[index];

				if (element.Level < level)
					break;

				if (element.Level == level || _hasSearch)
					children.Add(element);
			}

			return children;
		}
	}
}