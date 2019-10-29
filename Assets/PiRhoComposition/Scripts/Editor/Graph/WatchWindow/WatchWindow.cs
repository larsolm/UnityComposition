using PiRhoSoft.Utilities.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	public class WatchWindowElement : VisualElement
	{
		private const string _missingWatchWarning = "(CWWMW) Unable to find variable '{0}' to watch";
		private const string _invalidWatchWarning = "(CWWIW) Unable to watch variable '{0}' of type '{1}' - only variable collections can be watched";
		private const string _expressionResultLog = "{0}: ({1}) {2}";

		public const string Stylesheet = "Graph/WatchWindow/WatchWindow.uss";
		public const string UssClassName = "pirho-watch-window";

		public const string UssToolbarClassName = UssClassName + "__toolbar";
		public const string UssWatchClassName = UssClassName + "__watch";
		public const string UssWatchInvalidClassName = UssWatchClassName + "--invalid";
		public const string UssWatchButtonClassName = UssWatchClassName + "__button";
		public const string UssLoggingClassName = UssClassName + "__logging";
		public const string UssLoggingActiveClassName = UssLoggingClassName + "--active";
		public const string UssContainerClassName = UssClassName + "__main-container";
		public const string UssGlobalClassName = UssClassName + "__global-container";
		public const string UssCollectionsClassName = UssClassName + "__collections-container";
		public const string UssWatchedClassName = UssClassName + "__watched-container";
		public const string UssFooterClassName = UssClassName + "__footer";
		public const string UssExpressionClassName = UssClassName + "__expression";
		public const string UssExecuteButtonClassName = UssExpressionClassName + "__execute";
		public const string UssExpressionInvalidClassName = UssExpressionClassName + "--invalid";

		private static readonly Icon _logIcon = Icon.BuiltIn("UnityEditor.ConsoleWindow");
		private static readonly Icon _executeIcon = Icon.BuiltIn("Animation.Play");
		
		private static readonly BoolPreference _logGraphEnabled = new BoolPreference("PiRhoSoft.Composition.CompositionManager.LogGraphEnabled", false);

		private readonly VisualElement _globalContainer;
		private readonly VisualElement _collectionsContainer;
		private readonly VisualElement _watchedContainer;

		public WatchWindowElement()
		{
			var container = new ScrollView(ScrollViewMode.Vertical);
			container.AddToClassList(UssContainerClassName);

			var toolbar = CreateToolbar();
			_globalContainer = CreateGlobalContainer();
			_collectionsContainer = CreateCollectionsContainer();
			_watchedContainer = CreateWatchedContainer();
			var footer = CreateFooter();

			container.Add(_globalContainer);
			container.Add(_collectionsContainer);
			container.Add(_watchedContainer);

			Add(toolbar);
			Add(container);
			Add(footer);

			UpdateCollections();

			CompositionManager.LogTracking = _logGraphEnabled.Value;
			EditorApplication.playModeStateChanged += PlayModeStateChanged;

			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);
		}

		private void PlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
				ClearCollections();
			else if (state == PlayModeStateChange.EnteredPlayMode)
				UpdateCollections();
		}

		private VisualElement CreateToolbar()
		{
			var watchField = new TextField { tooltip = "Type a VariableReference to watch" };
			watchField.AddToClassList(UssWatchClassName);

			void watch()
			{
				if (string.IsNullOrWhiteSpace(watchField.text))
				{
					var valid = AddWatch(watchField.text);
					if (valid)
						watchField.SetValueWithoutNotify(string.Empty);

					EnableInClassList(UssWatchInvalidClassName, !valid);
				}
			};

			watchField.RegisterCallback<KeyDownEvent>(evt =>
			{
				if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return)
					watch();
			});

			var placeholder = new PlaceholderControl("Add watch");
			placeholder.AddToField(watchField);

			var watchButton = new IconButton(Icon.View.Texture, "Watch this variable", watch);
			watchButton.AddToClassList(UssWatchButtonClassName);

			var loggingButton = new Image { image = _logIcon.Texture, tooltip = "Enable/Disable logging of graph statistics" };
			loggingButton.AddToClassList(UssLoggingClassName);
			loggingButton.AddManipulator(new Clickable(() =>
			{
				CompositionManager.LogTracking = !CompositionManager.LogTracking;
				_logGraphEnabled.Value = CompositionManager.LogTracking;
				EnableInClassList(UssLoggingActiveClassName, CompositionManager.LogTracking);
			}));

			var toolbar = new Toolbar();
			toolbar.AddToClassList(UssToolbarClassName);
			toolbar.Add(watchField);
			toolbar.Add(watchButton);
			toolbar.Add(loggingButton);

			return toolbar;
		}

		private VisualElement CreateGlobalContainer()
		{
			var globalContainer = new VisualElement();
			globalContainer.AddToClassList(UssGlobalClassName);
			globalContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Variables));

			return globalContainer;
		}

		private VisualElement CreateCollectionsContainer()
		{
			var collectionContainer = new VisualElement();
			collectionContainer.AddToClassList(UssCollectionsClassName);
			collectionContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Variables));

			return collectionContainer;
		}

		private VisualElement CreateWatchedContainer()
		{
			var watchedContainer = new VisualElement();
			watchedContainer.AddToClassList(UssWatchedClassName);
			watchedContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Variables));

			return watchedContainer;
		}

		private VisualElement CreateFooter()
		{
			var expressionText = new TextField { tooltip = "Type an expression to execute" };
			expressionText.AddToClassList(UssExpressionClassName);

			void execute()
			{
				if (string.IsNullOrWhiteSpace(expressionText.text))
				{
					var valid = ExecuteExpression(expressionText.text);
					if (valid)
						expressionText.SetValueWithoutNotify(string.Empty);

					EnableInClassList(UssExpressionInvalidClassName, !valid);
				}
			};

			expressionText.RegisterCallback<KeyDownEvent>(evt =>
			{
				if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return)
					execute();
			});

			var executeButton = new IconButton(_executeIcon.Texture, "Execute this expression", execute);
			executeButton.AddToClassList(UssExecuteButtonClassName);

			var placeholder = new PlaceholderControl("Execute Expression");
			placeholder.AddToField(expressionText);

			var footer = new Toolbar();
			footer.AddToClassList(UssFooterClassName);
			footer.Add(expressionText);
			footer.Add(executeButton);

			return footer;
		}

		private void ClearCollections()
		{
			_globalContainer.Clear();
			_collectionsContainer.Clear();
			_watchedContainer.Clear();
		}

		private void UpdateCollections()
		{
			ClearCollections();

			if (CompositionManager.Exists)
			{
				_globalContainer.Add(new VariableDictionaryWatchControl(CompositionManager.GlobalStoreName, CompositionManager.Instance.GlobalStore, true));

				foreach (var graph in CompositionManager.TrackingState.Keys)
					_collectionsContainer.Add(new VariableDictionaryWatchControl(graph.name, graph.Variables, true));
			}
		}

		private bool AddWatch(string variable)
		{
			var reference = new VariableLookupReference { Variable = variable };

			foreach (var graph in CompositionManager.TrackingState.Keys)
			{
				var value = reference.GetValue(graph.Variables);
				if (!value.IsEmpty)
				{
					if (value.TryGetDictionary(out var collection))
					{
						AddWatch(variable, collection);
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
				var value = reference.GetValue(CompositionManager.Instance.DefaultStore);

				if (value.TryGetDictionary(out var collection))
				{
					AddWatch(variable, collection);
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

		private void AddWatch(string name, IVariableMap variables)
		{
			_watchedContainer.Add(new VariableDictionaryWatchControl(name, variables, true));
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
				var result = Variable.Empty;
				var graph = CompositionManager.TrackingState.FirstOrDefault();

				if (graph.Key != null)
					result = expression.Execute(graph.Value.Graph, graph.Key.Variables);
				else
					result = expression.Execute(null, CompositionManager.Instance.DefaultStore);

				Debug.LogFormat(_expressionResultLog, expression.Statement, result.Type, result);
				return true;
			}
		}

		private class VariableDictionaryWatchControl : VisualElement
		{
			public VariableDictionaryWatchControl(string name, IVariableMap variables, bool allowClose)
			{
				var proxy = new WatchProxy(name, variables);
				var list = new ListControl(proxy);

				if (allowClose)
					list.Header.Add(new IconButton(Icon.Close.Texture, "Close this watch", RemoveFromHierarchy));

				Add(list);
			}

			private class WatchProxy : IListProxy
			{
				public IVariableMap Variables { get; }
				public string Label { get; }
				public string Tooltip => "The variables in this map";
				public string EmptyLabel => "This variable map is empty";
				public string EmptyTooltip => "There are no variables in this map";
				public string AddTooltip => string.Empty;
				public string RemoveTooltip => string.Empty;
				public string ReorderTooltip => string.Empty;

				public int ItemCount => Variables.VariableNames.Count;

				public bool AllowAdd => false;
				public bool AllowRemove => false;
				public bool AllowReorder => false;

				public bool CanAdd() => false;
				public bool CanRemove(int index) => false;
				public bool CanReorder(int from, int to) => false;

				public void AddItem() { }
				public void RemoveItem(int index) { }
				public void ReorderItem(int from, int to) { }
				
				public WatchProxy(string name, IVariableMap variables)
				{
					Label = name;
					Variables = variables;
				}

				public VisualElement CreateElement(int index)
				{
					var name = Variables.VariableNames[index];
					var variable = Variables.GetVariable(name);

					var container = new VisualElement();
					container.Add(new Label(name));

					if (variable.IsEmpty)
					{
						container.Add(new Label("(empty)"));
					}
					else
					{
						if (variable.TryGetCollection(out var variables))
						{
							container.Add(new IconButton(Icon.View.Texture, "View the contents of the store", () =>
							{
								using (var evt = WatchWindow.WatchEvent.GetPooled(variables, name))
								{
									evt.target = container;
									container.SendEvent(evt);
								}
							}));
						}

						var control = new VariableControl(variable, new VariableDefinition(name, variable.Type), null);
						container.Add(control);
					}

					return container;
				}

				public bool NeedsUpdate(VisualElement item, int index)
				{
					return true;
				}
			}
		}
	}

	public class WatchWindow : EditorWindow
	{
		public class WatchEvent : EventBase<WatchEvent>
		{
			public IVariableMap Variables { get; private set; }
			public string Name { get; private set; }

			public static WatchEvent GetPooled(IVariableMap variables, string name)
			{
				var pooled = GetPooled();
				pooled.Variables = variables;
				pooled.Name = name;
				return pooled;
			}
		}

		[MenuItem("Window/PiRho Composition/Watch Window")]
		public static void ShowWindow()
		{
			var window = GetWindow<WatchWindow>("Watch Window");
			window.minSize = new Vector2(200.0f, window.minSize.y);
			window.titleContent.image = Icon.View.Texture;
			window.Show();
		}

		private void OnEnable()
		{
			rootVisualElement.Add(new WatchWindowElement());
		}
	}
}
