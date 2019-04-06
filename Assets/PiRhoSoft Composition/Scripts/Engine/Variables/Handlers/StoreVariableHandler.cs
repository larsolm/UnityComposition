﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create((IVariableStore)null);
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Store);
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			var store = value.Store as VariableStore;

			if (store != null)
			{
				writer.Write(store.Variables.Count);

				for (var i = 0; i < store.Variables.Count; i++)
				{
					writer.Write(store.Variables[i].Name);
					Write(store.Variables[i].Value, writer, objects);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects)
		{
			var count = reader.ReadInt32();
			var store = new VariableStore();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var item = Read(reader, objects);

				store.AddVariable(name, item);
			}

			return VariableValue.Create(store);
		}

		protected override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			return LookupInStore(owner, lookup);
		}

		protected override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			return ApplyToStore(ref owner, lookup, value);
		}

		protected override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.HasReference)
				return left.Reference == right.Reference;
			else
				return null;
		}

		public static VariableValue LookupInStore(VariableValue owner, VariableValue lookup)
		{
			if (owner.HasList)
			{
				var value = ListVariableHandler.LookupInList(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

			if (lookup.Type == VariableType.String)
				return owner.Store.GetVariable(lookup.String);

			return VariableValue.Empty;
		}

		public static SetVariableResult ApplyToStore(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (owner.HasList)
			{
				var result = ListVariableHandler.ApplyToList(ref owner, lookup, value);
				if (result == SetVariableResult.Success)
					return result;
			}

			if (lookup.Type == VariableType.String)
				return owner.Store.SetVariable(lookup.String, value);
			else
				return SetVariableResult.TypeMismatch;
		}
	}
}
