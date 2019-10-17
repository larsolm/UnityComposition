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

	public enum BindingErrorType
	{
		Log,
		Suppress,
		HideObject
	}

	public abstract class VariableBinding : MonoBehaviour
	{
		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = string.Empty;

		[Tooltip("When set, the binding will update automatically when the variable changes")]
		public bool AutoUpdate = true;

		[Tooltip("Specifies how errors in resolving the binding are treated")]
		public BindingErrorType ErrorType = BindingErrorType.Log;

		[Tooltip("The variable to assign the bound value to")]
		public VariableAssignmentReference Target = new VariableAssignmentReference();

		private IVariableMap _variables;

		public IVariableMap Variables
		{
			get
			{
				if (_variables == null)
					_variables = new ThisWrapper(gameObject);

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

		protected void SetBinding(Variable value, bool didSucceed)
		{
			if (didSucceed)
				Target.SetValue(Variables, value);
			
			if (ErrorType == BindingErrorType.HideObject)
				gameObject.SetActive(didSucceed);
		}

		protected abstract void UpdateBinding(IVariableMap variables, BindingAnimationStatus status);

		#region This Access

		private class ThisWrapper : IVariableMap
		{
			private readonly string[] _names = new string[] { string.Empty };

			public static string ThisName = "this";

			private readonly GameObject _this;
			private IVariableMap _parent;

			public ThisWrapper(GameObject obj)
			{
				_this = obj;
				_parent = BindingRoot.FindParent(obj);
			}

			#region IVariableStore Implementation

			public virtual IReadOnlyList<string> VariableNames { get { _names[0] = ThisName; return _names; } }
			public virtual Variable GetVariable(string name) => name == ThisName ? Variable.Object(_this) : _parent.GetVariable(name);
			public virtual SetVariableResult SetVariable(string name, Variable value) => name == ThisName ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

			#endregion
		}

		#endregion

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