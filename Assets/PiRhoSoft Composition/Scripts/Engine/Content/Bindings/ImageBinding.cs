using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "image-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Image Binding")]
	public class ImageBinding : VariableBinding
	{
		[Tooltip("The variable holding the image to show on this object")]
		public VariableReference Variable = new VariableReference();

		private Image _image;

		public Image Image
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (!_image)
					_image = GetComponent<Image>();

				return _image;
			}
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			if (value.TryGetReference(out Sprite sprite))
			{
				Image.enabled = true;
				Image.sprite = sprite;
			}
			else
			{
				Image.enabled = false;
			}
		}
	}
}
