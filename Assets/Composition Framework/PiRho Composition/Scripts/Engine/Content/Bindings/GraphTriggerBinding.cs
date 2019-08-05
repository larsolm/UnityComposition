using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "graph-trigger-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Graph Trigger Binding")]
	public class GraphTriggerBinding : VariableBinding
	{
		[Tooltip("The graph to run when Variable changes")]
		public GraphCaller Graph = new GraphCaller();

		[Tooltip("The variable holding the value to watch for changes")]
		public VariableReference Variable = new VariableReference();

		private Variable _value = PiRhoSoft.Composition.Variable.Empty;

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			if (Graph.Graph && !Graph.IsRunning)
			{
				Resolve(variables, Variable, out Variable value);

				if (!VariableHandler.IsEqual(_value, value).GetValueOrDefault(false))
					StartCoroutine(VariableChanged(value, status));
			}
		}

		private IEnumerator VariableChanged(Variable value, BindingAnimationStatus status)
		{
			status.Increment();

			_value = value;

			CompositionManager.Instance.RunGraph(Graph, Variables, PiRhoSoft.Composition.Variable.Object(gameObject));

			while (Graph.IsRunning)
				yield return null;

			status.Decrement();
		}
	}
}
