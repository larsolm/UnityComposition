﻿using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Composition/Bindings/Binding Root")]
	[DisallowMultipleComponent]
	public class BindingRoot : MonoBehaviour, IVariableMap
	{
		[Tooltip("The name used to access this binding root's Object")]
		public string ValueName = "Value";

		[Tooltip("The object that to be used for binding variables")]
		public SerializedVariable Value;

		private IVariableMap _parent;

		void Awake()
		{
			if (transform.parent)
				_parent = FindParent(transform.parent.gameObject);
			else
				_parent = CompositionManager.Instance.DefaultStore;
		}

		#region Hierarchy

		private static readonly List<BindingRoot> _roots = new List<BindingRoot>();

		internal static IVariableMap FindParent(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? _roots[0] : CompositionManager.Instance.DefaultStore;
		}

		#endregion

		#region IVariableStore Implementation

		private readonly string[] _names = new string[] { string.Empty };

		public IReadOnlyList<string> VariableNames { get { _names[0] = ValueName; return _names; } }
		public Variable GetVariable(string name) => name == ValueName ? Value.Variable : _parent.GetVariable(name);
		public SetVariableResult SetVariable(string name, Variable value) => name == ValueName ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion
	}
}
