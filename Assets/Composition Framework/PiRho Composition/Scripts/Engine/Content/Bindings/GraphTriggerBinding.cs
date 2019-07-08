using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	[HelpURL(Composition.DocumentationUrl + "graph-trigger-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Graph Trigger Binding")]
	public class GraphTriggerBinding : VariableBinding
	{
		[Tooltip("The graph to run when Variable changes")]
		public GraphCaller Graph = new GraphCaller();

		[Tooltip("The variable holding the value to watch for changes")]
		public VariableReference Variable = new VariableReference();

		private VariableValue _value = VariableValue.Empty;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Graph.Graph && !Graph.IsRunning)
			{
				Resolve(variables, Variable, out VariableValue value);

				if (!VariableHandler.IsEqual(_value, value).GetValueOrDefault(false))
					StartCoroutine(VariableChanged(value, status));
			}
		}

		private IEnumerator VariableChanged(VariableValue value, BindingAnimationStatus status)
		{
			status.Increment();

			_value = value;

			CompositionManager.Instance.RunGraph(Graph, Variables, VariableValue.Create(gameObject));

			while (Graph.IsRunning)
				yield return null;

			status.Decrement();
		}
	}
}
