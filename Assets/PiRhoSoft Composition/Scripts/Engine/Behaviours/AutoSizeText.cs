using TMPro;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(TextMeshProUGUI))]
	[ExecuteInEditMode]
	[HelpURL(Composition.DocumentationUrl + "auto-size-text")]
	[AddComponentMenu("PiRho Soft/Interface/Auto Size Text")]
	public class AutoSizeText : MonoBehaviour
	{
		void OnEnable()
		{
			var text = GetComponent<TextMeshProUGUI>();
			text.autoSizeTextContainer = true;
		}
	}
}
