using PiRhoSoft.Utilities;
using System.IO;

namespace PiRhoSoft.Composition
{
	public class ListConstraint : VariableConstraint
	{
		public override VariableType Type => VariableType.List;

		public VariableType ItemType { get => _itemType; set => SetType(value); }
		public VariableConstraint ItemConstraint { get => _itemConstraint; set => SetConstraint(value); }

		private VariableType _itemType;
		private VariableConstraint _itemConstraint;

		public ListConstraint()
		{
			ItemType = VariableType.Empty;
		}

		public ListConstraint(VariableType type)
		{
			ItemType = type;
		}

		public ListConstraint(VariableConstraint itemConstraint)
		{
			ItemConstraint = itemConstraint;
		}

		public override string ToString()
		{
			if (ItemConstraint == null)
				return Type.ToString();
			else
				return string.Format($"{ItemType}({ItemConstraint})");
		}

		public override Variable Generate()
		{
			return Variable.List(new VariableList());
		}

		public override bool IsValid(Variable variable)
		{
			if (variable.TryGetList(out var list))
			{
				for (var i = 0; i < list.VariableCount; i++)
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
			data.SaveInstance(writer, ItemConstraint);
		}

		public override void Load(BinaryReader reader, SerializedData data)
		{
			ItemType = (VariableType)reader.ReadInt32();
			ItemConstraint = data.LoadInstance<VariableConstraint>(reader);
		}

		private void SetType(VariableType type)
		{
			_itemType = type;
			_itemConstraint = Create(type);
		}

		private void SetConstraint(VariableConstraint constraint)
		{
			_itemType = constraint?.Type ?? VariableType.Empty;
			_itemConstraint = constraint;
		}
	}
}