namespace PiRhoSoft.Composition
{
	public class GeneralBinding : VariableBinding
	{
		public VariableAssignmentReference Target = new VariableAssignmentReference();
		public VariableLookupReference Binding = new VariableLookupReference();

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			var value = Binding.GetValue(variables);
			Target.SetValue(variables, value); // TODO: make 'this' accessible on variables
		}
	}
}