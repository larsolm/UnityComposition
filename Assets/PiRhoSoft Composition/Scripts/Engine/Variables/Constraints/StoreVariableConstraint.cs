using System.Collections.Generic;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableConstraint : VariableConstraint
	{
		public VariableSchema Schema;

		public override string Write(IList<Object> objects)
		{
			objects.Add(Schema);
			return (objects.Count - 1).ToString();
		}

		public override bool Read(string data, IList<Object> objects)
		{
			if (int.TryParse(data, out var index) && index >= 0 && index < objects.Count)
			{
				Schema = objects[index] as VariableSchema;
				return true;
			}

			return false;
		}

		public override bool IsValid(VariableValue value)
		{
			if (Schema != null && value.TryGetReference<ISchemaOwner>(out var owner))
				return owner.Schema == Schema;
			else
				return false;
		}
	}
}
