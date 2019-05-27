using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "focus-binding-root")]
	[AddComponentMenu("PiRho Soft/Bindings/Focus Binding Root")]
	public class FocusBindingRoot : BindingRoot
	{
		[Tooltip("The Menu whose focused item to use for binding variables")]
		public Menu Menu;

		public override VariableValue Value
		{
			get => Menu ? VariableValue.Create((Object)Menu.FocusedItem) : VariableValue.Empty;
		}
	}
}
