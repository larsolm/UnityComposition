using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Vector3VariableResolver : VariableResolver
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Vector3.x);
			else if (lookup == "y") return VariableValue.Create(owner.Vector3.y);
			else if (lookup == "z") return VariableValue.Create(owner.Vector3.z);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.TryGetFloat(out var number))
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector3(number, owner.Vector3.y, owner.Vector3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector3(owner.Vector3.x, number, owner.Vector3.z));
					return SetVariableResult.Success;
				}
				else if (lookup == "z")
				{
					owner = VariableValue.Create(new Vector3(owner.Vector3.x, owner.Vector3.y, number));
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
