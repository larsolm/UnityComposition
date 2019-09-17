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

		[Tooltip("The value the animation starts at (empty to start at the current value)")]
		public VariableValueSource From = new VariableValueSource();

		[Tooltip("The value to animate to")]
		public VariableValueSource To = new VariableValueSource();

		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (variables.Resolve(this, From, out var from) && variables.Resolve(this, To, out var to))
			{
				yield return Animation.Animate(variables, from, to);

				switch (Animation.Result)
				{
					case AnimatedVariableResult.InvalidInputs: Debug.LogWarningFormat(this, _invalidInputsWarning, name, from.Type, to.Type); break;
					case AnimatedVariableResult.UnassignedOutput: Debug.LogWarningFormat(this, _unassignedOutputWarning, name, from.Type, to.Type); break;
					case AnimatedVariableResult.MissingOutput: Debug.LogWarningFormat(this, _missingOutputWarning, name, Animation.Target); break;
					case AnimatedVariableResult.ReadOnlyOutput: Debug.LogWarningFormat(this, _readOnlyOutputWarning, name, Animation.Target); break;
					case AnimatedVariableResult.InvalidOutput: Debug.LogWarningFormat(this, _invalidOutputWarning, name, Animation.Target); break;
				}
			}

			graph.GoTo(Next, nameof(Next));
		}
	}
}