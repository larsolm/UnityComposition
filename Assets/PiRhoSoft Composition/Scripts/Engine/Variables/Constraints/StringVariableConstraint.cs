using System.Linq;

namespace PiRhoSoft.CompositionEngine
{
	public class StringVariableConstraint : VariableConstraint
	{
		private const char _emptyCharacter = '-';
		private const char _nonEmptyCharacter = '+';
		private const char _separatorCharacter = ',';

		public string[] Values;

		public override string Write()
		{
			// A character needs to precede the values array so that it is not stored as an empty string which is checked for in the CreateConstraint method
			return Values.Length == 0 ? _emptyCharacter.ToString() : _nonEmptyCharacter + string.Join(_separatorCharacter.ToString(), Values);
		}

		public override bool Read(string data)
		{
			// string.Split returns a single length array if the string is empty but we want an empty array if there are no values to parse
			var empty = data[0];
			if (empty == _emptyCharacter)
				Values = new string[] { };
			else if (empty == _nonEmptyCharacter)
				Values = data.Substring(1).Split(_separatorCharacter);
			else
				return false;

			return true;
		}

		public override bool IsValid(VariableValue value)
		{
			return Values.Length == 0 || Values.Contains(value.String);
		}
	}
}
