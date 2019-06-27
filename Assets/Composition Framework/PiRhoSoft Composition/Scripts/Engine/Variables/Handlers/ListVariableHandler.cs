using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.CompositionEngine
{
	internal class ListVariableHandler : VariableHandler
	{
		public const string CountText = "Count";

		protected internal override VariableValue CreateDefault_(VariableConstraint constraint)
		{
			return VariableValue.Create(new VariableList());
		}

		protected internal override void ToString_(VariableValue value, StringBuilder builder)
		{
			builder.Append(value.List);
		}

		protected internal override void Write_(VariableValue value, BinaryWriter writer, List<Object> objects)
		{
			var list = value.List as VariableList;

			if (list != null)
			{
				writer.Write(list.Count);

				for (var i = 0; i < list.Count; i++)
					WriteValue(list.GetVariable(i), writer, objects);
			}
			else
			{
				writer.Write(-1);
			}
		}

		protected internal override VariableValue Read_(BinaryReader reader, List<Object> objects, short version)
		{
			var count = reader.ReadInt32();
			var list = new VariableList();

			for (var i = 0; i < count; i++)
			{
				var item = ReadValue(reader, objects, version);
				list.AddVariable(item);
			}

			return VariableValue.Create(list);
		}

		protected internal override VariableValue Lookup_(VariableValue owner, VariableValue lookup)
		{
			return LookupInList(owner, lookup);
		}

		protected internal override SetVariableResult Apply_(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			return ApplyToList(ref owner, lookup, value);
		}

		protected internal override bool? IsEqual_(VariableValue left, VariableValue right)
		{
			if (right.HasReference)
				return left.Reference == right.Reference;
			else
				return null;
		}

		public static VariableValue LookupInList(VariableValue owner, VariableValue lookup)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == CountText)
					return VariableValue.Create(owner.List.Count);
			}
			else if (lookup.Type == VariableType.Int)
			{
				if (lookup.Int >= 0 && lookup.Int < owner.List.Count)
					return owner.List.GetVariable(lookup.Int);
			}

			return VariableValue.Empty;
		}

		public static SetVariableResult ApplyToList(ref VariableValue owner, VariableValue lookup, VariableValue value)
		{
			if (lookup.Type == VariableType.String)
			{
				if (lookup.String == CountText)
					return SetVariableResult.ReadOnly;
				else
					return SetVariableResult.NotFound;
			}
			else if (lookup.Type == VariableType.Int)
			{
				if (lookup.Int >= 0 && lookup.Int < owner.List.Count)
					return owner.List.SetVariable(lookup.Int, value);
				else
					return SetVariableResult.NotFound;
			}
			else
			{
				return SetVariableResult.TypeMismatch;
			}
		}
	}
}
