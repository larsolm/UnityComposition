using System;

namespace PiRhoSoft.CompositionEngine
{
	public class ListVariableConstraint : VariableConstraint
	{
		public VariableType ItemType;
		public VariableConstraint ItemConstraint;

		public override string Write()
		{
			var constraint = ItemConstraint != null ? ItemConstraint.Write() : string.Empty;
			return string.Format("{0}|{1}", ItemType, constraint);
		}

		public override bool Read(string data)
		{
			var pipe = data.IndexOf('|');

			if (pipe < 0)
				return false;

			if (!Enum.TryParse(data.Substring(0, pipe), out ItemType))
				return false;
			
			ItemConstraint = VariableHandler.Get(ItemType).CreateConstraint(data.Substring(pipe + 1));
			return true;
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
