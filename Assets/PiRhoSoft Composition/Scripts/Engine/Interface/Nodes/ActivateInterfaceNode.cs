using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "activate-interface-node")]
	[CreateInstructionGraphNodeMenu("Interface/Activate Interface", 50)]
	public class ActivateInterfaceNode : InstructionGraphNode, IImmediate
	{
		private const string _invalidInterfaceNameWarning = "(CISNIIN) Unable to activate interface for {0}: the interface '{1}' does not exist";

		[Tooltip("The node to go to once the control is shown")]
		public InstructionGraphNode Next = null;

		[Tooltip("The name of the interface to activate")]
		public string InterfaceName;

		public override Color GetNodeColor()
		{
			return new Color(0.0f, 0.0f, 0.45f);
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var inter = InterfaceManager.Instance.GetInterface<Interface>(InterfaceName);
			if (inter)
				inter.Activate();
			else
				Debug.LogWarningFormat(this, _invalidInterfaceNameWarning, Name, InterfaceName);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
