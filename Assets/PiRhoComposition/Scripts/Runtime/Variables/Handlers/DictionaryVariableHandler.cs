using PiRhoSoft.Utilities;

namespace PiRhoSoft.Composition
{
	internal class DictionaryVariableHandler : VariableHandler
	{
		private const string DictionaryString = "(Dictionary)";

		protected internal override string ToString_(Variable variable)
		{
			return DictionaryString;
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			var dictionary = variable.AsDictionary;
			var names = dictionary.VariableNames;

			writer.Writer.Write(names.Count);

			for (var i = 0; i < names.Count; i++)
			{
				var name = names[i];
				var value = dictionary.GetVariable(name);

				writer.Writer.Write(name);
				Save(value, writer);
			}
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var dictionary = new VariableDictionary();
			var count = reader.Reader.ReadInt32();

			for (var i = 0; i < count; i++)
			{
				var name = reader.Reader.ReadString();
				var variable = Load(reader);

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
