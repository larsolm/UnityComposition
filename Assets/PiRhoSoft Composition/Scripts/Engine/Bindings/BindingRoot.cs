using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Soft/Interface/Binding Root")]
	public class BindingRoot : MonoBehaviour, IVariableStore
	{
		public string Name;
		public virtual VariableValue Value { get; set; }

		private IVariableStore _parent;

		protected virtual void Awake()
		{
			if (transform.parent)
				_parent = FindParent(transform.parent.gameObject);
			else
				_parent = CompositionManager.Instance.DefaultStore;
		}

		#region Hierarchy

		private static List<BindingRoot> _roots = new List<BindingRoot>();

		public static IVariableStore FindParent(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? _roots[0] : CompositionManager.Instance.DefaultStore;
		}

		#endregion

		#region IVariableStore Implementation

		public virtual IEnumerable<string> GetVariableNames() => Enumerable.Repeat(Name, 1);
		public virtual VariableValue GetVariable(string name) => name == Name ? Value : _parent.GetVariable(name);
		public virtual SetVariableResult SetVariable(string name, VariableValue value) => name == Name ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion
	}
}
