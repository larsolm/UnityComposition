using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[InitializeOnLoad]
	public class History : EditorWindow
	{
		public const string StyleSheetPath = Utilities.AssetPath + "Windows/History/History.uss";
		public const string UssClassName = "pirho-history";
		public const string UssToolbarClassName = UssClassName + "__toolbar";
		public const string UssToolbarButtonClassName = UssClassName + "__toolbar-button";
		public const string UssListClassName = UssClassName + "__list";
		public const string UssListItemClassName = UssListClassName + "-item";
		public const string UssListItemLabelClassName = UssListItemClassName + "-label";
		public const string UssListItemLabelCurrentClassName = UssListItemLabelClassName + "--current";

		private Button _back;
		private Button _forward;
		private Button _clear;
		private ListView _listView;

		void OnEnable()
		{
			ElementHelper.AddStyleSheet(rootVisualElement, StyleSheetPath);
			rootVisualElement.AddToClassList(UssClassName);

			var toolbar = new Toolbar();
			toolbar.AddToClassList(UssToolbarClassName);

			_back = new ToolbarButton(HistoryList.MoveBack) { text = "Back" };
			_back.AddToClassList(UssToolbarButtonClassName);
			_back.SetEnabled(HistoryList.CanMoveBack());

			_clear = new ToolbarButton(() => { HistoryList.Clear(); Refresh(); }) { text = "Clear" };
			_clear.AddToClassList(UssToolbarButtonClassName);

			_forward = new ToolbarButton(HistoryList.MoveForward) { text = "Forward" };
			_forward.AddToClassList(UssToolbarButtonClassName);
			_forward.SetEnabled(HistoryList.CanMoveForward());

			_listView = new ListView();
			_listView.itemsSource = HistoryList.History;
			_listView.makeItem = MakeItem;
			_listView.bindItem = BindItem;
			_listView.selectionType = SelectionType.Single;
			_listView.AddToClassList(UssListClassName);
			_listView.onItemChosen += item => Select();
			_listView.onSelectionChanged += selection => Highlight();

			toolbar.Add(_back);
			toolbar.Add(new ToolbarSpacer { flex = true });
			toolbar.Add(_clear);
			toolbar.Add(new ToolbarSpacer { flex = true });
			toolbar.Add(_forward);

			rootVisualElement.Add(toolbar);
			rootVisualElement.Add(_listView);
			
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
			_listView.ScrollToItem(HistoryList.Current); // This call has an internal bug that doesn't actually make the item fully visible
		}

		private VisualElement MakeItem()
		{
			var item = new VisualElement();
			item.AddToClassList(UssListItemClassName);

			var label = new DraggableHistory();
			label.AddToClassList(UssListItemLabelClassName);

			DragHelper.MakeDraggable(label);

			item.Add(label);
			return item;
		}

		private void BindItem(VisualElement container, int index)
		{
			var label = container.ElementAt(0) as DraggableHistory;
			label.Index = HistoryList.Current;
			label.text = HistoryList.GetName(index);
			label.EnableInClassList(UssListItemLabelCurrentClassName, index == HistoryList.Current);
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

		private class DraggableHistory : Label, IDraggable
		{
			private enum State
			{
				Idle,
				Ready,
				Dragging
			}

			public int Index;

			public DragState DragState { get; set; }
			public string DragText => HistoryList.GetName(Index);
			public Object[] DragObjects => HistoryList.History[Index];
			public object DragData => DragObjects;
		}

		private static class HistoryList
		{
			private const string _windowMenu = "Window/PiRho Soft/History";
			private const string _moveBackMenu = "Edit/Navigation/Move Back &LEFT";
			private const string _moveForwardMenu = "Edit/Navigation/Move Forward &RIGHT";
			private const int _capacity = 100;

			private static readonly Icon _historyIcon = Icon.BuiltIn("Clipboard");

			private static bool _skipNextSelection = false;

			public static int Current { get; private set; }
			public static List<Object[]> History { get; private set; } = new List<Object[]>();

			static HistoryList()
			{
				Selection.selectionChanged += SelectionChanged;
				EditorApplication.playModeStateChanged += state => Clear();
			}

			[MenuItem(_windowMenu)]
			private static void Open()
			{
				var window = GetWindow<History>();
				window.titleContent = new GUIContent("History", _historyIcon.Content);
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

			public static void Clear()
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
		}
	}
}