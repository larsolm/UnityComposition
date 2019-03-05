using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Composition.DocumentationUrl + "image-binding")]
	[AddComponentMenu("Composition/Interface/Image Binding")]
	public class ImageBinding : InterfaceBinding
	{
		public const string _invalidVariableError = "(IIBIV) Failed to update image binding: the variable '{0}' is not a Sprite and does not have a Sprite";
		public const string _missingVariableError = "(IIBMV) Failed to update image binding: the variable '{0}' could not be found";

		[Tooltip("The variable holding the image to show on this object")]
		public VariableReference Variable = new VariableReference();

		private Image _image;

		void Awake()
		{
			_image = GetComponent<Image>();
		}

		public override void UpdateBinding(IVariableStore variables, BindingAnimationStatus status)
		{
			var value = Variable.GetValue(variables);

			if (value.TryGetObject(out Sprite sprite))
				_image.sprite = sprite;
			else if (value.Type == VariableType.Empty)
				Debug.LogErrorFormat(this, _missingVariableError, Variable);
			else
				Debug.LogErrorFormat(this, _invalidVariableError, Variable);
		}
	}
}
