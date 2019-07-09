using PiRhoSoft.Utilities.Engine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition.Engine
{
	internal class ObjectVariableHandler : VariableHandler
	{
		public const string NullText = "(null)";

		private const string _gameObjectName = "GameObject";

		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create((Object)null);
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			if (value.Object == null)
				builder.Append(NullText);
			else
				builder.Append(value.Object.name);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			writer.Write(objects.Count);
			objects.Add(value.Object);
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var index = reader.ReadInt32();
			return VariableValue.Create(objects[index]);
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			if (owner.HasList)
				return ListVariableHandler.LookupInList(owner, lookup);

			if (owner.HasStore)
				return StoreVariableHandler.LookupInStore(owner, lookup);

			if (lookup.HasString && owner.ReferenceType != null && ClassMap.Get(owner.ReferenceType, out var map))
				return map.GetVariable(owner.Reference, lookup.String);

			// could fall back to reflection here and in Apply_

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (owner.HasList)
				return ListVariableHandler.ApplyToList(ref owner, lookup, value);

			if (owner.HasStore)
				return StoreVariableHandler.ApplyToStore(ref owner, lookup, value);

			if (lookup.HasString && owner.ReferenceType != null && ClassMap.Get(owner.ReferenceType, out var map))
				return map.SetVariable(owner.Reference, lookup.String, value);

			return SetVariableResult.NotFound;
		}

		protected internal override VariableValue Cast_(VariableValue owner, string type)
		{
			if (type == _gameObjectName)
			{
				var gameObject = ComponentHelper.GetAsGameObject(owner.Object);
				return VariableValue.Create(gameObject);
			}
			else
			{
				var component = ComponentHelper.GetAsComponent(owner.Object, type);
				return VariableValue.Create(component);
			}
		}

		protected internal override bool Test_(VariableValue owner, string type)
		{
			if (type == _gameObjectName)
			{
				var gameObject = ComponentHelper.GetAsGameObject(owner.Object);
				return gameObject != null;
			}
			else
			{
				var component = ComponentHelper.GetAsComponent(owner.Object, type);
				return component != null;
			}
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.IsEmpty)
				return left.IsNull;
			else if (right.HasReference)
				return left.IsNull && right.IsNull || left.Reference == right.Reference;
			else
				return null;
		}
	}
}
