using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "set-binding-node")]
	[CreateInstructionGraphNodeMenu("Interface/Set Binding", 300)]
	public class SetBindingNode : InstructionGraphNode
	{
		[Tooltip("The node to go to once the binding is set")]
		public InstructionGraphNode Next = null;

		[Tooltip("The Binding Root to set bindings on")]
		[VariableConstraint(typeof(BindingRoot))]
		public VariableReference Object = new VariableReference();

		[Tooltip("The Variable store to set the bindings to")]
		[VariableConstraint(typeof(IVariableStore))]
		public VariableReference Binding = new VariableReference();

		public override Color NodeColor => Colors.Interface;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Object, out BindingRoot root))
			{
				if (Resolve(variables, Binding, out IVariableStore binding))
				{
					root.Value = VariableValue.Create(binding);
					VariableBinding.UpdateBinding(root.gameObject, null, null);
				}
			}

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
