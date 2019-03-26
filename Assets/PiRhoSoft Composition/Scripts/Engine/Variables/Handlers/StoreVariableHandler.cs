using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableHandler : ListVariableHandler
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

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			if (owner.HasList)
				return base.Lookup(owner, lookup);
			else
				return VariableValue.Empty;
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (owner.HasList)
				return base.Apply(ref owner, lookup, value);
			else
				return SetVariableResult.NotFound;
		}
	}
}
