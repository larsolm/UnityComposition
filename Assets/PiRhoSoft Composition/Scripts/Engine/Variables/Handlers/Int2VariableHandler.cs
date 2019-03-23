using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class Int2VariableResolver : VariableHandler
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (lookup == "x") return VariableValue.Create(owner.Int2.x);
			else if (lookup == "y") return VariableValue.Create(owner.Int2.y);
			else return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				if (lookup == "x")
				{
					owner = VariableValue.Create(new Vector2Int(value.Int, owner.Int2.y));
					return SetVariableResult.Success;
				}
				else if (lookup == "y")
				{
					owner = VariableValue.Create(new Vector2Int(owner.Int2.x, value.Int));
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
