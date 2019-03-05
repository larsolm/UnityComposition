using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[HelpURL(Composition.DocumentationUrl + "binding-control")]
	[AddComponentMenu("Composition/Interface/Binding Control")]
	public class BindingControl : InterfaceControl
	{
		private const string _invalidBindingError = "(CBCIB) Failed to update binding: the variable '{0}' is not an IVariableStore";
		private const string _missingBindingError = "(CBCMB) Failed to update binding: the variable '{0}' could not be found";

		[Tooltip("The variable to set as the root for any sibling and child bindings")]
		public VariableReference Binding = new VariableReference();

		public override void UpdateBindings(IVariableStore variables, string group, BindingAnimationStatus status)
		{
			Variables = variables;

			var binding = Binding.GetValue(variables);

			if (binding.TryGetStore(out IVariableStore store))
				InterfaceBinding.UpdateBindings(gameObject, store, group, status);
			else if (binding.Type == VariableType.Empty)
				Debug.LogWarningFormat(this, _missingBindingError, Binding);
			else
				Debug.LogWarningFormat(this, _invalidBindingError, Binding);
		}
	}
}
