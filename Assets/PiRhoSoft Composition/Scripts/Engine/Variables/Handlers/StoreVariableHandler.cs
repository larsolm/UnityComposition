using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableHandler : VariableHandler
	{
		protected override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			IVariableStore store;

			if (constraint is StoreVariableConstraint storeConstraint && storeConstraint.Schema != null)
				store = new ConstrainedStore(storeConstraint.Schema);
			else
				store = new VariableStore();

			return VariableValue.Create(store);
		}

		protected override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.Store);
		}

		protected override VariableConstraint CreateConstraint()
		{
			return new StoreVariableConstraint();
		}

		protected override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			var store = value.Store as VariableStore;

			if (store != null)
			{
				if (store is ISchemaOwner schemaOwner)
				{
					writer.Write(objects.Count);
					objects.Add(schemaOwner.Schema);
				}
				else
				{
					writer.Write(-1);
				}

				writer.Write(store.Variables.Count);

				for (var i = 0; i < store.Names.Count && i < store.Variables.Count; i++)
				{
					writer.Write(store.Names[i]);
					WriteValue(store.Variables[i], writer, objects);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var index = reader.ReadInt32();
			var schema = index >= 0 && index < objects.Count ? objects[index] as VariableSchema : null;
			var store = schema != null ? new ConstrainedStore(schema) : new VariableStore();

			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var item = ReadValue(reader, objects, version);

				if (schema != null)
					store.SetVariable(name, item);
				else
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
