using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Game Object", 20)]
	[HelpURL(Composition.DocumentationUrl + "enable-game-object-node")]
	public class EnableGameObjectNode : InstructionGraphNode, IImmediate
	{
		private const string _missingObjectWarning = "(CEGONMO) failed to enable object {0}: the object could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is GameObject target)
				target.SetActive(true);
			else
				Debug.LogWarningFormat(this, _missingObjectWarning, This);

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
