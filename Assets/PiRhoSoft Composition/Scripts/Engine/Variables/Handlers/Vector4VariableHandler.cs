using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector4VariableResolver : VariableHandler
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Vector4.x);
			else if (lookup == "y") return VariableValue.Create(owner.Vector4.y);
			else if (lookup == "z") return VariableValue.Create(owner.Vector4.z);
			else if (lookup == "w") return VariableValue.Create(owner.Vector4.w);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector4(number, owner.Vector4.y, owner.Vector4.z, owner.Vector4.w));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector4(owner.Vector4.x, number, owner.Vector4.z, owner.Vector4.w));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Vector4(owner.Vector4.x, owner.Vector4.y, number, owner.Vector4.w));
					return SetVariableResult.Success;
				}
				else if (lookup == "w")
				{
					owner = VariableValue.Create(new Vector4(owner.Vector4.x, owner.Vector4.y, owner.Vector4.z, number));
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
