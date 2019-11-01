using PiRhoSoft.Utilities;
using System.Collections;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "animated-binding")]
	[AddComponentMenu("PiRho Composition/Bindings/Animated Binding")]
	public class AnimatedBinding : VariableBinding
	{
		[Inline]
		public AnimatedVariable Animation = new AnimatedVariable();

		protected override void UpdateBinding(IVariableMap variables, BindingAnimationStatus status)
		{
			StopAllCoroutines();
			StartCoroutine(Animate(variables, status));
		}

		private IEnumerator Animate(IVariableMap variables, BindingAnimationStatus status)
		{
			status.Increment();

			if (Animation.Start(this, variables))
			{
				while (!Animation.IsComplete)
				{
					Animation.Step();
					SetBinding(Animation.Value, true);
					yield return null;
				}
			}

			status.Decrement();
		}
	}
}
