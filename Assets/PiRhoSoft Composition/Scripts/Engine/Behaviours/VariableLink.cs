using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "global-variable-link")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Link")]
	public class VariableLink : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public VariablePool Variables = new VariablePool();

		void OnEnable()
		{
			foreach (var variable in Variables.Variables)
				CompositionManager.Instance.GlobalStore.SetVariable(variable.Name, variable.Value);
		}

		void OnDisable()
		{
			if (CompositionManager.Exists)
			{
				foreach (var variable in Variables.Variables)
					CompositionManager.Instance.GlobalStore.RemoveVariable(variable.Name);
			}
		}
	}
}
