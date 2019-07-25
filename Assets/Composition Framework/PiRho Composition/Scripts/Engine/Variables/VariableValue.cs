using PiRhoSoft.Utilities;
using System;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class VariableValue : ISerializableData, ISerializationCallbackReceiver
	{
		public Variable Variable = Variable.Empty;
		[SerializeField] private SerializedData _variableData = new SerializedData();

		public void Save(BinaryWriter writer, SerializedData data)
		{
			VariableHandler.Save(Variable, writer, data);
		}

		public void Load(BinaryReader reader, SerializedData data)
		{
			Variable = VariableHandler.Load(reader, data);
		}

		public void OnBeforeSerialize()
		{
			_variableData.SaveInstance(this, 1);
		}

		public void OnAfterDeserialize()
		{
			_variableData.LoadInstance(this);
		}
	}
}