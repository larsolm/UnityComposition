using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(Composition.DocumentationUrl + "image-color-binding")]
	[AddComponentMenu("PiRho Soft/Bindings/Image Color Binding")]
	public class ImageColorBinding : VariableBinding
	{
		private const string _missingVariableWarning = "(CBICBMV) Unable to bind color for image color binding '{0}': the variable '{1}' could not be found";
		private const string _invalidVariableWarning = "(CBICBIV) Unable to bind color for image color binding '{0}': the variable '{1}' is not a color";

		[Tooltip("The variable holding the image to show on this object")]
		public VariableReference Variable = new VariableReference();

		private Image _image;

		public Image Sprite
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

			Sprite.enabled = value.Type == VariableType.Color;
			Sprite.color = value.Color;

			if (!SuppressErrors && value.Type != VariableType.Color)
				Debug.LogWarningFormat(this, value.IsEmpty ? _missingVariableWarning : _invalidVariableWarning, name, Variable);
		}
	}
}
