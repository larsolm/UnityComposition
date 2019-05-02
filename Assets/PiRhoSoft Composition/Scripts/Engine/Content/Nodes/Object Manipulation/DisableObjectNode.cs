using System.Collections;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[CreateInstructionGraphNodeMenu("Object Manipulation/Disable Behaviour", 21)]
	[HelpURL(Composition.DocumentationUrl + "disable-behaviour-node")]
	public class DisableObjectNode : InstructionGraphNode
	{
		private const string _invalidObjectWarning = "(CDONIO) unable to disable object for node '{0)': the object '{1}' is not a GameObject, Behaviour, or Renderer";

		[Tooltip("The node to move to when this node is finished")]
		public InstructionGraphNode Next = null;

		[Tooltip("The target GameObject, Behaviour, or Renderer to disable")]
		[VariableConstraint(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(InstructionGraph graph, InstructionStore variables, int iteration)
		{
			if (ResolveObject(variables, Target, out Object target))
			{
				if (target is GameObject gameObject)
					gameObject.SetActive(false);
				else if (target is Behaviour behaviour)
					behaviour.enabled = false;
				else if (target is Renderer renderer)
					renderer.enabled = false;
				else
					Debug.LogWarningFormat(this, _invalidObjectWarning, Name, Target);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
