using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Int3VariableResolver : VariableResolver
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Int3.x);
			else if (lookup == "y") return VariableValue.Create(owner.Int3.y);
			else if (lookup == "z") return VariableValue.Create(owner.Int3.z);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector3Int(value.Int, owner.Int3.y, owner.Int3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector3Int(owner.Int3.x, value.Int, owner.Int3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Vector3Int(owner.Int3.x, owner.Int3.y, value.Int));
					return SetVariableResult.Success;
				}
				else
				{
					return SetVariableResult.NotFound;
				}
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}
	}
}
