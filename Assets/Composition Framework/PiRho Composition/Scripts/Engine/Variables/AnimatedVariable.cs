using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
		InvalidInputs
	}

	[Serializable]
	public class AnimatedVariable
	{
		private const string _invalidInputsWarning = "(PCAVII) failed to animate from '{0}' to '{1}': the types '{0}' and '{1}' cannot be animated";

		[Tooltip("The value the animation starts at")]
		public VariableLookupReference From = new VariableLookupReference(); // TODO; VariableValueSource?

		[Tooltip("The value to animate to")]
		public VariableLookupReference To = new VariableLookupReference();

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

		public bool IsComplete => _location >= _end;
		public Variable Value { get; private set; }

		private Variable _from;
		private Variable _to;
		private float _location;
		private float _end;

		public bool Start(Object context, IVariableCollection variables)
		{
			_from = From.GetValue(variables);
			_to = To.GetValue(variables);

			_location = 0.0f;
			_end = Advance == AnimatedVariableType.Speed
				? VariableHandler.Distance(_from, _to)
				: Duration;

			Set(variables, 0.0f);

			if (Value.IsEmpty)
				Debug.LogWarningFormat(context, _invalidInputsWarning, _from, _to, _from.Type, _to.Type);

			return !Value.IsEmpty;
		}

		public void Step(IVariableCollection variables)
		{
			var elapsed = UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
			var increment = Advance == AnimatedVariableType.Duration ? elapsed : Speed * elapsed;

			_location += increment;

			var time = Animation.Evaluate(_location / _end);

			if (time >= 1.0f)
				Complete(variables);
			else
				Set(variables, time);
		}

		public void Set(IVariableCollection variables, float time)
		{
			Value = VariableHandler.Interpolate(_from, _to, time);
		}

		public void Complete(IVariableCollection variables)
		{
			_location = _end;
			Set(variables, 1.0f);
		}
	}
}
