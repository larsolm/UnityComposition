using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Soft/Bindings/Binding Root")]
	[DisallowMultipleComponent]
	public class BindingRoot : MonoBehaviour, IVariableCollection
	{
		private readonly string[] _names = new string[] { string.Empty };

		public string ValueName = "Value";

		public virtual Variable Value { get; set; }

		private IVariableCollection _parent;

		protected virtual void Awake()
		{
			if (transform.parent)
				_parent = FindParent(transform.parent.gameObject);
			else
				_parent = CompositionManager.Instance.DefaultStore;
		}

		#region Hierarchy

		private static List<BindingRoot> _roots = new List<BindingRoot>();

		internal static IVariableCollection FindParent(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? _roots[0] : CompositionManager.Instance.DefaultStore;
		}

		#endregion

		#region IVariableStore Implementation

		public virtual IReadOnlyList<string> VariableNames { get { _names[0] = ValueName; return _names; } }
		public virtual Variable GetVariable(string name) => name == ValueName ? Value : _parent.GetVariable(name);
		public virtual SetVariableResult SetVariable(string name, Variable value) => name == ValueName ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion
	}
}
