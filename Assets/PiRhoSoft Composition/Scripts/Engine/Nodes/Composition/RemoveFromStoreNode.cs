using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Composition/Remove From Store", 31)]
	[HelpURL(Composition.DocumentationUrl + "remove-from-store-node")]
	public class RemoveFromStoreNode : InstructionGraphNode
	{
		private const string _invalidValueWarning = "(CCRFSNIV) Unable to remove value from store for {0}";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The value to remove from to the store")]
		public VariableReference Value = new VariableReference();

		[Tooltip("The store to add Value to")]
		[VariableConstraint(typeof(IIndexedVariableStore))]
		public VariableReference Store = new VariableReference();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveOther<object>(variables, Value, out var value) && ResolveOther<IIndexedVariableStore>(variables, Store, out var store))
			{
				for (var i = 0; i < store.Count; i++)
				{
					var item = store.GetItem(i);
					if (item == value)
					{
						store.RemoveItem(i);
						graph.GoTo(Next, nameof(Next));

						yield break;
					}
				}

				Debug.LogWarningFormat(this, _invalidValueWarning, Name);
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
