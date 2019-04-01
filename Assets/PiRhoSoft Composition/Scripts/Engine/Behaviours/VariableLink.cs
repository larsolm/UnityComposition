using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[HelpURL(Composition.DocumentationUrl + "global-variable-link")]
	[AddComponentMenu("PiRho Soft/Composition/Variable Link")]
	public class VariableLink : MonoBehaviour, ISerializationCallbackReceiver
	{
		[Tooltip("The variables to add to the global variable store")]
		public List<Variable> Variables = new List<Variable>();
		public List<ValueDefinition> Constraints = new List<ValueDefinition>();

		[SerializeField] private List<string> _variablesData;
		[SerializeField] private List<Object> _variablesObjects;

		void OnEnable()
		{
			foreach (var variable in Variables)
				CompositionManager.Instance.GlobalStore.SetVariable(variable.Name, variable.Value);
		}

		void OnDisable()
		{
			if (CompositionManager.Exists)
			{
				foreach (var variable in Variables)
					CompositionManager.Instance.GlobalStore.RemoveVariable(variable.Name);
			}
		}

		#region ISerializationCallbackReceiver Implementation

		public void OnBeforeSerialize()
		{
			Variable.Save(Variables, ref _variablesData, ref _variablesObjects);
		}

		public void OnAfterDeserialize()
		{
			Variable.Load(Variables, ref _variablesData, ref _variablesObjects);
		}

		#endregion
	}
}
