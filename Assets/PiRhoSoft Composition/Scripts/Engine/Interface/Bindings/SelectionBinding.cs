using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "selection-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Selection Binding")]
	public class SelectionBinding : MonoBehaviour
	{
		[Tooltip("The Selection Control whose focused item to use for binding variables")]
		public SelectionControl SelectionControl;

		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup;

		void Update()
		{
			if (SelectionControl)
			{
				var variables = SelectionControl.FocusedVariables;
				if (variables != null)
					InterfaceBinding.UpdateBindings(gameObject, variables, BindingGroup, null);
			}
		}
	}
}
