using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "enable-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Enable Binding")]
	public class EnableBinding : VariableBinding
	{
		[Tooltip("The behaviour to enable or disable based on Expression")]
		public Behaviour Behaviour;

		[Tooltip("The expression to run to determine if the refereneced component should be enabled")]
		public Expression Condition = new Expression();

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Behaviour)
			{
				var active = false;

				try
				{
					Condition.Evaluate(variables).TryGetBool(out active);
				}
				catch
				{
				}

				Behaviour.enabled = active;
			}
		}
	}
}
