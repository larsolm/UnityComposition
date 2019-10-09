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
		public const string UssLoggingClassName = UssClassName + "__logging";
		public const string UssLoggingActiveClassName = UssLoggingClassName + "--active";
		public const string UssContainerClassName = UssClassName + "__main-container";
		public const string UssGlobalClassName = UssClassName + "__global-container";
		public const string UssCollectionsClassName = UssClassName + "__collections-container";
		public const string UssWatchedClassName = UssClassName + "__watched-container";
		public const string UssFooterClassName = UssClassName + "__footer";
		public const string UssExpressionClassName = UssClassName + "__expression";
		public const string UssExpressionInvalidClassName = UssExpressionClassName + "--invalid";

		private static readonly Icon _logIcon = Icon.BuiltIn("UnityEditor.ConsoleWindow");
		private static readonly BoolPreference _logGraphEnabled = new BoolPreference("PiRhoSoft.Composition.CompositionManager.LogGraphEnabled", false);

		private VisualElement _globalContainer;
		private VisualElement _collectionsContainer;
		private VisualElement _watchedContainer;
		private TextField _expressionText;

		public WatchWindowElement()
		{
			this.AddStyleSheet(Configuration.EditorPath, Stylesheet);
			AddToClassList(UssClassName);

			var container = new VisualElement();
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
		}

		private void PlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
				ClearCollections();
			else if (state == PlayModeStateChange.EnteredPlayMode)
				UpdateCollections();

			_expressionText.SetEnabled(state == PlayModeStateChange.EnteredPlayMode);
		}

		private VisualElement CreateToolbar()
		{
			var watchField = new TextField { isDelayed = true, tooltip = "Type a VariableReference to watch" };
			watchField.AddToClassList(UssWatchClassName);
			watchField.RegisterValueChangedCallback(evt =>
			{
				var valid = AddWatch(evt.newValue);
				if (valid)
					watchField.SetValueWithoutNotify(string.Empty);

				watchField.EnableInClassList(UssWatchInvalidClassName, !valid);
			});

			var placeholder = new PlaceholderControl("Add watch");
			placeholder.AddToField(watchField);

			var loggingButton = new Image { image = _logIcon.Texture, tooltip = "Enable/Disable logging of graph statistics" };
			loggingButton.AddToClassList(UssLoggingClassName);
			loggingButton.AddManipulator(new Clickable(() =>
			{
				CompositionManager.LogTracking = !CompositionManager.LogTracking;
				_logGraphEnabled.Value = CompositionManager.LogTracking;
				loggingButton.EnableInClassList(UssLoggingActiveClassName, CompositionManager.LogTracking);
			}));

			var toolbar = new Toolbar();
			toolbar.AddToClassList(UssToolbarClassName);
			toolbar.Add(watchField);
			toolbar.Add(loggingButton);

			return toolbar;
		}

		private VisualElement CreateGlobalContainer()
		{
			var globalContainer = new VisualElement();
			globalContainer.AddToClassList(UssGlobalClassName);
			globalContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Collection));

			return globalContainer;
		}

		private VisualElement CreateCollectionsContainer()
		{
			var collectionContainer = new VisualElement();
			collectionContainer.AddToClassList(UssCollectionsClassName);
			collectionContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Collection));

			return collectionContainer;
		}

		private VisualElement CreateWatchedContainer()
		{
			var watchedContainer = new VisualElement();
			watchedContainer.AddToClassList(UssWatchedClassName);
			watchedContainer.RegisterCallback<WatchWindow.WatchEvent>(evt => AddWatch(evt.Name, evt.Collection));

			return watchedContainer;
		}

		private VisualElement CreateFooter()
		{
			_expressionText = new TextField { isDelayed = true, tooltip = "Type an Expression to execute" };
			_expressionText.AddToClassList(UssExpressionClassName);
			_expressionText.RegisterValueChangedCallback(evt =>
			{
				var valid = ExecuteExpression(evt.newValue);
				if (valid)
					_expressionText.SetValueWithoutNotify(string.Empty);

				_expressionText.EnableInClassList(UssExpressionInvalidClassName, !valid);
			});

			var placeholder = new PlaceholderControl("Execute Expression");
			placeholder.AddToField(_expressionText);

			var footer = new Toolbar();
			footer.AddToClassList(UssFooterClassName);
			footer.Add(_expressionText);

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
				_globalContainer.Add(new VariableCollectionControl(CompositionManager.GlobalStoreName, CompositionManager.Instance.GlobalStore, false, false));

				foreach (var graph in CompositionManager.TrackingState.Keys)
					_collectionsContainer.Add(new VariableCollectionControl(graph.name, graph.Variables, true, false));
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

		private void AddWatch(string name, IVariableCollection collection)
		{
			_watchedContainer.Add(new VariableCollectionControl(name, collection, false, true));
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
	}

	public class WatchWindow : EditorWindow
	{
		public class WatchEvent : EventBase<WatchEvent>
		{
			public IVariableCollection Collection { get; private set; }
			public string Name { get; private set; }

			public static WatchEvent GetPooled(IVariableCollection collection, string name)
			{
				var pooled = GetPooled();
				pooled.Collection = collection;
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
