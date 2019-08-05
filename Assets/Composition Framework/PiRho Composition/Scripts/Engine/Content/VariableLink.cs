using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "variable-link")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Link")]
	public sealed class VariableLink : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public CustomVariableCollection Variables = new CustomVariableCollection();

		void OnEnable()
		{
			for (var i = 0; i < Variables.VariableNames.Count && i < Variables.Variables.Count; i++)
				CompositionManager.Instance.GlobalStore.SetVariable(Variables.VariableNames[i], Variables.Variables[i]);
		}

		void OnDisable()
		{
			if (CompositionManager.Exists)
			{
				foreach (var name in Variables.VariableNames)
					CompositionManager.Instance.GlobalStore.SetVariable(name, Variable.Empty);
			}
		}
	}
}
