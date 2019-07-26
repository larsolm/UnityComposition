using PiRhoSoft.Utilities;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Object = UnityEngine.Object;

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
				builder.Append(variable.AsObject);
		}

		protected internal override void Save_(Variable value, BinaryWriter writer, SerializedData data)
		{
			if (value.TryGetObject<Object>(out var obj))
			{
				writer.Write(true);
				data.SaveReference(writer, obj);
			}
			else
			{
				writer.Write(false);
				data.SaveObject(writer, value.AsObject);
			}
		}

		protected internal override Variable Load_(BinaryReader reader, SerializedData data)
		{
			var isObject = reader.ReadBoolean();

			if (isObject)
			{
				var obj = data.LoadReference(reader);
				return Variable.Object(obj);
			}
			else
			{
				var obj = data.LoadObject<object>(reader);
				return Variable.Object(obj);
			}
		}

		protected internal override Variable Lookup_(Variable owner, Variable lookup)
		{
			if (owner.IsNullObject)
				return Variable.Empty;
			// TODO: try index
			else if (owner.TryGetObject<IVariableStore>(out var store))
				return LookupInStore(store, lookup);
			else if (ClassMap.Get(owner.ObjectType, out var map))
				return LookupWithMap(owner.AsObject, map, lookup);
			else
				return LookupWithReflection(owner.AsObject, lookup);
		}

		protected internal override SetVariableResult Apply_(ref Variable owner, Variable lookup, Variable value)
		{
			if (owner.IsNullObject)
				return SetVariableResult.NotFound;
			// TODO: try index
			else if (owner.TryGetObject<IVariableStore>(out var store))
				return ApplyToStore(store, lookup, value);
			else if (ClassMap.Get(owner.ObjectType, out var map))
				return ApplyWithMap(owner.AsObject, map, lookup, value);
			else
				return ApplyWithReflection(owner.AsObject, lookup, value);
		}

		protected internal override Variable Cast_(Variable owner, string type)
		{
			if (owner.TryGetObject<Object>(out var obj))
			{
				if (type == _gameObjectName)
				{
					var gameObject = ComponentHelper.GetAsGameObject(obj);
					return Variable.Object(gameObject);
				}
				else
				{
					var component = ComponentHelper.GetAsComponent(obj, type);
					return Variable.Object(component);
				}
			}

			return Variable.Empty;
		}

		protected internal override bool Test_(Variable owner, string type)
		{
			if (owner.TryGetObject<Object>(out var obj))
			{
				if (type == _gameObjectName)
				{
					var gameObject = ComponentHelper.GetAsGameObject(obj);
					return gameObject != null;
				}
				else
				{
					var component = ComponentHelper.GetAsComponent(obj, type);
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

		private static Variable LookupInStore(IVariableStore store, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
				return store.GetVariable(s);

			return Variable.Empty;
		}

		private static SetVariableResult ApplyToStore(IVariableStore store, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
				return store.SetVariable(s, value);
			else
				return SetVariableResult.TypeMismatch;
		}

		private static Variable LookupWithMap(object obj, IClassMap map, Variable lookup)
		{
			// TODO: maybe IClassIndex as well as IClassMap?

			if (lookup.TryGetString(out var s))
				return map.GetVariable(obj, s);

			return Variable.Empty;
		}

		private static SetVariableResult ApplyWithMap(object obj, IClassMap map, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
				return map.SetVariable(obj, s, value);

			return SetVariableResult.NotFound;
		}

		private static Variable LookupWithReflection(object obj, Variable lookup)
		{
			if (lookup.TryGetString(out var s))
			{
				var field = obj.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					try { return Variable.Unbox(field.GetValue(obj)); }
					catch { return Variable.Empty; }
				}

				var property = obj.GetType().GetProperty(s, BindingFlags.Instance | BindingFlags.Public);
				if (property != null)
				{
					try { return Variable.Unbox(property.GetValue(obj)); }
					catch { return Variable.Empty; }
				}
			}

			return Variable.Empty;
		}

		private static SetVariableResult ApplyWithReflection(object obj, Variable lookup, Variable value)
		{
			if (lookup.TryGetString(out var s))
			{
				var field = obj.GetType().GetField(s, BindingFlags.Instance | BindingFlags.Public);
				if (field != null)
				{
					try
					{
						field.SetValue(obj, value.Box());
						return SetVariableResult.Success;
					}
					catch (FieldAccessException)
					{
						return SetVariableResult.ReadOnly;
					}
					catch (ArgumentException)
					{
						return SetVariableResult.TypeMismatch;
					}
				}

				var property = obj.GetType().GetProperty(s, BindingFlags.Instance | BindingFlags.Public);
				if (property != null)
				{
					try
					{
						property.SetValue(obj, value.Box());
						return SetVariableResult.Success;
					}
					catch (FieldAccessException)
					{
						return SetVariableResult.ReadOnly;
					}
					catch (ArgumentException)
					{
						return SetVariableResult.TypeMismatch;
					}
				}
			}

			return SetVariableResult.NotFound;
		}
	}
}