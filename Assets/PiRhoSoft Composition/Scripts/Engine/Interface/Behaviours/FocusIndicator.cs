using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "focus-indicator")]
	[AddComponentMenu("PiRho Soft/Interface/Focus Indicator")]
	public class FocusIndicator : MonoBehaviour
	{
		void Awake()
		{
			gameObject.SetActive(false);
		}

		public virtual void SetFocused(bool focused)
		{
			gameObject.SetActive(focused);
		}
	}
}
