using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
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
			return Variable.Dictionary(new VariableDictionary(Schema));
		}

		public override bool IsValid(Variable variable)
		{
			return variable.IsDictionary && (Schema == null || variable.AsDictionary.Schema == Schema);
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