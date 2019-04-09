using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "selection-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Selection Binding")]
	public class SelectionBinding : BindingRoot
	{
		[Tooltip("The Selection Control whose focused item to use for binding variables")]
		public SelectionControl SelectionControl;

		public override VariableValue Value
		{
			get
			{
				return SelectionControl ? SelectionControl.FocusedValue : VariableValue.Empty;
			}
		}
	}
}
