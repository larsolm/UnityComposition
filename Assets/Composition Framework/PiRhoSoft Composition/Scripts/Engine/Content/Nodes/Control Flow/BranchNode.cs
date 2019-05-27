using PiRhoSoft.UtilityEngine;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Branch", 1)]
	[HelpURL(Composition.DocumentationUrl + "branch-node")]
	public class BranchNode : InstructionGraphNode
	{
		[Tooltip("The expression to evaluate to determine which node to follow")]
		public Expression Switch = new Expression();

		[Tooltip("The node to follow depending on the result of Switch")]
		[DictionaryDisplay(EmptyText = "No outputs")]
		public InstructionGraphNodeDictionary Outputs = new InstructionGraphNodeDictionary();

		[Tooltip("The node to follow if the result of Switch is not in Outputs")]
		public InstructionGraphNode Default;

		public override Color NodeColor => Colors.Branch;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			var value = Switch.Execute(this, variables, VariableType.String);

			if (value.TryGetString(out var name))
			{
				if (Outputs.TryGetValue(name, out var output))
					graph.GoTo(output, nameof(Outputs), name);
				else
					graph.GoTo(Default, nameof(Default));
			}

			yield break;
		}
	}
}
