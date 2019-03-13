using PiRhoSoft.UtilityEngine;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable] public class ObjectDictionary : SerializedDictionary<string, Object> { }

	public class VariableLink : MonoBehaviour
	{
		[Tooltip("The variables to add to the global variable store")]
		public VariableList Variables = new VariableList();

		void OnEnable()
		{
			for (var i = 0; i < Variables.VariableCount; i++)
			{
				var name = Variables.GetVariableName(i);
				var value = Variables.GetVariableValue(i);
				InstructionManager.Instance.GlobalStore.SetVariable(name, value);
			}
		}

		void OnDisable()
		{
			for (var i = 0; i < Variables.VariableCount; i++)
			{
				var name = Variables.GetVariableName(i);
				InstructionManager.Instance.GlobalStore.RemoveVariable(name);
			}
		}
	}
}
