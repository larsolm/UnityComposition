using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public abstract class InterfaceBinding : MonoBehaviour
	{
		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = "";

		public abstract void UpdateBinding(IVariableStore variables);

		public static void UpdateBindings(GameObject obj, IVariableStore variables, string group)
		{
			var bindings = obj.GetComponentsInChildren<InterfaceBinding>(true); // this includes components directly on obj

			foreach (var binding in bindings)
				binding.UpdateBinding(variables, group);
		}

		public void UpdateBinding(IVariableStore store, string group)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
				UpdateBinding(store);
		}
	}
}
