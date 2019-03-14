using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Game Object", 11)]
	[HelpURL(Composition.DocumentationUrl + "disable-game-object-node")]
	public class DisableGameObjectNode : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(COMDGONMO) Unable to disable object for {0}: the given variables must be a GameObject";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is GameObject target)
				target.SetActive(false);
			else
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
