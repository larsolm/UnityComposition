using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition.Extensions
{
	[CreateGraphNodeMenu("Animation/Animate Variable", 0)]
	public class AnimateNode : GraphNode
	{
		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The variable to assign the animated value to")]
		public VariableAssignmentReference Target = new VariableAssignmentReference();

		[Tooltip("The value to animate the variable from and to")]
		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		[Tooltip("Whether to wait for the completion of the animation before continuing to the next node")]
		public bool WaitForCompletion = true;

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableMap variables)
		{
			if (Animation.Start(this, variables))
			{
				if (WaitForCompletion)
					yield return RunAnimation(variables);
				else
					CompositionManager.Instance.StartCoroutine(RunAnimation(variables));
			}

			graph.GoTo(Next, nameof(Next));
		}

		private IEnumerator RunAnimation(IVariableMap variables)
		{
			while (!Animation.IsComplete)
			{
				Animation.Step();
				Target.SetValue(variables, Animation.Value);
				yield return null;
			}
		}
	}
}
