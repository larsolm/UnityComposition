using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Disable Object", 21)]
	[HelpURL(Configuration.DocumentationUrl + "disable-object-node")]
	public class DisableObjectNode : GraphNode
	{
		private const string _invalidObjectWarning = "(CDONIO) Unable to disable object for node '{0)': the object '{1}' is not a GameObject, Behaviour, or Renderer";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The target GameObject, Behaviour, or Renderer to disable")]
		[VariableReference(typeof(Object))]
		public VariableLookupReference Target = new VariableLookupReference();

		public override Color NodeColor => Colors.SequencingDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.ResolveObject(this, Target, out Object target))
			{
				if (target is GameObject gameObject)
					gameObject.SetActive(false);
				else if (target is Behaviour behaviour)
					behaviour.enabled = false;
				else if (target is Renderer renderer)
					renderer.enabled = false;
				else
					Debug.LogWarningFormat(this, _invalidObjectWarning, name, Target);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
