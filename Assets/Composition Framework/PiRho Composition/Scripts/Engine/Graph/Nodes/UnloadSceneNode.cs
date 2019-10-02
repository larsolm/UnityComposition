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
		private const string _invalidSceneError = "(CUSNIS) Unable to unload scene on node '{0}': a scene with name '{1}' is not loaded - make sure the scene exists and has been added to the build settings";
		private const string _lastSceneWarning = "(CUSNLS) Unable to unload scene for node '{0}': the scene '{1}' is the only loaded scene";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Scene to unload")]
		public SceneVariableSource Scene = new SceneVariableSource();

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
			if (variables.Resolve(this, Scene, out var scene))
			{
				if (scene.Asset)
				{
					var loadStatus = Unload(scene.Asset.name);

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
				Debug.LogErrorFormat(this, _invalidSceneError, name, sceneName);
			}

			return null;
		}
	}
}
