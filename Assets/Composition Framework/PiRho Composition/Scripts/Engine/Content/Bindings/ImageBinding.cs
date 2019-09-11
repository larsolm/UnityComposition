﻿using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.Composition
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	[HelpURL(Configuration.DocumentationUrl + "image-binding")]
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

		protected override void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status)
		{
			Image.enabled = variables.ResolveObject(this, Variable, out Sprite sprite);
			Image.sprite = sprite;
		}
	}
}
