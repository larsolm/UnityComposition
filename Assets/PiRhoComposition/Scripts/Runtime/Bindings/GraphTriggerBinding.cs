using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "graph-trigger-binding")]
	[AddComponentMenu("PiRho Composition/Bindings/Graph Trigger Binding")]
	public class GraphTriggerBinding : VariableBinding
	{
		[Tooltip("The graph to run when Variable changes")]
		public GraphCaller Graph = new GraphCaller();

		[Tooltip("The variable holding the value to watch for changes")]
		public VariableLookupReference Variable = new VariableLookupReference();

		private Variable _value = Composition.Variable.Empty;

		protected override void UpdateBinding(IVariableMap variables, BindingAnimationStatus status)
		{
			if (Graph.Graph && !Graph.IsRunning)
			{
				variables.Resolve(this, Variable, out Variable value);

				if (!VariableHandler.IsEqual(_value, value).GetValueOrDefault(false))
					StartCoroutine(VariableChanged(value, status));
			}
		}

		private IEnumerator VariableChanged(Variable value, BindingAnimationStatus status)
		{
			status.Increment();

			_value = value;

			CompositionManager.Instance.RunGraph(Graph, Variables, Variables.This);

			while (Graph.IsRunning)
				yield return null;

			status.Decrement();
		}
	}
}
