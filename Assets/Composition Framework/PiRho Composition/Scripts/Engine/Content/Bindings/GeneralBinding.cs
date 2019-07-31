namespace PiRhoSoft.Composition
{
	public class GeneralBinding : VariableBinding
	{
		public VariableReference Target = new VariableReference();
		public VariableReference Binding = new VariableReference();

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			var value = Binding.GetValue(variables);
			Target.SetValue(variables, value); // TODO: make 'this' accessible on variables
		}
	}
}