using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class StoreVariableConstraint : VariableConstraint
	{
		public VariableSchema Schema;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write(objects.Count);
			objects.Add(Schema);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			var index = reader.ReadInt32();
			Schema = objects[index] as VariableSchema;
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
