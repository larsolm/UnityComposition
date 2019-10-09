using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Animation/Animate", 0)]
	public class AnimateNode : GraphNode
	{
		private const string _invalidInputsWarning = "(PCANIA) failed to animate in node '{0}': the types '{1}' and '{2}' cannot be animated";
		private const string _unassignedOutputWarning = "(PCANIA) failed to animate in node '{0}': the variable to animate has not been set";
		private const string _missingOutputWarning = "(PCANIA) failed to animate in node '{0}': the variable '{1}' cannot be found";
		private const string _readOnlyOutputWarning = "(PCANIA) failed to animate in node '{0}': the variable '{1}' is read only";
		private const string _invalidOutputWarning = "(PCANIA) failed to animate in node '{0}': the variable '{1}' could not be set";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The variable to assign the animated value to")]
		public VariableAssignmentReference Target = new VariableAssignmentReference();

		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (Animation.Start(this, variables))
			{
				while (!Animation.IsComplete)
				{
					Animation.Step(variables);
					Target.SetValue(variables, Animation.Value);
					yield return null;
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}
