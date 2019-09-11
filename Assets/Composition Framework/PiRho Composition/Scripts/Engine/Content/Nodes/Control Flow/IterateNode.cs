using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Control Flow/Iterate", 21)]
	[HelpURL(Configuration.DocumentationUrl + "iterate-node")]
	public class IterateNode : GraphNode
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

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (Loop != null)
			{
				if (variables.Resolve(this, Container, out IVariableList list))
				{
					for (var i = 0; i < list.VariableCount; i++)
					{
						var item = list.GetVariable(i);
						yield return SetValues(graph, variables, i, item);
					}
				}
				else if (variables.Resolve(this, Container, out IVariableDictionary dictionary))
				{
					var names = dictionary.VariableNames;
					for (var i = 0; i < names.Count; i++)
					{
						var name = names[i];
						var item = dictionary.GetVariable(name);

						yield return SetValues(graph, variables, i, item);
					}
				}
			}

			yield break;
		}

		private IEnumerator SetValues(IGraphRunner graph, IVariableCollection variables, int iteration, Variable item)
		{
			if (Index.IsAssigned)
				Index.SetValue(variables, Variable.Int(iteration));

			if (Value.IsAssigned)
				Value.SetValue(variables, item);

			yield return graph.Run(Loop, variables, nameof(Loop));
		}
	}
}