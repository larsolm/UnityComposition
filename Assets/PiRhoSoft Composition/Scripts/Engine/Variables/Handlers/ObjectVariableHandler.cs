using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ObjectVariableHandler : VariableHandler
	{
		private const string _gameObjectName = "GameObject";

		protected override VariableConstraint CreateConstraint() => new ObjectVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create((Object)null);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(objects.Count);
			objects.Add(value.Object);
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var index = reader.ReadInt32();
			value = VariableValue.Create(objects[index]);
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (owner.HasList)
			{
				var value = ListVariableHandler.ListLookup(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

			if (owner.HasStore)
			{
				var value = StoreVariableHandler.StoreLookup(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

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

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (owner.HasList)
			{
				var result = ListVariableHandler.ListApply(ref owner, lookup, value);
				if (result == SetVariableResult.Success)
					return result;
			}

			if (owner.HasStore)
			{
				var result = StoreVariableHandler.StoreApply(ref owner, lookup, value);
				if (result == SetVariableResult.Success)
					return result;
			}

			return SetVariableResult.ReadOnly;
		}
	}
}
