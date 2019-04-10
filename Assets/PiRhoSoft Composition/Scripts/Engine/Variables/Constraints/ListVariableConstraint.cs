using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace PiRhoSoft.CompositionEngine
{
	public class ListVariableConstraint : VariableConstraint
	{
		public VariableType ItemType;
		public VariableConstraint ItemConstraint;

		public override string Write(IList<Object> objects)
		{
			var constraint = ItemConstraint != null ? ItemConstraint.Write(objects) : string.Empty;
			return string.Format("{0}|{1}", ItemType, constraint);
		}

		public override bool Read(string data, IList<Object> objects)
		{
			var pipe = data.IndexOf('|');

			if (pipe < 0)
				return false;

			if (!Enum.TryParse(data.Substring(0, pipe), out ItemType))
				return false;
			
			ItemConstraint = VariableHandler.CreateConstraint(ItemType, data.Substring(pipe + 1), objects);
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
