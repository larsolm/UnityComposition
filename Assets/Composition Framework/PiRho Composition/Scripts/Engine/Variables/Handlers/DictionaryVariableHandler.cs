using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class DictionaryVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable value, StringBuilder builder)
		{
			builder.Append(value.AsDictionary);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var dictionary = variable.AsDictionary;
			var names = dictionary.VariableNames;

			data.SaveReference(writer, dictionary.Schema);
			writer.Write(names.Count);

			for (var i = 0; i < names.Count; i++)
			{
				var name = names[i];
				var value = dictionary.GetVariable(name);

				writer.Write(name);
				Save(value, writer, data);
			}
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var schema = data.LoadReference(reader) as VariableSchema;
			var dictionary = new VariableDictionary(schema);
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.ReadString();
				var variable = Load(reader, data);

				dictionary.SetVariable(name, variable);
			}

			return Variable.Dictionary(dictionary);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
				return owner.AsDictionary.GetVariable(s);

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
				return owner.AsDictionary.SetVariable(s, value);
			else
				return SetVariableResult.TypeMismatch;
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetDictionary(out var dictionary))
				return left.AsDictionary == dictionary;
			else
				return null;
		}
	}
}