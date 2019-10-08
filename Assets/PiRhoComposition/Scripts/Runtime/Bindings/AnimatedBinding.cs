using PiRhoSoft.Utilities;
using System.Collections;

namespace PiRhoSoft.Composition
{
	public class AnimatedBinding : VariableBinding
	{
		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			StopAllCoroutines();
			StartCoroutine(Animate(variables, status));
		}

		private IEnumerator Animate(IVariableCollection variables, BindingAnimationStatus status)
		{
			status.Increment();

			if (Animation.Start(this, variables))
			{
				while (!Animation.IsComplete)
				{
					Animation.Step(variables);
					SetBinding(Animation.Value, true);
					yield return null;
				}
			}

			status.Decrement();
		}
	}
}
