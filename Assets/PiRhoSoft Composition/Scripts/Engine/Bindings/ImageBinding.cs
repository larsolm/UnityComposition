using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "image-binding")]
	[AddComponentMenu("PiRho Soft/Interface/Image Binding")]
	public class ImageBinding : VariableBinding
	{
		[Tooltip("The variable holding the image to show on this object")]
		public VariableReference Variable = new VariableReference();

		private Image _image;

		void Awake()
		{
			_image = GetComponent<Image>();
		}

		protected override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			if (value.TryGetReference(out Sprite sprite))
			{
				_image.enabled = true;
				_image.sprite = sprite;
			}
			else
			{
				_image.enabled = false;
			}
		}
	}
}
