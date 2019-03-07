using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Unload Scene", 101)]
	[HelpURL(Composition.DocumentationUrl + "unload-scene-node")]
	public class UnloadSceneNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Scene to unload")]
		[SceneReference("New Scene", "SetupScene")]
		public SceneReference Scene = new SceneReference();

		[Tooltip("Whether to wait for Scene to finish unloading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				InstructionManager.Instance.StartCoroutine(UnloadScene());
			else
				yield return UnloadScene();

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator UnloadScene()
		{
			var loadStatus = SceneManager.UnloadSceneAsync(Scene.Index, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

			while (!loadStatus.isDone)
				yield return null;

			if (CleanupAssets)
			{
				var unloadStatus = Resources.UnloadUnusedAssets();

				while (!unloadStatus.isDone)
					yield return null;
			}
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
			var cameraObject = new GameObject("MainCamera");
			cameraObject.AddComponent<Camera>();
			cameraObject.AddComponent<TransitionRenderer>();
		}

		#endregion
	}
}
