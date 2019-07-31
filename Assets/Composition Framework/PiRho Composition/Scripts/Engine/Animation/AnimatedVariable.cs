using PiRhoSoft.Utilities;
using System;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	public enum AnimatedVariableType
	{
		Duration,
		Speed
	}

	public enum AnimatedVariableResult
	{
		Success,
		InvalidInputs,
		UnassignedOutput,
		MissingOutput,
		ReadOnlyOutput,
		InvalidOutput
	}

	[Serializable]
	public class AnimatedVariable
	{
		[Tooltip("The variable to update as the animation plays")]
		public VariableReference Target = new VariableReference();

		[Tooltip("Whether to advance the animation using Duration or Speed")]
		public AnimatedVariableType Advance = AnimatedVariableType.Duration;

		[Tooltip("Whether to respect the global timeScale (unchecked) or not (checked)")]
		public bool UseUnscaledTime = false;

		[Tooltip("The duration of the animation (in seconds)")]
		[Conditional(nameof(Advance), (int)AnimatedVariableType.Duration)]
		public float Duration = 1.0f;

		[Tooltip("The speed of the animation (in units of corresponding type per second)")]
		[Conditional(nameof(Advance), (int)AnimatedVariableType.Speed)]
		public float Speed = 1.0f;

		[Tooltip("The curve of the animation")]
		public AnimationCurve Animation = new AnimationCurve();

		public AnimatedVariableResult Result { get; private set; }

		public IEnumerator Animate(IVariableCollection variables, Variable from, Variable to)
		{
			if (from.IsEmpty)
				from = Target.GetValue(variables);

			var location = 0.0f;
			var end = Advance == AnimatedVariableType.Speed
				? VariableHandler.Distance(from, to)
				: Duration;

			while (location < end)
			{
				var elapsed = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
				var increment = Advance == AnimatedVariableType.Duration ? elapsed : Speed * elapsed;
				var time = Animation.Evaluate(location / end);

				location += increment;

				Result = Assign(variables, from, to, time);

				if (Result == AnimatedVariableResult.Success)
					yield return null;
				else
					yield break;
			}

			Result = Assign(variables, from, to, 1.0f);
		}

		private AnimatedVariableResult Assign(IVariableCollection variables, Variable from, Variable to, float time)
		{
			var result = VariableHandler.Interpolate(from, to, time);

			if (result.IsEmpty)
			{
				return AnimatedVariableResult.InvalidInputs;
			}
			else if (Target.IsAssigned)
			{
				var set = Target.SetValue(variables, result);

				switch (set)
				{
					case SetVariableResult.Success: return AnimatedVariableResult.Success;
					case SetVariableResult.NotFound: return AnimatedVariableResult.MissingOutput;
					case SetVariableResult.ReadOnly: return AnimatedVariableResult.ReadOnlyOutput;
					case SetVariableResult.TypeMismatch: return AnimatedVariableResult.InvalidOutput;
				}
			}

			return AnimatedVariableResult.UnassignedOutput;
		}
	}
}