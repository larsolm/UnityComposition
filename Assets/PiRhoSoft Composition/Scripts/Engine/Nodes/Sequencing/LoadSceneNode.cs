using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Load Scene", 100)]
	[HelpURL(Composition.DocumentationUrl + "load-scene-node")]
	public class LoadSceneNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Scene to load")]
		[SceneReference("New Scene", "SetupScene")]
		public SceneReference Scene = new SceneReference();

		[Tooltip("Whether to wait for Scene to finish loading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		public override Color NodeColor => Colors.ExecutionLight;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				yield return LoadScene();
			else
				InstructionManager.Instance.StartCoroutine(LoadScene());

			graph.GoTo(Next, variables.This, nameof(Next));
		}

		private IEnumerator LoadScene()
		{
			var loadStatus = SceneManager.LoadSceneAsync(Scene.Index, LoadSceneMode.Additive);

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
