using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "reference-binding")]
	[AddComponentMenu("PiRho Composition/Bindings/Reference Binding")]
	public class ReferenceBinding : VariableBinding
	{
		[Tooltip("The variable to get the bound the value from")]
		public VariableLookupReference Binding = new VariableLookupReference();

		protected override void UpdateBinding(IVariableMap variables, BindingAnimationStatus status)
		{
			var value = Binding.GetValue(variables);
			SetBinding(value, !value.IsEmpty);
		}
	}
}