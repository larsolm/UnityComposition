using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "graph-trigger-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Graph Trigger Binding")]
	public class GraphTriggerBinding : VariableBinding
	{
		[Tooltip("The graph to run when Variable changes")]
		public InstructionCaller Graph = new InstructionCaller();

		[Tooltip("The variable holding the value to watch for changes")]
		public VariableReference Variable = new VariableReference();

		private VariableValue _value = VariableValue.Empty;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Graph.Instruction && !Graph.IsRunning)
			{
				if (Resolve(variables, Variable, out VariableValue value))
				{
					if (!VariableHandler.IsEqual(_value, value).GetValueOrDefault(false))
						StartCoroutine(VariableChanged(value, status));
				}
			}
		}

		private IEnumerator VariableChanged(VariableValue value, BindingAnimationStatus status)
		{
			status.Increment();

			_value = value;

			CompositionManager.Instance.RunInstruction(Graph, Variables, VariableValue.Create(this));

			while (Graph.IsRunning)
				yield return null;

			status.Decrement();
		}
	}
}
