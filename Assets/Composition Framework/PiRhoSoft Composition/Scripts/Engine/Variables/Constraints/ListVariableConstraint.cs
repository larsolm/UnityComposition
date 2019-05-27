using System.Collections.Generic;
using System.IO;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ListVariableConstraint : VariableConstraint
	{
		public VariableType ItemType;
		public VariableConstraint ItemConstraint;

		protected internal override void Write(BinaryWriter writer, IList<Object> objects)
		{
			writer.Write((int)ItemType);
			writer.Write(ItemConstraint != null);

			if (ItemConstraint != null)
				VariableHandler.WriteConstraint(ItemType, ItemConstraint, writer, objects);
		}

		protected internal override void Read(BinaryReader reader, IList<Object> objects, short version)
		{
			ItemType = (VariableType)reader.ReadInt32();

			var constrained = reader.ReadBoolean();

			if (constrained)
				ItemConstraint = VariableHandler.ReadConstraint(reader, objects, version);
		}

		public override bool IsValid(VariableValue value)
		{
			if (ItemType != VariableType.Empty)
			{
				var list = value.List;

				for (var i = 0; i < list.Count; i++)
				{
					var item = list.GetVariable(i);

					if (item.Type != ItemType)
						return false;

					if (ItemConstraint != null && !ItemConstraint.IsValid(item))
						return false;
				}
			}

			return true;
		}
	}
}
