using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "object-binding-root")]
	[AddComponentMenu("PiRho Composition/Bindings/Object Binding Root")]
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
