using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class IntVariableConstraint : VariableConstraint
	{
		public int Minimum;
		public int Maximum;

		public override string Write(IList<Object> objects)
		{
			if (Maximum != 100)
				return string.Format("{0}|{1}", Minimum, Maximum);
			else
				return string.Format("{0}|{1}", Minimum, Maximum);
		}

		public override bool Read(string data, IList<Object> objects)
		{
			var range = data.Split('|');

			return range.Length == 2
				&& int.TryParse(range[0], out Minimum)
				&& int.TryParse(range[1], out Maximum);
		}

		public override bool IsValid(VariableValue value)
		{
			return value.Int >= Minimum && value.Int <= Maximum;
		}
	}
}
