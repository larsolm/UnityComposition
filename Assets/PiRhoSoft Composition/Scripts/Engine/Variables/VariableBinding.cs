using System.Collections.Generic;
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

	public abstract class VariableBinding : MonoBehaviour
	{
		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = string.Empty;

		[Tooltip("When set, the binding will update automatically when the variable changes")]
		public bool AutoUpdate = true;

		private IVariableStore _variables;

		public IVariableStore Variables
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (_variables == null)
					_variables = BindingRoot.FindParent(gameObject);

				return _variables;
			}
		}

		protected virtual void OnEnable()
		{
			CompositionManager.Instance.AddBinding(this);
		}

		protected virtual void OnDisable()
		{
			if (CompositionManager.Exists)
				CompositionManager.Instance.RemoveBinding(this);
		}

		public void UpdateBinding(string group, BindingAnimationStatus status)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
				UpdateBinding(Variables, status ?? _ignoredStatus);
		}

		protected abstract void UpdateBinding(IVariableStore variables, BindingAnimationStatus status);

		#region Static Helpers

		private static BindingAnimationStatus _ignoredStatus = new BindingAnimationStatus();
		private static List<VariableBinding> _bindings = new List<VariableBinding>();

		public static void UpdateBinding(GameObject obj, string group, BindingAnimationStatus status)
		{
			UpdateBinding(obj, group, status, _bindings);
		}

		public static void UpdateBinding(GameObject obj, string group, BindingAnimationStatus status, List<VariableBinding> bindings)
		{
			GetBindings(obj, true, bindings);

			foreach (var binding in bindings)
				binding.UpdateBinding(group, status);
		}

		private static void GetBindings(GameObject obj, bool includeChildren, List<VariableBinding> bindings)
		{
			bindings.Clear();

			if (includeChildren)
				obj.GetComponentsInChildren(true, bindings); // includes components directly on obj
			else
				obj.GetComponents(bindings); // always includes inactive
		}

		#endregion
	}
}
