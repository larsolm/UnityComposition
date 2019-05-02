﻿using PiRhoSoft.UtilityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class ObjectVariableHandler : VariableHandler
	{
		public const string NullText = "(null)";

		private const string _gameObjectName = "GameObject";

		protected internal override VariableConstraint CreateConstraint() => new ObjectVariableConstraint();

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
			{
				var value = ListVariableHandler.LookupInList(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

			if (owner.HasStore)
			{
				var value = StoreVariableHandler.LookupInStore(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

			return VariableValue.Empty;
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (owner.HasList)
			{
				var result = ListVariableHandler.ApplyToList(ref owner, lookup, value);
				if (result == SetVariableResult.Success)
					return result;
			}

			if (owner.HasStore)
				return StoreVariableHandler.ApplyToStore(ref owner, lookup, value);

			return SetVariableResult.ReadOnly;
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
