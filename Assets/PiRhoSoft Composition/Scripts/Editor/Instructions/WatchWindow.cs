using PiRhoSoft.CompositionEngine;
using PiRhoSoft.UtilityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.CompositionEditor
{
	public class WatchWindow : EditorWindow
	{
		private const float _minimumWidth = 200.0f;
		private const float _toolbarHeight = 17.0f;
		private const float _toolbarPadding = 17.0f;
		private const float _promptHeight = 20.0f;
		private const float _watchButtonWidth = 60.0f;

		private const string _expressionResultLog = "{0}: ({1}) {2}";
		private const string _missingWatchWarning = "(CWWMW) unable to find variable {0} to watch";
		private const string _invalidWatchWarning = "(CWWIW) unable to watch variable {0} of type {1} - only variable stores can be watched";

		private static BoolPreference _logInstructionsEnabled = new BoolPreference("PiRhoSoft.Composition.CompositionManager.LogInstructionsEnabled", false);
		private static readonly Label _windowLabel = new Label(Icon.BuiltIn("UnityEditor.LookDevView"), label: "Watch Window");
		private static readonly Label _enableLogInstructionsButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Enable logging of instruction execution");
		private static readonly Label _disableLogInstructionsButton = new Label(Icon.BuiltIn("UnityEditor.ConsoleWindow"), "", "Disable logging of instruction execution");
		private static readonly Label _executeButton = new Label(Icon.BuiltIn(Icon.Add), "", "Execute the expression");

		private Vector2 _scrollPosition;
		private WatchMenu _watchMenu;

		private bool _promptValid = true;
		private string _promptText = string.Empty;

		private VariableStoreControl _globalStore;
		private List<VariableStoreControl> _instructionStores = new List<VariableStoreControl>();
		private List<VariableStoreControl> _watchedStores = new List<VariableStoreControl>();

		[MenuItem("Window/PiRho Soft/Watch Window")]
		public static void ShowWindow()
		{
			var window = GetWindow<WatchWindow>("Watch Window");
			window.Show();
		}

		void OnEnable()
		{
			EditorApplication.playModeStateChanged += PlayModeStateChanged;
			minSize = new Vector2(_minimumWidth, minSize.y);

			titleContent = _windowLabel.Content;

			CompositionManager.LogInstructions = _logInstructionsEnabled.Value;

			SetupToolbar();
		}

		void OnDisable()
		{
			TeardownToolbar();

			EditorApplication.playModeStateChanged -= PlayModeStateChanged;
		}

		private void PlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
			{
				_globalStore = null;
				_instructionStores.Clear();
				_watchedStores.Clear();
			}
		}

		void OnInspectorUpdate()
		{
			if (Application.isPlaying)
			{
				UpdateGlobalStore();
				UpdateInstructionStores();
				Repaint();
			}
		}

		void OnGUI()
		{
			DrawToolbar();

			using (var scroller = new EditorGUILayout.ScrollViewScope(_scrollPosition, GUILayout.Height(position.height - _toolbarHeight - _promptHeight)))
			{
				DrawGlobalStore();
				DrawInstructionStores();
				DrawWatchedStores();

				_scrollPosition = scroller.scrollPosition;
			}

			DrawPrompt();
		}

		#region Global Store

		private void UpdateGlobalStore()
		{
			if (CompositionManager.Exists && (_globalStore == null || _globalStore.Store != CompositionManager.Instance.GlobalStore))
			{
				_globalStore = new VariableStoreControl();
				_globalStore.Setup(CompositionManager.GlobalStoreName, CompositionManager.Instance.GlobalStore, false, false);
			}
		}

		private void DrawGlobalStore()
		{
			if (_globalStore != null)
			{
				_globalStore.Draw();

				if (_globalStore.Selected != null)
					AddWatch(_globalStore.SelectedName, _globalStore.Selected);
			}
		}

		#endregion

		#region Instruction Stores

		private void UpdateInstructionStores()
		{
			if (CompositionManager.Exists)
			{
				for (var i = 0; i < _instructionStores.Count; i++)
				{
					if (!CompositionManager.InstructionState.Any(data => data.Key.Variables == _instructionStores[i].Store))
						_instructionStores.RemoveAt(i--);
				}

				foreach (var instruction in CompositionManager.InstructionState)
				{
					if (!_instructionStores.Any(control => control.Store == instruction.Key.Variables))
					{
						var control = new VariableStoreControl();
						control.Setup(instruction.Key.name, instruction.Key.Variables, true, false);
						_instructionStores.Add(control);
					}
				}
			}
		}

		private void DrawInstructionStores()
		{
			foreach (var instruction in _instructionStores)
			{
				instruction.Draw();

				if (instruction.Selected != null)
					AddWatch(instruction.SelectedName, instruction.Selected);
			}
		}

		#endregion

		#region Watched Stores

		private void AddWatch(string name, IVariableStore store)
		{
			var control = new VariableStoreControl();
			control.Setup(name, store, false, true);
			_watchedStores.Add(control);
		}

		private void RemoveWatch(VariableStoreControl control)
		{
			_watchedStores.RemoveAll(c => c == control);
		}

		private void DrawWatchedStores()
		{
			var name = string.Empty;
			IVariableStore add = null;
			VariableStoreControl remove = null;

			foreach (var watched in _watchedStores)
			{
				watched.Draw();

				if (watched.Selected != null)
				{
					name = watched.SelectedName;
					add = watched.Selected;
				}

				if (watched.ShouldClose)
					remove = watched;
			}

			if (add != null)
				AddWatch(name, add);

			if (remove != null)
				RemoveWatch(remove);
		}

		#endregion

		#region Toolbar

		private class WatchMenu : PopupWindowContent
		{
			public WatchWindow Window;

			private const float _padding = 5.0f;
			private string _variable = string.Empty;
			private bool _valid = true;

			private static readonly Label _addButton = new Label(Icon.BuiltIn(Icon.Add), "", "Add the variable to the watch window");

			public override Vector2 GetWindowSize()
			{
				return new Vector2(300, RectHelper.LineHeight + 2 * _padding);
			}

			public override void OnGUI(Rect rect)
			{
				rect = RectHelper.Inset(rect, _padding);

				var add = false;
				var addRect = RectHelper.TakeTrailingWidth(ref rect, 32.0f);
				RectHelper.TakeTrailingWidth(ref rect, RectHelper.HorizontalSpace);

				using (new InvalidScope(_valid))
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						add = EnterField.DrawString("AddWatchVariable", rect, GUIContent.none, ref _variable);

						if (changes.changed)
							_valid = true;
					}
				}

				if (GUI.Button(addRect, _addButton.Content))
					add = true;

				if (add)
				{
					_valid = Window.AddWatch(_variable);

					if (_valid)
					{
						_variable = string.Empty;
						editorWindow.Close();
					}
					else
					{
						editorWindow.Repaint();
					}
				}
			}
		}

		private bool AddWatch(string variable)
		{
			var reference = new VariableReference { Variable = variable };

			foreach (var instruction in CompositionManager.InstructionState)
			{
				var value = reference.GetValue(instruction.Key.Variables);
				if (!value.IsEmpty)
				{
					if (value.HasStore)
					{
						AddWatch(variable, value.Store);
						return true;
					}
					else
					{
						Debug.LogWarningFormat(_invalidWatchWarning, variable, value.Type);
						return false;
					}
				}
			}

			if (CompositionManager.Exists)
			{
				var value = reference.GetValue(CompositionManager.Instance.GlobalStore);

				if (value.HasStore)
				{
					AddWatch(variable, value.Store);
					return true;
				}
				else
				{
					Debug.LogWarningFormat(_invalidWatchWarning, variable, value.Type);
					return false;
				}
			}

			Debug.LogWarningFormat(_missingWatchWarning, variable);
			return false;
		}

		private void SetupToolbar()
		{
			_watchMenu = new WatchMenu { Window = this };
		}

		private void TeardownToolbar()
		{
			_watchMenu = null;
		}

		private void DrawToolbar()
		{
			using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
			{
				using (new EditorGUI.DisabledScope(!Application.isPlaying))
				{
					if (GUILayout.Button("Watch", EditorStyles.toolbarDropDown, GUILayout.Width(_watchButtonWidth)))
						PopupWindow.Show(new Rect(_toolbarPadding, _toolbarHeight, 0.0f, 0.0f), _watchMenu);
				}

				GUILayout.FlexibleSpace();

				CompositionManager.LogInstructions = GUILayout.Toggle(CompositionManager.LogInstructions, CompositionManager.LogInstructions ? _disableLogInstructionsButton.Content : _enableLogInstructionsButton.Content, EditorStyles.toolbarButton);
				_logInstructionsEnabled.Value = CompositionManager.LogInstructions;
			}
		}

		#endregion

		#region Prompt

		private void DrawPrompt()
		{
			var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
			var execute = false;
			var executeRect = RectHelper.TakeTrailingWidth(ref rect, 32.0f);
			RectHelper.TakeTrailingWidth(ref rect, RectHelper.HorizontalSpace);

			using (new EditorGUI.DisabledScope(!Application.isPlaying))
			{
				using (new InvalidScope(_promptValid))
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						execute = EnterField.DrawString("PromptEntry", rect, GUIContent.none, ref _promptText);

						if (changes.changed)
							_promptValid = true;
					}
				}

				if (GUI.Button(executeRect, _executeButton.Content))
					execute = true;

				if (execute)
				{
					_promptValid = ExecuteExpression(_promptText);

					if (_promptValid)
						_promptText = string.Empty;
				}
			}
		}

		private bool ExecuteExpression(string text)
		{
			var expression = new Expression();
			var compilation = expression.SetStatement(text);

			if (!compilation.Success)
			{
				Debug.LogError(compilation.Message);
				return false;
			}
			else
			{
				var result = VariableValue.Empty;
				var instruction = CompositionManager.InstructionState.FirstOrDefault();

				if (instruction.Key != null)
					result = expression.Execute(instruction.Value.Instruction, instruction.Key.Variables);
				else
					result = expression.Execute(null, CompositionManager.Instance.GlobalStore);

				Debug.LogFormat(_expressionResultLog, expression.Statement, result.Type, result);
				return !result.IsEmpty;
			}
		}

		#endregion
	}
}
