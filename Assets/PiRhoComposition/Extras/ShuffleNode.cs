using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Composition/Shuffle", 20)]
	public class ShuffleNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The list to shuffle")]
		public VariableLookupReference Variable = new VariableLookupReference();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (variables.Resolve(this, Variable, out IVariableList list))
			{
				for (var i = 0; i < list.VariableCount - 2; i++)
				{
					var j = Random.Range(i, list.VariableCount);
					var iValue = list.GetVariable(i);
					var jValue = list.GetVariable(j);

					list.SetVariable(i, jValue);
					list.SetVariable(j, iValue);
				}
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
