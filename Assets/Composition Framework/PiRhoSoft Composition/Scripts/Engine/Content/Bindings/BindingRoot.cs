﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Soft/Bindings/Binding Root")]
	[DisallowMultipleComponent]
	public class BindingRoot : MonoBehaviour, IVariableStore
	{
		private readonly string[] _names = new string[] { string.Empty };

		public string ValueName = "Value";

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

		internal static IVariableStore FindParent(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? _roots[0] : CompositionManager.Instance.DefaultStore;
		}

		#endregion

		#region IVariableStore Implementation

		public virtual IList<string> GetVariableNames() { _names[0] = ValueName; return _names; }
		public virtual VariableValue GetVariable(string name) => name == ValueName ? Value : _parent.GetVariable(name);
		public virtual SetVariableResult SetVariable(string name, VariableValue value) => name == ValueName ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion
	}
}
