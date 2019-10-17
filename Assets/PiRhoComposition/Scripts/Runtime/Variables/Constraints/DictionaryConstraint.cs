using PiRhoSoft.Utilities;
using System;
using System.IO;

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
			var dictionary = new VariableDictionary();

			if (Schema != null)
				dictionary.ApplySchema(Schema, null);

			return Variable.Dictionary(dictionary);
		}

		public override bool IsValid(Variable variable)
		{
			return variable.IsDictionary && (Schema == null);// || variable.AsDictionary.Imple == Schema); TODO: move ImplementsSchema (and maybe ApplySchema as well) to VariableSchema
		}

		public override void Save(SerializedDataWriter writer)
		{
			writer.SaveReference(Schema);
		}

		public override void Load(SerializedDataReader reader)
		{
			Schema = reader.LoadReference() as VariableSchema;
		}
	}
}
