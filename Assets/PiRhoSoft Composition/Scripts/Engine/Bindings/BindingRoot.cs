using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "binding-root")]
	[AddComponentMenu("PiRho Soft/Interface/Binding Root")]
	public class BindingRoot : MonoBehaviour
	{
		private IVariableStore _variables;
		private BindingRoot _root;

		public virtual IVariableStore Variables
		{
			get => _variables ?? (_root != null ? _root.Variables : null);
			set => _variables = value;
		}

		void Awake()
		{
			_root = FindRoot(gameObject);
		}

		#region Lookup

		private static List<BindingRoot> _roots = new List<BindingRoot>();

		public static BindingRoot FindRoot(GameObject obj)
		{
			_roots.Clear();
			obj.GetComponentsInParent(true, _roots);
			return _roots.Count > 0 ? _roots[0] : null;
		}

		#endregion
	}
}
