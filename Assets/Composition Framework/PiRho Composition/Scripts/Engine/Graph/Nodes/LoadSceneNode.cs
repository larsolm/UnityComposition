using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Sequencing/Load Scene", 100)]
	[HelpURL(Configuration.DocumentationUrl + "load-scene-node")]
	public sealed class LoadSceneNode : GraphNode
	{
		private const string _invalidIndexError = "(CLSNII) Unable to load scene on node '{0}': the index '{1}' is not a valid scene - make sure the scene has been added to the build settings";
		private const string _invalidNameError = "(CLSNIN) Unable to load scene on node '{0}': the name '{1}' is not a valid scene - make sure the scene exists and has been added to the build settings";
		private const string _invalidSceneWarning = "(CLSNIS) Unable to load scene on node '{0}': the variable '{1}' could not be found";

		public enum SceneSource
		{
			Value,
			Variable,
			Name,
			Index,
		}

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The source of the scene to load")]
		[EnumButtons]
		public SceneSource Source = SceneSource.Value;

		[Tooltip("The Scene to load")]
		[Conditional(nameof(Source), (int)SceneSource.Value)]
		[ScenePicker(nameof(SetupScene))]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The variable reference that stores the scene to load (can be an index or a name")]
		[Conditional(nameof(Source), (int)SceneSource.Variable)]
		public VariableLookupReference SceneVariable = new VariableLookupReference();

		[Tooltip("The name of the scene to load")]
		[Conditional(nameof(Source), (int)SceneSource.Name)]
		public string SceneName;

		[Tooltip("The index of the scene to load")]
		[Conditional(nameof(Source), (int)SceneSource.Index)]
		public int SceneIndex;

		[Tooltip("Whether to wait for Scene to finish loading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		[Tooltip("Whether to load the Scene additive or not - this should be used with caution as some references could become invalid")]
		public bool Additive = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (WaitForCompletion)
				yield return LoadScene(variables);
			else
				CompositionManager.Instance.StartCoroutine(LoadScene(variables));

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator LoadScene(IVariableCollection variables)
		{
			var loadStatus = Load(variables);

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

		#region Scene Loading

		private AsyncOperation Load(IVariableCollection variables)
		{
			var mode = Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

			switch (Source)
			{
				case SceneSource.Value: return Load(Scene.Index, mode);
				case SceneSource.Name: return Load(SceneName, mode);
				case SceneSource.Index: return Load(SceneIndex, mode);
				case SceneSource.Variable:
				{
					var value = SceneVariable.GetValue(variables);
					if (value.TryGetInt(out var index)) return Load(index, mode);
					else if (value.TryGetString(out var name)) return Load(name, mode);
					else Debug.LogWarningFormat(this, _invalidSceneWarning, name, SceneVariable);
					break;
				}
			}

			return null;
		}

		private AsyncOperation Load(int buildIndex, LoadSceneMode mode)
		{
			var path = SceneUtility.GetScenePathByBuildIndex(buildIndex);

			if (!string.IsNullOrEmpty(path))
			{
				return SceneManager.LoadSceneAsync(buildIndex, mode);
			}
			else
			{
				Debug.LogErrorFormat(this, _invalidIndexError, name, buildIndex);
				return null;
			}
		}

		private AsyncOperation Load(string sceneName, LoadSceneMode mode)
		{
			var index = SceneUtility.GetBuildIndexByScenePath(sceneName);

			if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
			{
				return SceneManager.LoadSceneAsync(index, mode);
			}
			else
			{
				Debug.LogErrorFormat(this, _invalidNameError, name, sceneName);
				return null;
			}
		}

		#endregion

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
