using UnityEngine;

namespace PiRhoSoft.Composition
{
	[HelpURL(Configuration.DocumentationUrl + "global-variables")]
	[AddComponentMenu("PiRho Composition/Global Variables")]
	public class GlobalVariablesComponent : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public VariableCollection Variables = new VariableCollection();

		void OnEnable()
		{
			for (var i = 0; i < Variables.VariableNames.Count && i < Variables.VariableCount; i++)
				CompositionManager.Instance.GlobalStore.SetVariable(Variables.VariableNames[i], Variables.GetVariable(i));
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
