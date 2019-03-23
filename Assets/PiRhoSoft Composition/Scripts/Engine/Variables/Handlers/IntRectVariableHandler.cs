using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntRectVariableResolver : VariableHandler
	{
		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			switch (lookup)
			{
				case "x": return VariableValue.Create(owner.IntRect.position.x);
				case "y": return VariableValue.Create(owner.IntRect.position.y);
				case "w": return VariableValue.Create(owner.IntRect.size.x);
				case "h": return VariableValue.Create(owner.IntRect.size.y);
				default: return VariableValue.Empty;
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (value.Type == VariableType.Int)
			{
				switch (lookup)
				{
					case "x":
					{
						owner = VariableValue.Create(new RectInt(value.Int, owner.IntRect.position.y, owner.IntRect.size.x, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "y":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, value.Int, owner.IntRect.size.x, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "w":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, owner.IntRect.position.y, value.Int, owner.IntRect.size.y));
						return SetVariableResult.Success;
					}
					case "h":
					{
						owner = VariableValue.Create(new RectInt(owner.IntRect.position.x, owner.IntRect.position.y, owner.IntRect.size.x, value.Int));
						return SetVariableResult.Success;
					}
					default:
					{
						return SetVariableResult.NotFound;
					}
				}
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}
	}
}
