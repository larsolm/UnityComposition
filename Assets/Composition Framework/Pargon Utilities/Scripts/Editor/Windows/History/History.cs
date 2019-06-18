using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[InitializeOnLoad]
	public class History : EditorWindow
	{
		private const string _styleSheetPath = Utilities.AssetPath + "Windows/History/History.uss";
		private const string _uxmlPath = Utilities.AssetPath + "Windows/History/History.uxml";

		private Button _back;
		private Button _forward;
		private ListView _listView;

		void OnEnable()
		{
			var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(_styleSheetPath);
			var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_uxmlPath);

			uxml.CloneTree(rootVisualElement);
			rootVisualElement.styleSheets.Add(styleSheet);

			_back = rootVisualElement.Query<Button>("back");
			_back.clickable.clicked += HistoryList.MoveBack;
			_back.SetEnabled(HistoryList.CanMoveBack());

			_forward = rootVisualElement.Query<Button>("forward");
			_forward.clickable.clicked += HistoryList.MoveForward;
			_forward.SetEnabled(HistoryList.CanMoveForward());

			_listView = rootVisualElement.Query<ListView>().First();
			_listView.selectionType = SelectionType.Single;
			_listView.itemsSource = HistoryList.History;
			_listView.makeItem = MakeItem;
			_listView.bindItem = BindItem;
			_listView.onItemChosen += item => Select();
			_listView.onSelectionChanged += selection => Highlight();

			Selection.selectionChanged += Refresh;
		}

		private void OnDisable()
		{
			Selection.selectionChanged -= Refresh;
		}

		private void Refresh()
		{
			_back.SetEnabled(HistoryList.CanMoveBack());
			_forward.SetEnabled(HistoryList.CanMoveForward());
			_listView.Refresh();
		}

		private VisualElement MakeItem()
		{
			var item = new VisualElement();
			item.AddToClassList("selection-item");

			var label = new Label();
			label.AddToClassList("selection-label");

			item.Add(label);
			return item;
		}

		private void BindItem(VisualElement container, int index)
		{
			var label = container.ElementAt(0) as Label;
			label.text = HistoryList.GetName(index);

			if (index == HistoryList.Current)
				label.AddToClassList("current");
			else
				label.RemoveFromClassList("current");
		}

		private void Select()
		{
			if (_listView.selectedIndex != HistoryList.Current)
			{
				HistoryList.Select(_listView.selectedIndex);
				Refresh();
			}
		}

		private void Highlight()
		{
			if (_listView.selectedItem is Object[] obj && obj.Length > 0)
				EditorGUIUtility.PingObject(obj[0]);
		}

		private static class HistoryList
		{
			private const string _windowMenu = "Window/Pargon Utilities/History";
			private const string _moveBackMenu = "Edit/Navigation/Move Back &LEFT";
			private const string _moveForwardMenu = "Edit/Navigation/Move Forward &RIGHT";
			private const int _capacity = 100;

			private static bool _skipNextSelection = false;

			public static int Current { get; private set; }
			public static List<Object[]> History { get; private set; } = new List<Object[]>();

			static HistoryList()
			{
				Selection.selectionChanged += SelectionChanged;
				EditorApplication.playModeStateChanged += OnPlayModeChanged;
			}

			[MenuItem(_windowMenu)]
			private static void Open()
			{
				var window = GetWindow<History>();
				window.titleContent = new GUIContent("History");
				window.Show();
			}

			[MenuItem(_moveBackMenu, validate = true)]
			public static bool CanMoveBack()
			{
				return Current > 0;
			}

			[MenuItem(_moveForwardMenu, validate = true)]
			public static bool CanMoveForward()
			{
				return Current < History.Count - 1;
			}

			[MenuItem(_moveBackMenu, priority = 1)]
			public static void MoveBack()
			{
				if (CanMoveBack())
					Select(--Current);
			}

			[MenuItem(_moveForwardMenu, priority = 2)]
			public static void MoveForward()
			{
				if (CanMoveForward())
					Select(++Current);
			}

			public static void Select(int index)
			{
				Current = index;
				_skipNextSelection = true;
				Selection.objects = History[index];
			}

			public static string GetName(int index)
			{
				var objects = History[index];
				if (objects.Length > 1)
					return string.Join(", ", objects.Select(obj => GetName(obj)));

				return GetName(objects[0]);
			}

			private static string GetName(Object obj)
			{
				if (obj == null)
					return "(missing)";

				if (string.IsNullOrEmpty(obj.name))
					return string.Format("'{0}'", obj.GetType().Name);

				return obj.name;
			}

			private static void Clear()
			{
				Current = 0;
				History.Clear();
			}

			private static void SelectionChanged()
			{
				if (!_skipNextSelection)
				{
					if (Selection.objects != null && Selection.objects.Length > 0 && Selection.objects[0] is Object obj)
					{
						if (History.Count == 0 || History[Current][0] != obj)
						{
							var trailing = History.Count - Current - 1;

							if (trailing > 0)
								History.RemoveRange(Current + 1, trailing);

							if (Current == _capacity)
								History.RemoveAt(0);

							History.Add(Selection.objects);
							Current = History.Count - 1;
						}
					}
				}
				else
				{
					_skipNextSelection = false;
				}
			}

			private static void OnPlayModeChanged(PlayModeStateChange state)
			{
				switch (state)
				{
					case PlayModeStateChange.ExitingEditMode: Clear(); break;
					case PlayModeStateChange.EnteredEditMode: Clear(); break;
				}
			}
		}
	}
}