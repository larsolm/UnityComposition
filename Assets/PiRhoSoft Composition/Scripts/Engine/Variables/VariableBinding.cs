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

		private BindingRoot _root;

		protected abstract void UpdateBinding(IVariableStore variables, BindingAnimationStatus status);

		void Awake()
		{
			_root = BindingRoot.FindRoot(gameObject);
		}

		void Update()
		{
			if (AutoUpdate)
				UpdateBinding(string.Empty, null);
		}

		public void UpdateBinding(string group, BindingAnimationStatus status)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
			{
				var variables = (_root != null ? _root.Variables : null) ?? CompositionManager.Instance.GlobalStore;
				UpdateBinding(variables, status ?? _ignoredStatus);
			}
		}

		#region Static Helpers

		private static BindingAnimationStatus _ignoredStatus = new BindingAnimationStatus();
		private static List<VariableBinding> _bindings = new List<VariableBinding>();

		public static List<VariableBinding> GetBindings(GameObject obj, bool includeChildren)
		{
			_bindings.Clear();

			if (includeChildren)
				obj.GetComponentsInChildren(true, _bindings); // includes components directly on obj
			else
				obj.GetComponents(_bindings); // always includes inactive

			return _bindings;
		}

		public static void UpdateBinding(GameObject obj, string group, BindingAnimationStatus status)
		{
			var bindings = GetBindings(obj, true);

			foreach (var binding in _bindings)
				binding.UpdateBinding(group, status);
		}

		#endregion
	}
}
