using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableHandler : VariableHandler
	{
		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create((IVariableStore)null);
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			var store = value.Store as VariableStore;

			if (store != null)
			{
				writer.Write(store.Variables.Count);

				for (var i = 0; i < store.Variables.Count; i++)
				{
					writer.Write(store.Variables[i].Name);
					store.Variables[i].Value.Write(writer, objects);
				}
			}
			else
			{
				writer.Write(-1);
			}
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var count = reader.ReadInt32();

			if (count >= 0)
			{
				var store = new VariableStore();

				for (var i = 0; i < count; i++)
				{
					var name = reader.ReadString();
					var item = new VariableValue();

					item.Read(reader, objects);
					store.AddVariable(name, item);
				}

				value = VariableValue.Create(store);
			}
		}

		public static VariableValue StoreLookup(VariableValue owner, string lookup)
		{
			if (owner.HasList)
			{
				var value = ListVariableHandler.ListLookup(owner, lookup);
				if (!value.IsEmpty)
					return value;
			}

			return owner.Store.GetVariable(lookup);
		}

		public static SetVariableResult StoreApply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (owner.HasList)
			{
				var result = ListVariableHandler.ListApply(ref owner, lookup, value);
				if (result == SetVariableResult.Success)
					return result;
			}

			return owner.Store.SetVariable(lookup, value);
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return StoreLookup(owner, lookup);
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return StoreApply(ref owner, lookup, value);
		}
	}
}
