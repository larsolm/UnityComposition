namespace PiRhoSoft.Composition
{
	public class ReferenceBinding : VariableBinding
	{
		public VariableLookupReference Binding = new VariableLookupReference();

		protected override void UpdateBinding(IVariableMap variables, BindingAnimationStatus status)
		{
			var value = Binding.GetValue(variables);
			SetBinding(value, !value.IsEmpty);
		}
	}
}