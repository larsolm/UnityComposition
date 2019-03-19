using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Add To Store", 30)]
	[HelpURL(Composition.DocumentationUrl + "add-to-store-node")]
	public class AddToStoreNode : InstructionGraphNode
	{
		private const string _invalidValueWarning = "(CCATSNIV) Unable to add value to store for {0}";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The value to add to the store")]
		public VariableReference Value = new VariableReference();

		[Tooltip("The store to add Value to")]
		[VariableConstraint(typeof(IIndexedVariableStore))]
		public VariableReference Store = new VariableReference();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveOther<object>(variables, Value, out var value) && ResolveOther<IIndexedVariableStore>(variables, Store, out var store))
			{
				if (!store.AddItem(value))
					Debug.LogWarningFormat(this, _invalidValueWarning, Name);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
