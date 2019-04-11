using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	[Serializable]
	public class PoolVariableStore : VariableStore, ISerializationCallbackReceiver
	{
		[SerializeField] private List<string> _variablesData;
		[SerializeField] private List<Object> _variablesObjects;

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
