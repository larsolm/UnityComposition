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
		public VariableReference Reference = new VariableReference();

		[Tooltip("The object to use as the root for Graph")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableReference { Variable = "context" } };

		[Tooltip("Whether to wait for the graph to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override void GetInputs(IList<VariableDefinition> inputs)
		{
			if (Source == GraphSource.Value)
			{
				foreach (var input in TargetGraph.Inputs)
				{
					if (GraphStore.IsInput(input))
						inputs.Add(TargetGraph.GetInputDefinition(input));
				}
			}
			else if (Source == GraphSource.Reference)
			{
				if (GraphStore.IsInput(Reference))
					inputs.Add(new VariableDefinition(Reference.RootName, new ObjectConstraint(typeof(Graph))));
			}
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (Source == GraphSource.Value)
			{
				foreach (var output in TargetGraph.Outputs)
				{
					if (GraphStore.IsOutput(output))
						outputs.Add(TargetGraph.GetOutputDefinition(output));
				}
			}
		}

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (!Resolve(variables, Context, out var context) && variables is GraphStore store)
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
				if (ResolveObject(variables, Reference, out Graph caller))
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
