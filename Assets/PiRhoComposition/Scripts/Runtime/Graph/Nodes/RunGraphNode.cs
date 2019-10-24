using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Composition/Run Graph", 1)]
	[HelpURL(Configuration.DocumentationUrl + "graph-caller-node")]
	public class RunGraphNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The graph to run when this node is reached")]
		public GraphCallerVariableSource TargetGraph = new GraphCallerVariableSource();

		[Tooltip("The object to use as the root for Graph")]
		public VariableValueSource Context = new VariableValueSource { Type = VariableSourceType.Reference, Reference = new VariableLookupReference { Variable = "context" } };

		[Tooltip("Whether to wait for the graph to finish before moving to Next")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.ExecutionLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (!variables.Resolve(this, Context, out var context) && variables is GraphStore store)
				context = store.Context;

			yield return TargetGraph.Execute(this, variables, context, WaitForCompletion);

			graph.GoTo(Next, nameof(Next));
		}

		#region Inputs and Outputs

		// These need to be overriden because the inputs need to chained manually from the TargetGraph if it is set to a value

		public override void GetInputs(VariableDefinitionList inputs, string storeName)
		{
			TargetGraph.GetInputs(inputs, storeName);

			if (Context.UsesStore(storeName))
				inputs.Add(Context.GetDefinition());
		}

		public override void GetOutputs(VariableDefinitionList outputs, string storeName)
		{
			TargetGraph.GetOutputs(outputs, storeName);
		}

		#endregion
	}

	#region GraphCallerVariableSource

	[Serializable]
	public class GraphCallerVariableSource : VariableSource
	{
		[Tooltip("The graph to run when this node is reached")]
		[Conditional(nameof(Type), (int)VariableSourceType.Value)]
		[NoLabel]
		public GraphCaller Value = new GraphCaller();

		public override string ToString() => Type == VariableSourceType.Value ? Value.ToString() : base.ToString();
		public override VariableDefinition GetDefinition() => new VariableDefinition(Reference.RootName, VariableType.Object);

		public void GetInputs(VariableDefinitionList inputs, string storeName)
		{
			if (Type == VariableSourceType.Value)
			{
				foreach (var input in Value.Inputs)
				{
					if (input.UsesStore(storeName))
						inputs.Add(input.Reference.GetDefinition());
				}
			}
			else if (Type == VariableSourceType.Reference)
			{
				if (Reference.UsesStore(storeName))
					inputs.Add(Reference.GetDefinition());
			}
		}

		public void GetOutputs(VariableDefinitionList outputs, string storeName)
		{
			if (Type == VariableSourceType.Value)
			{
				foreach (var output in Value.Outputs)
				{
					if (output.UsesStore(storeName))
						outputs.Add(output.Reference.GetDefinition());
				}
			}
		}

		public IEnumerable Execute(Object owner, IVariableMap variables, Variable context, bool waitForCompletion)
		{
			if (Type == VariableSourceType.Value)
			{
				if (waitForCompletion)
					yield return Value.Execute(variables, context);
				else
					CompositionManager.Instance.RunGraph(Value, variables, context);
			}
			else if (Type == VariableSourceType.Reference)
			{
				if (variables.ResolveObject(owner, Reference, out Graph caller))
				{
					if (waitForCompletion)
						yield return caller.Execute(context);
					else
						CompositionManager.Instance.RunGraph(caller, context);
				}
			}

		}
	}

	#endregion
}
