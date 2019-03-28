using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : InstructionGraphNode, ILoopNode
	{
		[Tooltip("The variable list containing the objects to iterate")]
		public VariableReference Container = new VariableReference();

		[Tooltip("The variable that will hold the number of times the iteration has run")]
		public VariableReference Index = new VariableReference();

		[Tooltip("The variable to assign the value of each iteration")]
		public VariableReference Value = new VariableReference();

		[Tooltip("The node to go to for each value in the iteration")]
		public InstructionGraphNode Loop = null;

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Loop != null && Resolve(variables, Container, out IVariableList list))
			{
				if (iteration < list.Count)
				{
					var item = list.GetVariable(iteration);

					if (Index.IsAssigned)
						Index.SetValue(variables, VariableValue.Create(iteration));

					if (Value.IsAssigned)
						Value.SetValue(variables, item);

					graph.GoTo(Loop, nameof(Loop));
				}
			}

			yield break;
		}
	}
}
