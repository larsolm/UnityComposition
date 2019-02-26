using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Game Object", 21)]
	[HelpURL(Composition.DocumentationUrl + "disable-game-object-node")]
	public class DisableGameObjectNode : InstructionGraphNode, IImmediate
	{
		private const string _missingObjectWarning = "(CDGONMO) failed to disable object {0}: the object could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is GameObject target)
				target.SetActive(false);
			else
				Debug.LogWarningFormat(this, _missingObjectWarning, This);

			graph.GoTo(Next, variables.This, nameof(Next));
			yield break;
		}
	}
}
