using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Load Scene", 100)]
	[HelpURL(Configuration.DocumentationUrl + "load-scene-node")]
	public class LoadSceneNode : GraphNode
	{
		private const string _invalidSceneWarning = "(CLSNIS) Unable to load scene on node '{0}': the scene '{1}' could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The Scene to load")]
		public SceneVariableSource Scene = new SceneVariableSource();

		[Tooltip("Whether to wait for Scene to finish loading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		[Tooltip("Whether to load the scene additive or not")]
		public bool Additive = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (WaitForCompletion)
				yield return LoadScene(variables);
			else
				CompositionManager.Instance.StartCoroutine(LoadScene(variables));

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator LoadScene(IVariableMap variables)
		{
			if (variables.Resolve(this, Scene, out var scene))
			{
				var mode = Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;
				var loadStatus = Addressables.LoadSceneAsync(scene.RuntimeKey, mode);

				if (loadStatus.IsValid())
				{
					while (!loadStatus.IsDone)
						yield return null;

					if (CleanupAssets)
					{
						var unloadStatus = Resources.UnloadUnusedAssets();

						while (!unloadStatus.isDone)
							yield return null;
					}
				}
				else
				{
					Debug.LogErrorFormat(this, _invalidSceneWarning, name, Scene);
				}
			}
		}
	}
}
