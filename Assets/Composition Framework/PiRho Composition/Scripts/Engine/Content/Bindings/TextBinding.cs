using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "text-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Binding")]
	public class TextBinding : StringBinding
	{
		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		private VariableValue _previousValue = VariableValue.Empty;

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			if (Resolve(variables, Variable, out VariableValue value))
			{
				var equal = VariableHandler.IsEqual(value, _previousValue);

				if (!equal.HasValue || !equal.Value)
				{
					SetText(value.ToString(), true);
					_previousValue = value;
				}
			}
			else
			{
				_previousValue = VariableValue.Empty;
				SetText(string.Empty, false);
			}
		}
	}
}
