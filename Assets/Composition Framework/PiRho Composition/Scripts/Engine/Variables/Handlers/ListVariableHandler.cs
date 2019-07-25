using PiRhoSoft.Utilities;
using System.IO;
using System.Text;

namespace PiRhoSoft.Composition
{
	internal class ListVariableHandler : VariableHandler
	{
		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			builder.Append(variable.AsList);
		}

		protected internal override void Save_(Variable variable, BinaryWriter writer, SerializedData data)
		{
			var list = variable.AsList as VariableList;

			if (list != null)
			{
				writer.Write(list.Count);

				for (var i = 0; i < list.Count; i++)
					Save(variable, writer, data);
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var count = reader.ReadInt32();
			var list = new VariableList();

			for (var i = 0; i < count; i++)
			{
				var item = Load(reader, data);
				list.AddVariable(item);
			}

			return Variable.List(list);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			return Lookup(owner.AsList, lookup);
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			return Apply(owner.AsList, lookup, value);
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.TryGetList(out var list))
				return left.AsList == list;
			else
				return null;
		}
	}
}