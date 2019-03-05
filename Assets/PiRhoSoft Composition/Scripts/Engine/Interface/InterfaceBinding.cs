using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class BindingAnimationStatus
	{
		private int _count = 0;

		public void Reset() => _count = 0;
		public bool IsFinished() => _count <= 0;
		public void Increment() =>  _count++;
		public void Decrement() => _count--;
	}

	public abstract class InterfaceBinding : MonoBehaviour
	{
		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = "";

		public abstract void UpdateBinding(IVariableStore variables, BindingAnimationStatus status);

		public static void UpdateBindings(GameObject obj, IVariableStore variables, string group, BindingAnimationStatus status)
		{
			var bindings = obj.GetComponentsInChildren<InterfaceBinding>(true); // this includes components directly on obj

			foreach (var binding in bindings)
				binding.UpdateBinding(variables, group, status);
		}

		public void UpdateBinding(IVariableStore store, string group, BindingAnimationStatus status)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
				UpdateBinding(store, status);
		}
	}
}
