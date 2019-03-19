using PiRhoSoft.UtilityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableResolver : StoreVariableResolver
	{
		private const string _gameObjectName = "GameObject";

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (char.IsDigit(lookup[0]))
			{
				if (owner.TryGetReference(out IIndexedVariableStore store))
					return base.Lookup(owner, lookup);
				else
					return VariableValue.Empty;
			}
			else if (lookup == _gameObjectName)
			{
				var gameObject = ComponentHelper.GetAsGameObject(owner.Object);
				return VariableValue.Create(gameObject);
			}
			else
			{
				var component = ComponentHelper.GetAsComponent(owner.Object, lookup);
				return VariableValue.Create(component);
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (char.IsDigit(lookup[0]))
			{
				if (owner.TryGetReference(out IIndexedVariableStore store))
					return base.Apply(ref owner, lookup, value);
				else
					return SetVariableResult.NotFound;
			}
			else
			{
				return SetVariableResult.ReadOnly;
			}
		}
	}
}
