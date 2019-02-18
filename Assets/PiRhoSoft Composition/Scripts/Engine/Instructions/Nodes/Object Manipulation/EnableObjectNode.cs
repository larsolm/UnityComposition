using System.Collections;
using System.Collections.Generic;
using PiRhoSoft.CompositionEngine;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Object", 20)]
	[HelpURL(Composition.DocumentationUrl + "enable-object-node")]
	public class EnableObjectNode : InstructionGraphNode
	{
		private const string _objectNotFoundWarning = "(COMEONF) Unable to enable object {0}: the object could not be found";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The object to enable")]
		public VariableReference Target = new VariableReference();

		public override bool IsExecutionImmediate => true;
		public override InstructionGraphExecutionMode ExecutionMode => InstructionGraphExecutionMode.Normal;

		public override void GetInputs(List<VariableDefinition> inputs)
		{
			if (InstructionStore.IsInput(Target))
				inputs.Add(VariableDefinition.Create<GameObject>(Target.RootName));
		}

		protected override IEnumerator Run_(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (Target.GetValue(variables).TryGetObject(out GameObject target))
				target.SetActive(true);
			else
				Debug.LogWarningFormat(this, _objectNotFoundWarning, Target);

			graph.GoTo(Next);

			yield break;
		}
	}
}
