using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "text-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Text Binding")]
	public class TextBinding : StringBinding
	{
		[Tooltip("The variable holding the value to display as text in this object")]
		public VariableReference Variable = new VariableReference();

		private Variable _previousValue = PiRhoSoft.Composition.Variable.Empty;

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			if (variables.Resolve(this, Variable, out Variable value))
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
				_previousValue = PiRhoSoft.Composition.Variable.Empty;
				SetText(string.Empty, false);
			}
		}
	}
}
