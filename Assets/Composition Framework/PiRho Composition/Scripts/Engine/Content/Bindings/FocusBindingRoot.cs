using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "focus-binding-root")]
	[AddComponentMenu("PiRho Soft/Bindings/Focus Binding Root")]
	public class FocusBindingRoot : BindingRoot
	{
		[Tooltip("The Menu whose focused item to use for binding variables")]
		public Menu Menu;

		public override Variable Value
		{
			get => Menu ? Variable.Object((Object)Menu.FocusedItem) : Variable.Empty;
		}
	}
}
