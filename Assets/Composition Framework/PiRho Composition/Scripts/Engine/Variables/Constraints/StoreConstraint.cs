using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public class StoreConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.Store;

		public VariableSchema Schema;

		public StoreConstraint()
		{
		}

		public StoreConstraint(VariableSchema schema)
		{
			Schema = schema;
		}

		public override Variable Generate()
		{
			var store = Schema == null
				? new VariableStore()
				: new ConstrainedStore(Schema);

			return Variable.Store(store);
		}

		public override bool IsValid(Variable variable)
		{
			// TODO: would be cool to have a more complex determination based on the store meeting the
			// requirements of the Schema rather than having to actually use it

			if (Schema != null && variable.TryGet<ISchemaOwner>(out var owner))
				return owner.Schema == Schema;
			else
				return false;
		}

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			data.SaveReference(writer, Schema);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			Schema = data.LoadReference(reader) as VariableSchema;
		}
	}
}