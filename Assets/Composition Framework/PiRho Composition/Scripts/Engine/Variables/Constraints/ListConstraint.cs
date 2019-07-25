using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public class ListConstraint : VariableConstraint
	{
		public VariableType ItemType;
		public VariableConstraint ItemConstraint;

		public ListConstraint()
		{
		}

		public ListConstraint(VariableConstraint itemConstraint)
		{
			ItemConstraint = itemConstraint;
		}

		public override Variable Generate()
		{
			return Variable.List(new VariableList());
		}

		public override bool IsValid(Variable variable)
		{
			if (variable.TryGetList(out var list))
			{
				for (var i = 0; i < list.Count; i++)
				{
					var item = list.GetVariable(i);

					if (ItemConstraint != null)
					{
						if (!ItemConstraint.IsValid(item))
							return false;
					}
					else if (ItemType != VariableType.Empty)
					{
						if (!item.Is(ItemType))
							return false;
					}
				}

				return true;
			}

			return false;
		}

		public override void Save(BinaryWriter writer, SerializedData data)
		{
			writer.Write((int)ItemType);
			data.SaveObject(writer, ItemConstraint);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			ItemType = (VariableType)reader.ReadInt32();
			ItemConstraint = data.LoadObject<VariableConstraint>(reader);
		}
	}
}