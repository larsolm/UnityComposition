using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Composition.DocumentationUrl + "iterate-node")]
	public class IterateNode : GraphNode, ILoopNode
	{
		[Tooltip("The variable list containing the objects to iterate")]
		public VariableReference Container = new VariableReference();

		[Tooltip("The variable that will hold the number of times the iteration has run")]
		public VariableReference Index = new VariableReference();

		[Tooltip("The variable to assign the value of each iteration")]
		public VariableReference Value = new VariableReference();

		[Tooltip("The node to go to for each value in the iteration")]
		public GraphNode Loop = null;

		public override Color NodeColor => Colors.Loop;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Loop != null)
			{
				if (Resolve(variables, Container, out IVariableList list))
				{
					if (iteration < list.Count)
					{
						var item = list.GetVariable(iteration);
						SetValues(graph, variables, iteration, item);
					}
				}
				else if (Resolve(variables, Container, out IVariableStore store))
				{
					var names = store.GetVariableNames();
					if (iteration < names.Count)
					{
						var name = names[iteration];
						var item = store.GetVariable(name);

						SetValues(graph, variables, iteration, item);
					}
				}
			}

			yield break;
		}

		private void SetValues(Graph graph, GraphStore variables, int iteration, VariableValue item)
		{
			if (Index.IsAssigned)
				Index.SetValue(variables, VariableValue.Create(iteration));

			if (Value.IsAssigned)
				Value.SetValue(variables, item);

			graph.GoTo(Loop, nameof(Loop));
		}
	}
}
