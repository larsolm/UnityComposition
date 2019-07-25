using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class StoreVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable value, StringBuilder builder)
		{
			builder.Append(value.AsStore);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var store = variable.AsStore as VariableStore;

			writer.Write(store != null);

			if (store != null)
			{
				if (store is ISchemaOwner schemaOwner)
				{
					writer.Write(true);
					data.SaveReference(writer, schemaOwner.Schema);
				}
				else
				{
					writer.Write(false);
				}

				writer.Write(store.Variables.Count);

				for (var i = 0; i < store.Names.Count && i < store.Variables.Count; i++)
				{
					writer.Write(store.Names[i]);
					Save(store.Variables[i], writer, data);
				}
			}
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var isValid = reader.ReadBoolean();

			var hasSchema = isValid && reader.ReadBoolean();
			var schema = hasSchema ? data.LoadReference(reader) as VariableSchema : null;

			var store = schema != null ? new ConstrainedStore(schema) : new VariableStore();
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var variable = Load(reader, data);

				if (schema != null)
					store.SetVariable(name, variable);
				else
					store.AddVariable(name, variable);
			}

			return Variable.Store(store);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			return Lookup(owner.AsStore, lookup);
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			return Apply(owner.AsStore, lookup, value);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetStore(out var store))
				return left.AsStore == store;
			else
				return null;
		}
	}
}