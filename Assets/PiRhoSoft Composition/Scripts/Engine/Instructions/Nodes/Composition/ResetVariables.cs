using PiRhoSoft.UtilityEngine;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class ResetVariableList : SerializedList<string> { }

	[CreateInstructionGraphNodeMenu("Composition/Reset Variables", 20)]
	[HelpURL(Composition.DocumentationUrl + "reset-variables")]
	public class ResetVariables : InstructionGraphNode
	{
		private const string _invalidVariablesWarning = "(CCRTTNF) Unable to reset variables for {0}: the given variables must be an IVariableReset";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The list of variables to reset")]
		[ListDisplay(AllowCollapse = false, EmptyText = "No variables will be reset")]
		public ResetVariableList Variables = new ResetVariableList();

		public override Color NodeColor => Colors.ExecutionDark;

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (variables.This is IVariableReset reset)
				reset.ResetVariables(Variables);
			else
				Debug.LogWarningFormat(this, _invalidVariablesWarning, Name);

			graph.GoTo(Next, variables.This, nameof(Next));

			yield break;
		}
	}
}
