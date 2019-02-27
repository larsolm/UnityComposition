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

		public override Color GetNodeColor()
		{
			return new Color(0.0f, 0.25f, 0.0f);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is Object obj)
				Destroy(obj);
			else
				Debug.LogWarningFormat(this, _objectNotFoundWarning, Name);

			graph.GoTo(Next, null, nameof(Next));

			yield break;
		}
	}
}
