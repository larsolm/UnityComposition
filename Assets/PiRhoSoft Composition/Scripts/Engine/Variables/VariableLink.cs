using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "variable-link")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Link")]
	public class VariableLink : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public VariablePool Variables = new VariablePool();

		void OnEnable()
		{
			for (var i = 0; i < Variables.Names.Count && i < Variables.Variables.Count; i++)
				CompositionManager.Instance.GlobalStore.SetVariable(Variables.Names[i], Variables.Variables[i]);
		}

		void OnDisable()
		{
			if (CompositionManager.Exists)
			{
				foreach (var name in Variables.Names)
					CompositionManager.Instance.GlobalStore.RemoveVariable(name);
			}
		}
	}
}
