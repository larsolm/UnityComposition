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
		private const string _invalidSceneWarning = "(CNSUS) Unable to unload scene for {0}: the scene '{1}' could not be found. Make sure this variable refers to an int or a string";

		public enum SceneSource
		{
			Value,
			Variable,
			Name,
			Index,
		}

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The source of the scene to unload")]
		[EnumDisplay]
		public SceneSource Source = SceneSource.Value;

		[Tooltip("The Scene to unload")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Value)]
		[SceneReference(SaveLocation = AssetLocation.Selectable, DefaultName = "New Scene", Creator = "SetupScene")]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The variable reference that stores the scene to unload (can be an index or a name")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Variable)]
		public VariableReference SceneVariable = new VariableReference();

		[Tooltip("The name of the scene to unload")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Name)]
		public string SceneName;

		[Tooltip("The index of the scene to unload")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Index)]
		public int SceneIndex;

		[Tooltip("Whether to wait for Scene to finish unloading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				yield return UnloadScene(variables);
			else
				CompositionManager.Instance.StartCoroutine(UnloadScene(variables));

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator UnloadScene(InstructionStore variables)
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

		private AsyncOperation Unload(InstructionStore variables)
		{
			switch (Source)
			{
				case SceneSource.Value: return SceneManager.UnloadSceneAsync(Scene.Index, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				case SceneSource.Name: return SceneManager.UnloadSceneAsync(SceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				case SceneSource.Index: return SceneManager.UnloadSceneAsync(SceneIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				case SceneSource.Variable:
				{
					var value = SceneVariable.GetValue(variables);
					if (value.TryGetInt(out var index)) return SceneManager.UnloadSceneAsync(index, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
					else if (value.TryGetString(out var name)) return SceneManager.UnloadSceneAsync(name, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
					else Debug.LogWarningFormat(this, _invalidSceneWarning, Name, SceneVariable);
					break;
				}
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
			var cameraObject = new GameObject("MainCamera");
			cameraObject.AddComponent<Camera>();
			cameraObject.AddComponent<TransitionRenderer>();
		}

		#endregion
	}
}
