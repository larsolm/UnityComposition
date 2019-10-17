using PiRhoSoft.Utilities;
using System;
using System.IO;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class AssetConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Asset;

		public override Variable Generate()
		{
			return Variable.Asset(null);
		}

		public override bool IsValid(Variable value)
		{
			return true;
		}

		public override void Save(SerializedDataWriter writer)
		{
		}

		public override void Load(SerializedDataReader reader)
		{
		}
	}
}
