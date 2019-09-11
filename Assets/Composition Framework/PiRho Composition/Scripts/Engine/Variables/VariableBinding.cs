using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
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
		private const string _missingVariableWarning = "(PCVBMV) failed to resolve variable '{0}' on binding '{1}': the variable could not be found";
		private const string _invalidVariableWarning = "(PCVBIV) failed to resolve variable '{0}' on binding '{1}': the variable has type '{2}' and should have type '{3}'";
		private const string _invalidEnumWarning = "(PCVBIE) failed to resolve variable '{0}' on binding '{1}': the variable has enum type '{2}' and should have enum type '{3}'";
		private const string _invalidObjectWarning = "(PCVBIO) failed to resolve variable '{0}' on node '{1}': the object '{2}' is a '{3}' and cannot be converted to a '{4}'";
		private const string _invalidTypeWarning = "(PCVBIT) failed to resolve variable '{0}' on node '{1}': the value is a '{2}' and cannot be converted to a '{3}'";

		private const string _missingAssignmentWarning = "(PCVBMA) failed to assign to variable '{0}' from binding '{1}': the variable could not be found";
		private const string _readOnlyAssignmentWarning = "(PCVBROA) failed to assign to variable '{0}' from binding '{1}': the variable is read only";
		private const string _invalidAssignmentWarning = "(PCVBIA) failed to assign to variable '{0}' from binding '{1}': the variable has an incompatible type";

		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = string.Empty;

		[Tooltip("When set, the binding will update automatically when the variable changes")]
		public bool AutoUpdate = true;

		[Tooltip("When set, errors in resolving the binding will be treated as a valid condition that hides or disables corresponding components")]
		public bool SuppressErrors = false;

		private IVariableCollection _variables;

		public IVariableCollection Variables
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (_variables == null)
					_variables = BindingRoot.FindParent(gameObject);

				return _variables;
			}
		}

		protected virtual void Awake()
		{
			CompositionManager.Instance.AddBinding(this);
		}

		protected virtual void OnDestroy()
		{
			if (CompositionManager.Exists)
				CompositionManager.Instance.RemoveBinding(this);
		}

		public void UpdateBinding(string group, BindingAnimationStatus status)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
				UpdateBinding(Variables, status ?? _ignoredStatus);
		}

		protected abstract void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status);

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