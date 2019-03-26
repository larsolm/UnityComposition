using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(Composition.DocumentationUrl + "sprite-color-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Sprite Color Binding")]
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

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			Sprite.enabled = value.Type == VariableType.Color;
			Sprite.color = value.Color;
		}
	}
}
