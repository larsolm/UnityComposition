using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Sequencing/Load Scene", 100)]
	[HelpURL(Composition.DocumentationUrl + "load-scene-node")]
	public sealed class LoadSceneNode : InstructionGraphNode
	{
		private const string _invalidSceneWarning = "(CNSLS) Unable to load scene for {0}: the scene '{1}' could not be found. Make sure this variable refers to an int or a string";

		public enum SceneSource
		{
			Value,
			Variable,
			Name,
			Index,
		}

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The source of the scene to load")]
		[EnumDisplay]
		public SceneSource Source = SceneSource.Value;

		[Tooltip("The Scene to load")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Value)]
		[SceneReference(SaveLocation = AssetLocation.Selectable, DefaultName = "New Scene", Creator = "SetupScene")]
		public SceneReference Scene = new SceneReference();

		[Tooltip("The variable reference that stores the scene to load (can be an index or a name")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Variable)]
		public VariableReference SceneVariable = new VariableReference();

		[Tooltip("The name of the scene to load")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Name)]
		public string SceneName;

		[Tooltip("The index of the scene to load")]
		[ConditionalDisplaySelf(nameof(Source), EnumValue = (int)SceneSource.Index)]
		public int SceneIndex;

		[Tooltip("Whether to wait for Scene to finish loading before moving to Next")]
		public bool WaitForCompletion = true;

		[Tooltip("Whether to cleanup assets and trigger the GarbageCollector")]
		public bool CleanupAssets = true;

		[Tooltip("Whether to load the Scene additive or not - this should be used with caution as some references could become invalid")]
		public bool Additive = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (WaitForCompletion)
				yield return LoadScene(variables);
			else
				CompositionManager.Instance.StartCoroutine(LoadScene(variables));

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator LoadScene(InstructionStore variables)
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

		private AsyncOperation Load(InstructionStore variables)
		{
			var mode = Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

			switch (Source)
			{
				case SceneSource.Value: return SceneManager.LoadSceneAsync(Scene.Index, mode);
				case SceneSource.Name: return SceneManager.LoadSceneAsync(SceneName, mode);
				case SceneSource.Index: return SceneManager.LoadSceneAsync(SceneIndex, mode);
				case SceneSource.Variable:
				{
					var value = SceneVariable.GetValue(variables);
					if (value.TryGetInt(out var index)) return SceneManager.LoadSceneAsync(index, mode);
					else if (value.TryGetString(out var name)) return SceneManager.LoadSceneAsync(name, mode);
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
			new GameObject("MainCamera", typeof(Camera), typeof(TransitionRenderer));
		}

		#endregion
	}
}
