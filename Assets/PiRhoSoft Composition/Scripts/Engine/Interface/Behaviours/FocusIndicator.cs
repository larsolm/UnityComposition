using UnityEngine;
using UnityEngine.UI;

namespace PiRhoSoft.CompositionEngine
{
	[RequireComponent(typeof(Graphic))]
	[HelpURL(Composition.DocumentationUrl + "focus-indicator")]
	[AddComponentMenu("Composition/Interface/Focus Indicator")]
	public class FocusIndicator : MonoBehaviour
	{
		private Graphic _graphic;

		void Awake()
		{
			_graphic = GetComponent<Graphic>();
			_graphic.enabled = false;
		}

		public virtual void SetFocused(bool focused)
		{
			_graphic.enabled = focused;
		}
	}
}
