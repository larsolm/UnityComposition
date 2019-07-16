using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "number-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Number Binding")]
	public class NumberBinding : StringBinding
	{
		private const string _invalidVariableWarning = "(CNBIV) Failed to resolve variable '{0}' on binding '{1}': the variable has type '{2}' and should have type 'Int' or 'Float'";

		public BindingFormatter Format;

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
					if (value.TryGetInt(out int i))
					{
						var text = Format.GetFormattedString(i);
						SetText(text, true);
						_previousValue = value;
					}
					else if (value.TryGetFloat(out float f))
					{
						var text = Format.GetFormattedString(f);
						SetText(text, true);
						_previousValue = value;
					}
					else
					{
						if (!SuppressErrors)
							Debug.LogWarningFormat(this, _invalidVariableWarning, Variable, name, value.Type);

						SetText(string.Empty, false);
						_previousValue = VariableValue.Empty;
					}
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
