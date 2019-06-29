using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using MenuItem = UnityEditor.MenuItem;

namespace PiRhoSoft.CompositionEditor
{
	public class WatchWindowElement : VisualElement
	{
		private const string _uxmlPath = Composition.StylePath + "Instructions/WatchWindow/WatchWindow.uxml";
		private const string _styleSheetPath = Composition.StylePath + "Instructions/WatchWindow/WatchWindow.uss";

		private const string _ussGlobalName = "global-container";
		private const string _ussStoreName = "store-container";
		private const string _ussWatchName = "watch-container";

		private const string _ussBaseClass = "pargon-watch-window";

		private static readonly BoolPreference _logInstructionsEnabled = new BoolPreference("PiRhoSoft.Composition.CompositionManager.LogInstructionsEnabled", false);

		private readonly VisualElement _globalContainer;
		private readonly VisualElement _storeContainer;
		private readonly VisualElement _watchedContainer;

		private VariableStoreElement _globalStore;

		public WatchWindowElement()
		{
			ElementHelper.AddVisualTree(this, _uxmlPath);
			ElementHelper.AddStyleSheet(this, _styleSheetPath);

			AddToClassList(_ussBaseClass);

			_globalContainer = this.Q(_ussGlobalName);
			_globalContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Owner, evt.Name, evt.Store));

			_storeContainer = this.Q(_ussStoreName);
			_watchedContainer = this.Q(_ussWatchName);

			// Make Watch Toolbar
			CreateGlobalStore();
			CreateStores();
			// Make Expression Field

			CompositionManager.LogTracking = _logInstructionsEnabled.Value;

			EditorApplication.playModeStateChanged += PlayModeStateChanged;
		}

		private void PlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
				_globalStore = null;
				_storeContainer.Clear();
				_watchedContainer.Clear();
			}
		}

		private void CreateGlobalStore()
		{
			if (CompositionManager.Exists && (_globalStore == null || _globalStore.Store != CompositionManager.Instance.GlobalStore))
			{
				_globalStore = new VariableStoreElement(CompositionManager.Instance, CompositionManager.GlobalStoreName, CompositionManager.Instance.GlobalStore, false, false);
				_globalContainer.Clear();
				_globalContainer.Add(_globalStore);
			}
		}

		private void CreateStores()
		{
			if (CompositionManager.Exists)
			{
				foreach (var instruction in CompositionManager.TrackingState)
				{
					var store = new VariableStoreElement(instruction.Key, instruction.Key.name, instruction.Key.Variables, true, false);
					_storeContainer.Add(store);
				}
			}
		}

		private void AddWatch(Object owner, string name, IVariableStore store)
		{
			var element = new VariableStoreElement(owner, name, store, false, true);
			_watchedContainer.Add(element);
		}
	}

	public class WatchWindow : EditorWindow
	{
		public class WatchEvent : EventBase<WatchEvent>
		{
			public Object Owner { get; private set; }
			public IVariableStore Store { get; private set; }
			public string Name { get; private set; }

			public static WatchEvent GetPooled(Object owner, IVariableStore store, string name)
			{
				var pooled = GetPooled();
				pooled.Owner = owner;
				pooled.Store = store;
				pooled.Name = name;
				return pooled;
			}
		}

		private const float _minimumWidth = 200.0f;

		private static readonly Icon _windowIcon = Icon.BuiltIn("UnityEditor.LookDevView");

		//private const float _toolbarHeight = 17.0f;
		//private const float _toolbarPadding = 17.0f;
		//private const float _promptHeight = 20.0f;
		//private const float _watchButtonWidth = 60.0f;
		//
		//private const string _expressionResultLog = "{0}: ({1}) {2}";
		//private const string _missingWatchWarning = "(CWWMW) Unable to find variable '{0}' to watch";
		//private const string _invalidWatchWarning = "(CWWIW) Unable to watch variable '{0}' of type '{1}' - only variable stores can be watched";
		//

		//private static readonly Label _enableLogInstructionsButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Enable logging of instruction execution");
		//private static readonly Label _disableLogInstructionsButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Disable logging of instruction execution");
		//private static readonly Label _executeButton = new Label(Icon.Add, "", "Execute the expression");
		//
		//private Vector2 _scrollPosition;
		//private WatchMenu _watchMenu;
		//
		//private bool _promptValid = true;
		//private string _promptText = string.Empty;

		[MenuItem("Window/PiRho Soft/Watch Window")]
		public static void ShowWindow()
		{
			var window = CreateWindow<WatchWindow>("Watch Window");
			window.minSize = new Vector2(_minimumWidth, window.minSize.y);
			window.titleContent.image = _windowIcon.Content;
			window.rootVisualElement.Add(new WatchWindowElement());
			window.Show();
		}

		#region Toolbar
		//
		//private class WatchMenu : PopupWindowContent
		//{
		//	public WatchWindow Window;
		//
		//	private const float _padding = 5.0f;
		//	private string _variable = string.Empty;
		//	private bool _valid = true;
		//
		//	private static readonly Label _addButton = new Label(Icon.Add, "", "Add the variable to the watch window");
		//
		//	public override Vector2 GetWindowSize()
		//	{
		//		return new Vector2(300, RectHelper.LineHeight + 2 * _padding);
		//	}
		//
		//	public override void OnGUI(Rect rect)
		//	{
		//		rect = RectHelper.Inset(rect, _padding);
		//
		//		var add = false;
		//		var addRect = RectHelper.TakeTrailingWidth(ref rect, 32.0f);
		//		RectHelper.TakeTrailingWidth(ref rect, RectHelper.HorizontalSpace);
		//
		//		using (new InvalidScope(_valid))
		//		{
		//			using (var changes = new EditorGUI.ChangeCheckScope())
		//			{
		//				add = EnterField.DrawString("AddWatchVariable", rect, GUIContent.none, ref _variable);
		//
		//				if (changes.changed)
		//					_valid = true;
		//			}
		//		}
		//
		//		if (GUI.Button(addRect, _addButton.Content))
		//			add = true;
		//
		//		if (add)
		//		{
		//			_valid = Window.AddWatch(_variable);
		//
		//			if (_valid)
		//			{
		//				_variable = string.Empty;
		//				editorWindow.Close();
		//			}
		//			else
		//			{
		//				editorWindow.Repaint();
		//			}
		//		}
		//	}
		//}
		//
		//private bool AddWatch(string variable)
		//{
		//	var reference = new VariableReference { Variable = variable };
		//
		//	foreach (var instruction in CompositionManager.TrackingState)
		//	{
		//		var value = reference.GetValue(instruction.Key.Variables);
		//		if (!value.IsEmpty)
		//		{
		//			if (value.HasStore)
		//			{
		//				AddWatch(variable, value.Store);
		//				return true;
		//			}
		//			else
		//			{
		//				Debug.LogWarningFormat(_invalidWatchWarning, variable, value.Type);
		//				return false;
		//			}
		//		}
		//	}
		//
		//	if (CompositionManager.Exists)
		//	{
		//		var value = reference.GetValue(CompositionManager.Instance.DefaultStore);
		//
		//		if (value.HasStore)
		//		{
		//			AddWatch(variable, value.Store);
		//			return true;
		//		}
		//		else
		//		{
		//			Debug.LogWarningFormat(_invalidWatchWarning, variable, value.Type);
		//			return false;
		//		}
		//	}
		//
		//	Debug.LogWarningFormat(_missingWatchWarning, variable);
		//	return false;
		//}
		//
		//private void CreateToolbar()
		//{
		//	_watchMenu = new WatchMenu { Window = this };
		//}
		//
		//private void TeardownToolbar()
		//{
		//	_watchMenu = null;
		//}
		//
		//private void DrawToolbar()
		//{
		//	using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
		//	{
		//		using (new EditorGUI.DisabledScope(!Application.isPlaying))
		//		{
		//			if (GUILayout.Button("Watch", EditorStyles.toolbarDropDown, GUILayout.Width(_watchButtonWidth)))
		//				PopupWindow.Show(new Rect(_toolbarPadding, _toolbarHeight, 0.0f, 0.0f), _watchMenu);
		//		}
		//
		//		GUILayout.FlexibleSpace();
		//
		//		CompositionManager.LogTracking = GUILayout.Toggle(CompositionManager.LogTracking, CompositionManager.LogTracking ? _disableLogInstructionsButton.Content : _enableLogInstructionsButton.Content, EditorStyles.toolbarButton);
		//		_logInstructionsEnabled.Value = CompositionManager.LogTracking;
		//	}
		//}
		//
		#endregion

		#region Prompt
		//
		//private void DrawPrompt()
		//{
		//	var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
		//	var execute = false;
		//	var executeRect = RectHelper.TakeTrailingWidth(ref rect, 32.0f);
		//	RectHelper.TakeTrailingWidth(ref rect, RectHelper.HorizontalSpace);
		//
		//	using (new EditorGUI.DisabledScope(!Application.isPlaying))
		//	{
		//		using (new InvalidScope(_promptValid))
		//		{
		//			using (var changes = new EditorGUI.ChangeCheckScope())
		//			{
		//				execute = EnterField.DrawString("PromptEntry", rect, GUIContent.none, ref _promptText);
		//
		//				if (changes.changed)
		//					_promptValid = true;
		//			}
		//		}
		//
		//		if (GUI.Button(executeRect, _executeButton.Content))
		//			execute = true;
		//
		//		if (execute)
		//		{
		//			_promptValid = ExecuteExpression(_promptText);
		//
		//			if (_promptValid)
		//				_promptText = string.Empty;
		//		}
		//	}
		//}
		//
		//private bool ExecuteExpression(string text)
		//{
		//	var expression = new Expression();
		//	var compilation = expression.SetStatement(text);
		//
		//	if (!compilation.Success)
		//	{
		//		Debug.LogError(compilation.Message);
		//		return false;
		//	}
		//	else
		//	{
		//		var result = VariableValue.Empty;
		//		var instruction = CompositionManager.TrackingState.FirstOrDefault();
		//
		//		if (instruction.Key != null)
		//			result = expression.Execute(instruction.Value.Instruction, instruction.Key.Variables);
		//		else
		//			result = expression.Execute(null, CompositionManager.Instance.DefaultStore);
		//
		//		Debug.LogFormat(_expressionResultLog, expression.Statement, result.Type, result);
		//		return !result.IsEmpty;
		//	}
		//}
		//
		#endregion
	}
}
