using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.UtilityEditor
{
	[InitializeOnLoad]
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

		private const int _headerHeight = 25;
		private const int _itemHeight = 20;
		private const int _visibleItemCount = 14;
		private const int _windowHeight = 2 * _headerHeight + _itemHeight * _visibleItemCount;

		public static int Selection = -1;

		private static Styles _styles;
		private static SearchTreeControl _instance;
		private Element[] _tree;
		private Element[] _searchTree;
		private List<GroupElement> _stack = new List<GroupElement>();
		private float _animation = 1.0f;
		private int _animationTarget = 1;
		private long _lastTime;
		private bool _scrollToSelected;
		private string _delayedSearch;
		private string _search = string.Empty;

		private bool _hasSearch => !string.IsNullOrEmpty(_search);

		private GroupElement _activeParent => _stack[_stack.Count - 2 + _animationTarget];

		private Element[] _activeTree => (!_hasSearch) ? _tree : _searchTree;

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

		private bool _isAnimating => _animation != _animationTarget;

		public static void Show(Rect rect, GUIContent[] names, GUIContent label, int currentIndex)
		{
			if (_instance == null)
				_instance = CreateInstance<SearchTreeControl>();

			_instance.Init(rect, names, label, currentIndex);
		}

		private void Init(Rect rect, GUIContent[] names, GUIContent label, int currentIndex)
		{
			wantsMouseMove = true;

			Selection = currentIndex;

			var position = GUIUtility.GUIToScreenPoint(rect.position);

			CreateTree(names, label);
			ShowAsDropDown(new Rect(position, rect.size), new Vector2(rect.width, _windowHeight));
			Focus();
		}

		private void CreateTree(GUIContent[] names, GUIContent label)
		{
			var submenus = new List<string> { label.text };
			var elements = new List<Element> { new GroupElement(0, label) };

			for (var i = 0; i < names.Length; i++)
			{
				var content = names[i];
				var path = label.text + "/" + names[i].text; // Add the top level group
				var menus = path.Split(new char[] { '/' });
				var name = new GUIContent(menus[menus.Length - 1], content.image); 

				while (menus.Length - 1 < submenus.Count)
					submenus.RemoveAt(submenus.Count - 1);

				while (submenus.Count > 0 && menus[submenus.Count - 1] != submenus[submenus.Count - 1])
					submenus.RemoveAt(submenus.Count - 1);

				while (menus.Length - 1 > submenus.Count)
				{
					var menu = menus[submenus.Count];
					elements.Add(new GroupElement(submenus.Count, new GUIContent(menu)));
					submenus.Add(menu);
				}

				elements.Add(new LeafElement(submenus.Count, name, i));
			}

			_tree = elements.ToArray();

			if (_stack.Count == 0)
			{
				_stack.Add(_tree[0] as GroupElement);
			}
			else
			{
				var groupElement = _tree[0] as GroupElement;
				var level = 0;

				while (true)
				{
					var groupElement2 = _stack[level];

					_stack[level] = groupElement;
					_stack[level].SelectedIndex = groupElement2.SelectedIndex;
					_stack[level].Scroll = groupElement2.Scroll;

					level++;

					if (level == _stack.Count)
						break;

					var children = GetChildren(_activeTree, groupElement);
					var element = children.FirstOrDefault(child => child.Content.text == _stack[level].Content.text);

					if (element != null && element is GroupElement group)
					{
						groupElement = group;
					}
					else
					{
						while (_stack.Count > level)
							_stack.RemoveAt(level);
					}
				}
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
				var num = (ticks - _lastTime) / 1E+07f;

				_lastTime = ticks;
				_animation = Mathf.MoveTowards(_animation, _animationTarget, num * 4);

				if (_animationTarget == 0 && _animation == 0.0f)
				{
					_animation = 1.0f;
					_animationTarget = 1;
					_stack.RemoveAt(_stack.Count - 1);
				}

				Repaint();
			}
		}

		private void DrawHeader(Rect rect, Element[] tree, float animation, GroupElement parent, GroupElement grandParent)
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

			DrawList(rect, tree, parent);
		}

		private void DrawList(Rect rect, Element[] tree, GroupElement parent)
		{
			var children = GetChildren(tree, parent);
			var width = children.Count > 14 ? rect.width - RectHelper.IconWidth : rect.width;
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
					_activeParent.SelectedIndex++;
					_activeParent.SelectedIndex = Mathf.Min(_activeParent.SelectedIndex, GetChildren(_activeTree, _activeParent).Count - 1);
					_scrollToSelected = true;
					current.Use();
				}

				if (current.keyCode == KeyCode.UpArrow)
				{
					_activeParent.SelectedIndex--;
					_activeParent.SelectedIndex = Mathf.Max(_activeParent.SelectedIndex, 0);
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
				_searchTree = null;

				if (_stack[_stack.Count - 1].Content.text == "Search")
				{
					_stack.Clear();
					_stack.Add(_tree[0] as GroupElement);
				}

				_animationTarget = 1;
				_lastTime = DateTime.Now.Ticks;
			}
			else
			{
				var subwords = _search.ToLower().Split(' ').Where(subword => !string.IsNullOrEmpty(subword)).ToArray();

				var starts = new List<Element>();
				var contains = new List<Element>();

				foreach (var element in _tree)
				{
					if (element is LeafElement)
					{
						if (subwords.Length > 0 && element.SearchName.StartsWith(subwords[0]))
							starts.Add(element);

						foreach (var subword in subwords)
						{
							if (element.SearchName.Contains(subword))
								contains.Add(element);
						}
					}
				}

				var results = new List<Element>();
				var search = new GroupElement(0, new GUIContent("Search"));
				results.Add(search);
				results.AddRange(starts);
				results.AddRange(contains);

				_searchTree = results.ToArray();

				_stack.Clear();
				_stack.Add(search);

				if (GetChildren(_activeTree, _activeParent).Count >= 1)
					_activeParent.SelectedIndex = 0;
				else
					_activeParent.SelectedIndex = -1;
			}
		}

		private GroupElement GetElementRelative(int relative)
		{
			var index = _stack.Count + relative - 1;
			return index < 0 ? null : _stack[index];
		}

		private void GoToParent()
		{
			if (_stack.Count > 1)
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
			else if (!_hasSearch)
			{
				_lastTime = DateTime.Now.Ticks;

				if (_animationTarget == 0)
				{
					_animationTarget = 1;
				}
				else if (_animation == 1.0f)
				{
					_animation = 0.0f;
					_stack.Add(element as GroupElement);
				}
			}
		}

		private List<Element> GetChildren(Element[] tree, Element parent)
		{
			var children = new List<Element>();
			var level = -1;
			var i = 0;
			for (i = 0; i < tree.Length; i++)
			{
				if (tree[i] == parent)
				{
					level = parent.Level + 1;
					i++;
					break;
				}
			}

			if (level == -1)
				return children;

			for (; i < tree.Length; i++)
			{
				var element = tree[i];

				if (element.Level < level)
					break;

				if (element.Level == level || _hasSearch)
					children.Add(element);
			}

			return children;
		}
	}
}