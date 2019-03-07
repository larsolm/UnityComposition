using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Game Object", 21)]
	[HelpURL(Composition.DocumentationUrl + "disable-game-object-node")]
	public class DisableGameObjectNode : InstructionGraphNode
	{
		private const string _missingObjectWarning = "(COMDGONMO) Unable to disable object for {0}: the given variables must be a GameObject";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is GameObject target)
				target.SetActive(false);
			else
				Debug.LogWarningFormat(this, _missingObjectWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
