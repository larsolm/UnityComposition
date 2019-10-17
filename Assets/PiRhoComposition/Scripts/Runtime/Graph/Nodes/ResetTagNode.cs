using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Composition/Reset Tag", 31)]
	[HelpURL(Configuration.DocumentationUrl + "reset-tag-node")]
	public class ResetTagNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The object containing the variables to reset")]
		public VariableLookupReference Object = new VariableLookupReference();

		[Tooltip("The tag to reset")]
		public StringVariableSource Tag = new StringVariableSource();

		public override Color NodeColor => Colors.ExecutionDark;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			//if (variables.ResolveInterface(this, Object, out IResettableVariables reset) && variables.Resolve(this, Tag, out var tag))
			//	reset.ResetTag(tag);

			graph.GoTo(Next, nameof(Next));
			yield break;
		}
	}
}
