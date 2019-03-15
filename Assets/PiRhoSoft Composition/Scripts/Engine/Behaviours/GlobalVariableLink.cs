using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "global-variable-link")]
	[AddComponentMenu("PiRho Soft/Composition/Global Variable Link")]
	public class GlobalVariableLink : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public VariableList Variables = new VariableList();

		void OnEnable()
		{
			for (var i = 0; i < Variables.VariableCount; i++)
			{
				var name = Variables.GetVariableName(i);
				var value = Variables.GetVariableValue(i);
				CompositionManager.Instance.GlobalStore.SetVariable(name, value);
			}
		}

		void OnDisable()
		{
			for (var i = 0; i < Variables.VariableCount; i++)
			{
				var name = Variables.GetVariableName(i);
				CompositionManager.Instance.GlobalStore.RemoveVariable(name);
			}
		}
	}
}
