using PiRhoSoft.Utilities;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[ExecuteInEditMode]
	[HelpURL(Configuration.DocumentationUrl + "graph-component")]
	[AddComponentMenu("PiRho Soft/Composition/Graph Component")]
	public class GraphComponent : MonoBehaviour
	{
		[ReadOnly]
		[Button(nameof(OpenGraph), Label = "Open Graph Window", Location = TraitLocation.After, Tooltip = "Open the graph window for editing")]
		public Graph Graph;

		private void OnEnable()
		{
			if (!Graph)
			{
				Graph = ScriptableObject.CreateInstance<Graph>();
				Graph.name = $"Graph";
			}
		}

		private void OpenGraph()
		{
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.OpenAsset(Graph);
#endif
		}
	}
}