using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Unload Scene", 101)]
	[HelpURL(Configuration.DocumentationUrl + "unload-scene-node")]
	public sealed class UnloadSceneNode : GraphNode
	{
		private const string _invalidIndexError = "(CUSNII) Unable to unload scene on node '{0}': the index '{1}' is not a valid scene - make sure the scene has been added to the build settings";
		private const string _invalidNameError = "(CUSNIN) Unable to unload scene on node '{0}': a scene with name '{1}' is not loaded - make sure the scene exists and has been added to the build settings";
		private const string _invalidSceneWarning = "(CUSNIS) Unable to unload scene for node '{0}': the scene '{1}' could not be found";
		private const string _unloadedIndexWarning = "(CUSNUI) Unable to unload scene on node '{0}': the scene with index '{1}' is not loaded";
		private const string _lastSceneWarning = "(CUSNLS) Unable to unload scene for node '{0}': the scene '{1}' is the only loaded scene";

		public enum SceneSource
		{
			Value,
			Variable,
			Name,
			Index,
		}

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The source of the scene to unload")]
		[EnumButtons]
		public SceneSource Source = SceneSource.Value;

		[Tooltip("The Scene to unload")]
		[Conditional(nameof(Source), (int)SceneSource.Value)]
		[ScenePicker(nameof(SetupScene))]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The variable reference that stores the scene to unload (can be an index or a name")]
		[Conditional(nameof(Source), (int)SceneSource.Variable)]
		public VariableLookupReference SceneVariable = new VariableLookupReference();

		[Tooltip("The name of the scene to unload")]
		[Conditional(nameof(Source), (int)SceneSource.Name)]
		public string SceneName;

		[Tooltip("The index of the scene to unload")]
		[Conditional(nameof(Source), (int)SceneSource.Index)]
		public int SceneIndex;

		[Tooltip("Whether to wait for Scene to finish unloading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (WaitForCompletion)
				yield return UnloadScene(variables);
			else
				CompositionManager.Instance.StartCoroutine(UnloadScene(variables));

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator UnloadScene(IVariableCollection variables)
		{
			var loadStatus = Unload(variables);

			if (loadStatus != null)
			{
				while (!loadStatus.isDone)
					yield return null;

				if (CleanupAssets)
				{
					var unloadStatus = Resources.UnloadUnusedAssets();

					while (!unloadStatus.isDone)
						yield return null;
				}
			}
		}

		private AsyncOperation Unload(IVariableCollection variables)
		{
			switch (Source)
			{
				case SceneSource.Value: return Unload(Scene.Index);
				case SceneSource.Name: return Unload(SceneName);
				case SceneSource.Index: return Unload(SceneIndex);
				case SceneSource.Variable:
				{
					var value = SceneVariable.GetValue(variables);
					if (value.TryGetInt(out var index)) return Unload(index);
					else if (value.TryGetString(out var name)) return Unload(name);
					else Debug.LogWarningFormat(this, _invalidSceneWarning, name, SceneVariable);
					break;
				}
			}

			return null;
		}

		private AsyncOperation Unload(int buildIndex)
		{
			Scene scene;

			try
			{
				scene = SceneManager.GetSceneByBuildIndex(buildIndex);
			}
			catch
			{
				// exception is thrown when the scene is not in the build
				Debug.LogErrorFormat(this, _invalidIndexError, name, buildIndex);
				return null;
			}

			if (scene.IsValid())
			{
				if (SceneManager.sceneCount > 1)
					return SceneManager.UnloadSceneAsync(buildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				else
					Debug.LogWarningFormat(this, _lastSceneWarning, name, buildIndex);
			}
			else
			{
				// invalid scene is returned when the scene is not loaded
				Debug.LogErrorFormat(this, _unloadedIndexWarning, name, buildIndex);
			}

			return null;
		}

		private AsyncOperation Unload(string sceneName)
		{
			var scene = SceneManager.GetSceneByName(sceneName);

			if (scene.IsValid())
			{
				if (SceneManager.sceneCount > 1)
					return SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				else
					Debug.LogWarningFormat(this, _lastSceneWarning, name, sceneName);
			}
			else
			{
				// GetSceneByName does not differentiate between not loaded and not in build
				Debug.LogErrorFormat(this, _invalidNameError, name, sceneName);
			}

			return null;
		}

		#region SceneReference Maintenance

		void OnEnable()
		{
			Scene.Setup(this);
		}

		void OnDestroy()
		{
			Scene.Teardown();
		}

		private static void SetupScene()
		{
			new GameObject("MainCamera", typeof(Camera), typeof(TransitionRenderer));
		}

		#endregion
	}
}
