using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Composition.DocumentationUrl + "object-binding-root")]
	[AddComponentMenu("PiRho Soft/Bindings/Object Binding Root")]
	public class ObjectBindingRoot : BindingRoot
	{
		[Tooltip("The object that to be used a Binding Root to use for binding variables")]
		public Object Object;

		public override Variable Value
		{
			get
			{
				return Object ? Variable.Object(Object) : Variable.Empty;
			}
		}
	}
}
