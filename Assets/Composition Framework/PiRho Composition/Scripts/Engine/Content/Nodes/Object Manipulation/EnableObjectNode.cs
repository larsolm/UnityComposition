using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Object Manipulation/Enable Object", 20)]
	[HelpURL(Configuration.DocumentationUrl + "enable-object-node")]
	public class EnableObjectNode : GraphNode
	{
		private const string _invalidObjectWarning = "(CEONIO) Unable to enable object for node '{0)': the object '{1}' is not a GameObject, Behaviour, or Renderer";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The target GameObject, Behaviour, or Renderer to enable")]
		[VariableReference(typeof(Object))]
		public VariableReference Target = new VariableReference();

		public override Color NodeColor => Colors.SequencingLight;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
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
					Debug.LogWarningFormat(this, _invalidObjectWarning, name, Target);
			}

			graph.GoTo(Next, nameof(Next));

			yield break;
		}
	}
}
