using PiRhoSoft.Utilities;
using System;

namespace PiRhoSoft.Composition
{
	[Serializable]
	public class DictionaryConstraint : VariableConstraint
	{
		public VariableSchema Schema;

		public override VariableType Type => VariableType.Dictionary;

		public DictionaryConstraint()
		{
		}

		public DictionaryConstraint(VariableSchema schema)
		{
			Schema = schema;
		}

		public override string ToString()
		{
			return Schema?.name ?? string.Empty;
		}

		public override Variable Generate()
		{
			if (Schema == null)
			{
				return Variable.Dictionary(new VariableDictionary());
			}
			else
			{
				var collection = new VariableCollection();
				collection.SetSchema(Schema);
				return Variable.Dictionary(collection);
			}
		}

		public override bool IsValid(Variable variable)
		{
			return variable.IsDictionary && (Schema == null);// || variable.AsDictionary.Imple == Schema); TODO: move ImplementsSchema (and maybe ApplySchema as well) to VariableSchema
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.Writer.Write(Schema != null);

			if (Schema != null)
				writer.SaveReference(Schema);
		}

		public override void Load(SerializedDataReader reader)
		{
			var hasSchema = reader.Reader.ReadBoolean();

			if (hasSchema)
				Schema = reader.LoadReference() as VariableSchema;
		}
	}
}
