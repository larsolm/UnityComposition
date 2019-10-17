using PiRhoSoft.Utilities;

namespace PiRhoSoft.Composition
{
	internal class ListVariableHandler : VariableHandler
	{
		private const string ListCountName = "Count";
		private const string ListString = "(List)";

		protected internal override string ToString_(Variable variable)
		{
			return ListString;
		}

		protected internal override void Save_(Variable variable, SerializedDataWriter writer)
		{
			var list = variable.AsList as VariableList;

			if (list != null)
			{
				writer.Writer.Write(list.VariableCount);

				for (var i = 0; i < list.VariableCount; i++)
					Save(variable, writer);
			}
			else
			{
				writer.Writer.Write(-1);
			}
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var count = reader.Reader.ReadInt32();
			var list = new VariableList();

			for (var i = 0; i < count; i++)
			{
				var item = Load(reader);
				list.AddVariable(item);
			}

			return Variable.List(list);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				if (s == ListCountName)
					return Variable.Int(owner.AsList.VariableCount);
			}
			else if (lookup.TryGetInt(out var i))
			{
				if (i >= 0 && i < owner.AsList.VariableCount)
					return owner.AsList.GetVariable(i);
			}

			return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
			{
				if (s == ListCountName)
					return SetVariableResult.ReadOnly;
				else
					return SetVariableResult.NotFound;
			}
			else if (lookup.TryGetInt(out var i))
			{
				if (i >= 0 && i < owner.AsList.VariableCount)
					return owner.AsList.SetVariable(i, value);
				else
					return SetVariableResult.NotFound;
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
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
