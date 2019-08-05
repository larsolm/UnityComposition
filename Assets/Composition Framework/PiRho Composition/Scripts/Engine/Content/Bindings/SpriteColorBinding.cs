using UnityEngine;

namespace PiRhoSoft.Composition
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(Configuration.DocumentationUrl + "sprite-color-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Sprite Color Binding")]
	public class SpriteColorBinding : VariableBinding
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

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			Sprite.enabled = Resolve(variables, Variable, out Color color);
			Sprite.color = color;
		}
	}
}
