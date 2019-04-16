using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Soft/Interface/Binding Root")]
	public class BindingRoot : MonoBehaviour, IVariableStore
	{
		private readonly string[] _names = new string[1] { string.Empty };

		[ChangeTrigger(nameof(NameChanged))]
		public string Name;

		public virtual VariableValue Value { get; set; }

		private IVariableStore _parent;

		void Awake()
		{
			if (transform.parent)
				_parent = FindParent(transform.parent.gameObject);
			else
				_parent = CompositionManager.Instance.DefaultStore;

			_names[0] = Name;
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

		public virtual IList<string> GetVariableNames() => _names;
		public virtual VariableValue GetVariable(string name) => name == Name ? Value : _parent.GetVariable(name);
		public virtual SetVariableResult SetVariable(string name, VariableValue value) => name == Name ? SetVariableResult.ReadOnly : _parent.SetVariable(name, value);

		#endregion

		#region Editor Utility

		private void NameChanged()
		{
			_names[0] = Name;
		}

		#endregion
	}
}
