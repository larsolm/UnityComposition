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

		void Awake()
		{
			if (transform.parent)
				_parent = FindParent(transform.parent.gameObject);
			else
				_parent = _base;
		}

		#region Hierarchy

		private static BaseBindingRoot _base = new BaseBindingRoot();
		private static List<BindingRoot> _roots = new List<BindingRoot>();

		private class BaseBindingRoot : IVariableStore
		{
			public IEnumerable<string> GetVariableNames() => Enumerable.Repeat(CompositionManager.GlobalStoreName, 1);
			public VariableValue GetVariable(string name) => name == CompositionManager.GlobalStoreName ? VariableValue.Create(CompositionManager.Instance.DefaultStore) : VariableValue.Empty;
			public SetVariableResult SetVariable(string name, VariableValue value) => name == CompositionManager.GlobalStoreName ? SetVariableResult.ReadOnly : SetVariableResult.NotFound;
		}

		public static IVariableStore FindParent(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? (IVariableStore)_roots[0] : _base;
		}

		#endregion

		#region IVariableStore Implementation

		public virtual IEnumerable<string> GetVariableNames() => Enumerable.Repeat(Name, 1);
		public virtual VariableValue GetVariable(string name) => name == Name ? Value : _parent.GetVariable(name);
		public virtual SetVariableResult SetVariable(string name, VariableValue value) => name == Name ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion
	}
}
