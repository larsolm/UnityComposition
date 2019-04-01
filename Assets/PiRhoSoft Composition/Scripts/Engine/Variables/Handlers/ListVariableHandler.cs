﻿using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	public class ListVariableHandler : VariableHandler
	{
		public const char CountSymbol = '#';

		protected override VariableConstraint CreateConstraint() => new ListVariableConstraint();

		public override VariableValue CreateDefault(VariableConstraint constraint)
		{
			return VariableValue.Create(new VariableList());
		}

		public override void Write(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			var list = value.List as VariableList;

			if (list != null)
			{
				writer.Write(list.Count);

				for (var i = 0; i < list.Count; i++)
					list.GetVariable(i).Write(writer, objects);
			}
			else
			{
				writer.Write(-1);
			}
		}

		public override void Read(ref VariableValue value, BinaryReader reader, List<Object> objects)
		{
			var count = reader.ReadInt32();

			if (count >= 0)
			{
				var list = new VariableList();

				for (var i = 0; i < count; i++)
				{
					var item = new VariableValue();

					item.Read(reader, objects);
					list.AddVariable(item);
				}

				value = VariableValue.Create(list);
			}
		}

		public static VariableValue ListLookup(VariableValue owner, string lookup)
		{
			if (lookup.Length == 1 && lookup[0] == CountSymbol)
				return VariableValue.Create(owner.List.Count);
			else if (int.TryParse(lookup, out var index) && index >= 0 && index < owner.List.Count)
				return owner.List.GetVariable(index);
			else
				return VariableValue.Empty;
		}

		public static SetVariableResult ListApply(ref VariableValue owner, string lookup, VariableValue value)
		{
			if (int.TryParse(lookup, out var index) && index >= 0 && index < owner.List.Count)
				return owner.List.SetVariable(index, value);
			else if (lookup.Length == 1 && lookup[0] == CountSymbol)
				return SetVariableResult.ReadOnly;
			else
				return SetVariableResult.NotFound;
		}

		public override VariableValue Lookup(VariableValue owner, string lookup)
		{
			return ListLookup(owner, lookup);
		}

		public override SetVariableResult Apply(ref VariableValue owner, string lookup, VariableValue value)
		{
			return ListApply(ref owner, lookup, value);
		}

		public override bool? IsEqual(VariableValue left, VariableValue right)
		{
			if (right.HasReference)
				return left.Reference == right.Reference;
			else
				return null;
		}
	}
}
