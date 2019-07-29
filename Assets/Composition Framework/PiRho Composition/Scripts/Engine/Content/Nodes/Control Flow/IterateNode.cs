using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Configuration.DocumentationUrl + "iterate-node")]
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
					if (iteration < list.VariableCount)
					{
						var item = list.GetVariable(iteration);
						SetValues(graph, variables, iteration, item);
					}
				}
				else if (Resolve(variables, Container, out IVariableDictionary dictionary))
				{
					var names = dictionary.VariableNames;
					if (iteration < names.Count)
					{
						var name = names[iteration];
						var item = dictionary.GetVariable(name);

						SetValues(graph, variables, iteration, item);
					}
				}
			}

			yield break;
		}

		private void SetValues(Graph graph, GraphStore variables, int iteration, Variable item)
		{
			if (Index.IsAssigned)
				Index.SetValue(variables, Variable.Int(iteration));

			if (Value.IsAssigned)
				Value.SetValue(variables, item);

			graph.GoTo(Loop, nameof(Loop));
		}
	}
}
