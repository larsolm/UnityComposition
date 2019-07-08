﻿using PiRhoSoft.PargonUtilities.Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[CreateGraphNodeMenu("Composition/Graph Caller", 1)]
	[HelpURL(Composition.DocumentationUrl + "graph-caller-node")]
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
		public GraphCaller Graph = new GraphCaller();

		[Tooltip("The graph to run when this node is reached")]
		[VariableConstraint(typeof(Graph))]
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
				foreach (var input in Graph.Inputs)
				{
					if (GraphStore.IsInput(input))
						inputs.Add(Graph.GetInputDefinition(input));
				}
			}
			else if (Source == GraphSource.Reference)
			{
				if (GraphStore.IsInput(Reference))
					inputs.Add(new VariableDefinition { Name = Reference.RootName, Definition = ValueDefinition.Create<Graph>() });
			}
		}

		public override void GetOutputs(IList<VariableDefinition> outputs)
		{
			if (Source == GraphSource.Value)
			{
				foreach (var output in Graph.Outputs)
				{
					if (GraphStore.IsOutput(output))
						outputs.Add(Graph.GetOutputDefinition(output));
				}
			}
		}

		public override IEnumerator Run(Graph graph, GraphStore variables, int iteration)
		{
			if (!Resolve(variables, Context, out var context))
				context = variables.Context;

			if (Source == GraphSource.Value)
			{
				if (WaitForCompletion)
					yield return Graph.Execute(variables, context);
				else
					CompositionManager.Instance.RunGraph(Graph, variables, context);
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
