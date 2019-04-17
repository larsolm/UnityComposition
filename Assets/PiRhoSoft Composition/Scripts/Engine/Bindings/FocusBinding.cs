using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "focus-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Focus Binding")]
	public class FocusBinding : BindingRoot
	{
		[Tooltip("The Menu whose focused item to use for binding variables")]
		public Menu Menu;

		public override VariableValue Value
		{
			get => Menu ? VariableValue.Create((Object)Menu.FocusedItem) : VariableValue.Empty;
		}
	}
}
