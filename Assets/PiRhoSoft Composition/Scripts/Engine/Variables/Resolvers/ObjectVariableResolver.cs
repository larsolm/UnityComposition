using PiRhoSoft.UtilityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableResolver : StoreVariableResolver
	{
		private const string _gameObjectName = "GameObject";

		private bool IsClassName(string lookup) => char.IsLetter(lookup[0]) || lookup[0] == '_';

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (IsClassName(lookup))
			{
				if (lookup == _gameObjectName)
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
			else
			{
				return base.Lookup(owner, lookup);
			}
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (IsClassName(lookup))
				return SetVariableResult.ReadOnly;
			else
				return base.Apply(ref owner, lookup, value);
		}
	}
}
