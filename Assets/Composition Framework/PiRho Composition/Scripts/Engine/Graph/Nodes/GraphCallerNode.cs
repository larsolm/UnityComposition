using PiRhoSoft.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Composition/Graph Caller", 1)]
	[HelpURL(Configuration.DocumentationUrl + "graph-caller-node")]
	public class GraphCallerNode : GraphNode
	{
		public enum GraphSource
		{
			Value,
			Reference
		}

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The source of the graph to run")]
		[EnumButtons]
		public GraphSource Source = GraphSource.Value;

		[Tooltip("The graph to run when this node is reached")]
		[Conditional(nameof(Source), (int)GraphSource.Value)]
		public GraphCaller TargetGraph = new GraphCaller();

		[Tooltip("The graph to run when this node is reached")]
		[VariableReference(typeof(Graph))]
		[Conditional(nameof(Source), (int)GraphSource.Reference)]
		public VariableLookupReference Reference = new VariableLookupReference();

		[Tooltip("The object to use as the root for Graph")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableLookupReference { Variable = "context" } };

		[Tooltip("Whether to wait for the graph to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (!variables.Resolve(this, Context, out var context) && variables is GraphStore store)
				context = store.Context;

			if (Source == GraphSource.Value)
			{
				if (WaitForCompletion)
					yield return TargetGraph.Execute(variables, context);
				else
					CompositionManager.Instance.RunGraph(TargetGraph, variables, context);
			}
			else if (Source == GraphSource.Reference)
			{
				if (variables.ResolveObject(this, Reference, out Graph caller))
				{
					if (WaitForCompletion)
						yield return caller.Execute(context);
					else
						CompositionManager.Instance.RunGraph(caller, context);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
