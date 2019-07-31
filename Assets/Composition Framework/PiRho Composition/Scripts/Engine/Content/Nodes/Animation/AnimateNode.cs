using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[CreateGraphNodeMenu("Animation/Animate", 0)]
	[HelpURL(Configuration.DocumentationUrl + "play-animation-node")]
	public class AnimateNode : GraphNode
	{
		public enum AdvanceType
		{
			Duration,
			Speed
		}

		private const string _invalidAnimateWarning = "(PCANIA) failed to animate in node '{0}': the types '{1}' and '{2}' cannot be animated";

		[Tooltip("The node to move to when this node is finished")]
		public GraphNode Next = null;

		[Tooltip("The value the animation starts at (empty to start at the current value)")]
		public VariableValueSource From = new VariableValueSource();

		[Tooltip("The value to animate to")]
		public VariableValueSource To = new VariableValueSource();

		[Tooltip("The variable to apply the animation to")]
		public VariableReference Target = new VariableReference();

		[Tooltip("Whether to advance the animation using Duration or Speed")]
		public AdvanceType Advance = AdvanceType.Duration;

		[Tooltip("Whether to respect the global timeScale (unchecked) or not (checked)")]
		public bool UseUnscaledTime = false;

		[Tooltip("The duration of the animation (in seconds)")]
		[Conditional(nameof(Advance), (int)AdvanceType.Duration)]
		public float Duration = 1.0f;

		[Tooltip("The speed of the animation (in units of corresponding type per second)")]
		[Conditional(nameof(Advance), (int)AdvanceType.Speed)]
		public float Speed = 1.0f;

		[Tooltip("The curve of the animation")]
		public AnimationCurve Animation = new AnimationCurve();

		public override Color NodeColor => Colors.Animation;

		public override IEnumerator Run(IGraphRunner graph, IVariableCollection variables)
		{
			if (Resolve(variables, From, out var from) || Resolve(variables, Target, out from))
			{
				if (Resolve(variables, To, out var to))
				{
					var location = 0.0f;
					var end = Advance == AdvanceType.Speed
						? VariableHandler.Distance(from, to)
						: Duration;

					while (location < end)
					{
						var elapsed = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
						var increment = Advance == AdvanceType.Duration ? elapsed : Speed * elapsed;
						var time = Animation.Evaluate(location / end);

						location += increment;

						if (Assign(variables, from, to, time))
							yield return null;
						else
							yield break;
					}

					Assign(variables, from, to, 1.0f);
				}
			}

			graph.GoTo(Next, nameof(Next));
		}

		private bool Assign(IVariableCollection variables, Variable from, Variable to, float time)
		{
			var result = VariableHandler.Interpolate(from, to, time);

			if (result.IsEmpty)
			{
				Debug.LogFormat(this, _invalidAnimateWarning, name, from.Type, to.Type);
				return false;
			}
			else
			{
				Assign(variables, Target, result);
				return true;
			}
		}
	}
}