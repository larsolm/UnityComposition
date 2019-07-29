using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class SortConditionList : SerializedList<VariableReference> { }

	[CreateGraphNodeMenu("Composition/Sort", 21)]
	[HelpURL(Composition.DocumentationUrl + "sort-node")]
	public class SortNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The list to sort")]
		public VariableReference List = new VariableReference();

		[Tooltip("Specifies whether to sort each value in the list by itself or by properties on the value (if it is a list of variable stores)")]
		public bool SortByProperty = false;

		[Tooltip("The variables that will determine the order of the list")]
		[Conditional(nameof(SortByProperty), true)]
		[List(EmptyLabel = "Add variables to sort by")]
		public SortConditionList SortConditions = new SortConditionList();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (Resolve(variables, List, out IVariableList variableList) && variableList is VariableList list)
			{
				if (SortByProperty)
				{
					list.Values.Sort((left, right) =>
					{
						foreach (var condition in SortConditions)
						{
							var leftValue = condition.GetValue(left.As<IVariableCollection>());
							var rightValue = condition.GetValue(right.As<IVariableCollection>());

							var compare = VariableHandler.Compare(leftValue, rightValue).GetValueOrDefault(0);
							if (compare != 0)
								return compare;
						}

						return 0;
					});
				}
				else
				{
					list.Values.Sort((left, right) => VariableHandler.Compare(left, right).GetValueOrDefault(0));
				}
			}
			

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
