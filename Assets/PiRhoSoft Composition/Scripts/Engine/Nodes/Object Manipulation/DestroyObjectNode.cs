using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Destroy Object", 1)]
	[HelpURL(Composition.DocumentationUrl + "destroy-object-node")]
	public class DestroyObjectNode : InstructionGraphNode
	{
		private const string _objectNotFoundWarning = "(COMDOONF) Unable to destroy object for {0}: the given variables must be a Unity Object";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.Root is Object obj)
				Destroy(obj);
			else
				Debug.LogWarningFormat(this, _objectNotFoundWarning, Name);

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
