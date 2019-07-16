using UnityEngine;

namespace PiRhoSoft.Composition
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(Composition.DocumentationUrl + "sprite-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Sprite Binding")]
	public class SpriteBinding : VariableBinding
	{
		[Tooltip("The variable holding the image to show on this object")]
		public VariableReference Variable = new VariableReference();

		private SpriteRenderer _sprite;

		public SpriteRenderer Sprite
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_sprite)
					_sprite = GetComponent<SpriteRenderer>();

				return _sprite;
			}
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			Sprite.enabled = ResolveObject(variables, Variable, out Sprite sprite);
			Sprite.sprite = sprite;
		}
	}
}
