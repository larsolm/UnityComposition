using PiRhoSoft.Utilities;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	internal class ObjectVariableHandler : VariableHandler
	{
		private const string NullString = "(null)";
		private const string GameObjectName = "GameObject";

		protected internal override string ToString_(Variable variable)
		{
			if (variable.IsNullObject)
				return NullString;
			else
				return variable.AsObject.ToString();
		}

		protected internal override void Save_(Variable value, SerializedDataWriter writer)
		{
			if (value.TryGetObject<Object>(out var obj))
			{
				writer.Writer.Write(true);
				writer.SaveReference(obj);
			}
			else
			{
				writer.Writer.Write(false);
				writer.SaveInstance(value.AsObject);
			}
		}

		protected internal override Variable Load_(SerializedDataReader reader)
		{
			var isObject = reader.Reader.ReadBoolean();

			if (isObject)
			{
				var obj = reader.LoadReference();
				return Variable.Object(obj);
			}
			else
			{
				var obj = reader.LoadInstance<object>();
				return Variable.Object(obj);
			}
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (owner.IsNullObject)
				return Variable.Empty;
			else if (owner.TryGetArray(out var array) && lookup.IsInt)
				return array.GetVariable(lookup.AsInt);
			else if (lookup.IsString && owner.TryGetCollection(out var collection))
				return collection.GetVariable(lookup.AsString);
			else
				return Variable.Empty;
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (owner.IsNullObject)
				return SetVariableResult.NotFound;
			else if (lookup.IsInt && owner.TryGetArray(out var array))
				return array.SetVariable(lookup.AsInt, value);
			else if (lookup.IsString && owner.TryGetCollection(out var collection))
				return collection.SetVariable(lookup.AsString, value);
			else
				return SetVariableResult.NotFound;
		}

		protected internal override Variable Cast_(Variable owner, string type)
		{
			if (owner.TryGetObject<Object>(out var obj))
			{
				if (type == GameObjectName)
				{
					var gameObject = obj.GetAsGameObject();
					return Variable.Object(gameObject);
				}
				else
				{
					var component = obj.GetAsComponent(type);
					return Variable.Object(component);
				}
			}

			return Variable.Empty;
		}

		protected internal override bool Test_(Variable owner, string type)
		{
			if (owner.TryGetObject<Object>(out var obj))
			{
				if (type == GameObjectName)
				{
					var gameObject = obj.GetAsGameObject();
					return gameObject != null;
				}
				else
				{
					var component = obj.GetAsComponent(type);
					return component != null;
				}
			}

			return false;
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
