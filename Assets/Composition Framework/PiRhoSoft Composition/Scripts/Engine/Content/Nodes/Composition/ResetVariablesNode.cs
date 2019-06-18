using PiRhoSoft.PargonUtilities.Engine;
using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class ResetVariableList : SerializedList<string> { }

	[CreateInstructionGraphNodeMenu("Composition/Reset Variables Node", 30)]
	[HelpURL(Composition.DocumentationUrl + "reset-variables-node")]
	public class ResetVariablesNode : InstructionGraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The object containing the variables to reset")]
		public VariableReference Object;

		[Tooltip("The list of variables to reset")]
		[ListDisplay(AllowCollapse = false, EmptyText = "No variables will be reset")]
		public ResetVariableList Variables = new ResetVariableList();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveInterface(variables, Object, out IVariableReset reset))
				reset.ResetVariables(Variables);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
