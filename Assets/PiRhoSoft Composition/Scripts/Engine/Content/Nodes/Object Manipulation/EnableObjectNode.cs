using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Enable Behaviour", 20)]
	[HelpURL(Composition.DocumentationUrl + "enable-behaviour-node")]
	public class EnableObjectNode : InstructionGraphNode
	{
		private const string _invalidObjectWarning = "(CEONIO) unable to enable object for node '{0)': the object '{1}' is not a GameObject, Behaviour, or Renderer";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target GameObject, Behaviour, or Renderer to enable")]
		[VariableConstraint(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Object target))
			{
				if (target is GameObject gameObject)
					gameObject.SetActive(true);
				else if (target is Behaviour behaviour)
					behaviour.enabled = true;
				else if (target is Renderer renderer)
					renderer.enabled = true;
				else
					Debug.LogWarningFormat(this, _invalidObjectWarning, Name, Target);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
