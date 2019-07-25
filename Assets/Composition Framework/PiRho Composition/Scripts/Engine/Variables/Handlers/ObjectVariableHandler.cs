using PiRhoSoft.Utilities;
using System.IO;
using System.Text;
using UnityEngine;

namespace PiRhoSoft.Composition
{
	internal class ObjectVariableHandler : VariableHandler
	{
		public const string NullText = "(null)";

		private const string _gameObjectName = "GameObject";

		protected internal override void ToString_(Variable variable, StringBuilder builder)
		{
			if (variable.IsNullObject)
				builder.Append(NullText);
			else
				builder.Append(variable.AsObject.name);
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			data.SaveReference(writer, value.AsObject);
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var obj = data.LoadReference(reader);
			return Variable.Object(obj);
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (owner.IsNullObject)
				return Variable.Empty;
			else
				return Lookup(owner.AsObject, lookup);
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (owner.IsNullObject)
				return SetVariableResult.NotFound;
			else
				return Apply(owner.AsObject, lookup, value);
		}

		protected internal override Variable Cast_(Variable owner, string type)
		{
			if (type == _gameObjectName)
			{
				var gameObject = ComponentHelper.GetAsGameObject(owner.AsObject);
				return Variable.Object(gameObject);
			}
			else
			{
				var component = ComponentHelper.GetAsComponent(owner.AsObject, type);
				return Variable.Object(component);
			}
		}

		protected internal override bool Test_(Variable owner, string type)
		{
			if (type == _gameObjectName)
			{
				var gameObject = ComponentHelper.GetAsGameObject(owner.AsObject);
				return gameObject != null;
			}
			else
			{
				var component = ComponentHelper.GetAsComponent(owner.AsObject, type);
				return component != null;
			}
		}

		protected internal override bool? IsEqual_(Variable left, Variable right)
		{
			if (right.IsEmpty || right.IsNullObject)
				return left.IsNullObject;
			else if (right.TryGetObject(out var obj))
				return left.AsObject == obj;
			else
				return null;
		}
	}
}
