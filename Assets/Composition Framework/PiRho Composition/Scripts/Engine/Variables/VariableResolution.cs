using PiRhoSoft.Utilities;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public static class VariableResolution
	{
		public static void Assign(this IVariableCollection variables, Object context, VariableReference reference, Variable value)
		{
			if (reference.IsAssigned)
			{
				var result = reference.SetValue(variables, value);

				if (result != SetVariableResult.Success)
					LogAssignWarning(context, result, reference);
			}
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableValueSource source, out Variable result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value.Variable;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Variable result)
		{
			result = reference.GetValue(variables);
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, BoolVariableSource source, out bool result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out bool result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBool(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Bool);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, IntVariableSource source, out int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetInt(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Int);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, FloatVariableSource source, out float result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out float result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetFloat(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Int);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, Vector2IntVariableSource source, out Vector2Int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Vector2Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector2Int(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Vector2Int);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, Vector3IntVariableSource source, out Vector3Int result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Vector3Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector3Int(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Vector3Int);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, RectIntVariableSource source, out RectInt result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out RectInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetRectInt(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.RectInt);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, BoundsIntVariableSource source, out BoundsInt result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out BoundsInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBoundsInt(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.BoundsInt);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, Vector2VariableSource source, out Vector2 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Vector2 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector2(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Vector2);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, Vector3VariableSource source, out Vector3 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Vector3 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector3(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Vector3);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, Vector4VariableSource source, out Vector4 result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Vector4 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector4(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Vector4);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, QuaternionVariableSource source, out Quaternion result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Quaternion result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetQuaternion(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Quaternion);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, RectVariableSource source, out Rect result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Rect result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetRect(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Rect);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, BoundsVariableSource source, out Bounds result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Bounds result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBounds(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Bounds);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, ColorVariableSource source, out Color result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out Color result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetColor(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Color);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, StringVariableSource source, out string result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out string result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetString(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.String);
			return false;
		}

		public static bool Resolve<EnumType>(this IVariableCollection variables, Object context, VariableSource<EnumType> source, out EnumType result) where EnumType : struct, Enum
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve<EnumType>(this IVariableCollection variables, Object context, VariableReference reference, out EnumType result) where EnumType : struct, Enum
		{
			var value = reference.GetValue(variables);

			if (value.TryGetEnum(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Enum, typeof(EnumType));
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, DictionaryVariableSource source, out IVariableDictionary result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = null;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out IVariableDictionary result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetDictionary(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Dictionary);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, ListVariableSource source, out IVariableList result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, AssetVariableSource source, out AssetReference result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, SceneVariableSource source, out AssetReference result)
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.Resolve(context, source.Reference, out result);

			result = source.Value;
			return true;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out IVariableList result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetList(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.List);
			return false;
		}

		public static bool Resolve(this IVariableCollection variables, Object context, VariableReference reference, out AssetReference result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetAsset(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Asset);
			return false;
		}

		public static bool ResolveObject<ObjectType>(this IVariableCollection variables, Object context, VariableSource<ObjectType> source, out ObjectType result) where ObjectType : Object
		{
			if (source.Type == VariableSourceType.Reference)
				return variables.ResolveObject(context, source.Reference, out result);

			result = source.Value;
			return result;
		}

		public static bool ResolveObject<ObjectType>(this IVariableCollection variables, Object context, VariableReference reference, out ObjectType result) where ObjectType : Object
		{
			var value = reference.GetValue(variables);

			if (value.TryGetObject<Object>(out var obj))
			{
				result = obj.GetAsObject<ObjectType>();

				if (result != null)
					return true;
			}

			result = null;
			LogResolveWarning(context, value, reference, VariableType.Object, typeof(ObjectType));
			return false;
		}

		public static bool ResolveStore<StoreType>(this IVariableCollection variables, Object context, VariableReference reference, out StoreType result) where StoreType : class, IVariableCollection
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Dictionary, typeof(StoreType));
			return false;
		}

		public static bool ResolveIndex<ListType>(this IVariableCollection variables, Object context, VariableReference reference, out ListType result) where ListType : class, IVariableArray
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.List, typeof(ListType));
			return false;
		}

		public static bool ResolveInterface<InterfaceType>(this IVariableCollection variables, Object context, VariableReference reference, out InterfaceType result) where InterfaceType : class
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Object, typeof(InterfaceType));
			return false;
		}

		public static bool ResolveReference(this IVariableCollection variables, Object context, VariableReference reference, out object result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Object);
			return false;
		}

		public static bool ResolveAny<T>(this IVariableCollection variables, Object context, VariableReference reference, out T result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(context, value, reference, VariableType.Object);
			return false;
		}

		#region Logging

		private const string _missingVariableWarning = "(CVRMV) Failed to resolve variable '{0}'{1}: the variable could not be found";
		private const string _invalidVariableWarning = "(CVRIV) Failed to resolve variable '{0}'{1}: the variable has type '{2}' and should have type '{3}'";
		private const string _invalidEnumWarning = "(CVRIE) Failed to resolve variable '{0}'{1}: the variable has enum type '{2}' and should have enum type '{3}'";
		private const string _invalidObjectWarning = "(CVRIO) Failed to resolve variable '{0}'{1}: the object '{2}' is a '{3}' and cannot be converted to a '{4}'";
		private const string _invalidTypeWarning = "(CVRIT) Failed to resolve variable '{0}'{1}: the value is a '{2}' and cannot be converted to a '{3}'";

		private const string _missingAssignmentWarning = "(CVRMA) Failed to assign to variable '{0}'{1}: the variable could not be found";
		private const string _readOnlyAssignmentWarning = "(CVRROA) Failed to assign to variable '{0}'{1}: the variable is read only";
		private const string _invalidAssignmentWarning = "(CVrIA) Failed to assign to variable '{0}'{1}: the variable has an incompatible type";

		private static void LogResolveWarning(Object context, Variable value, VariableReference reference, VariableType expectedType, Type resolveType = null)
		{
			var description = context != null ? $" on {context.GetType().Name} '{context.name}'" : string.Empty;

			if (value.IsEmpty || value.IsNullObject)
				Debug.LogWarningFormat(context, _missingVariableWarning, reference, description);
			else if (value.Type == VariableType.Enum && resolveType != null)
				Debug.LogWarningFormat(context, _invalidEnumWarning, reference, description, value.EnumType.Name, resolveType.Name);
			else if (value.Type == VariableType.Object && resolveType != null)
				Debug.LogWarningFormat(context, _invalidObjectWarning, reference, description, value.AsObject, value.ObjectType.Name, resolveType.Name);
			else
				Debug.LogWarningFormat(context, _invalidVariableWarning, reference, description, value.Type, expectedType);
		}

		private static void LogAssignWarning(Object context, SetVariableResult result, VariableReference reference)
		{
			var description = context != null ? $" from {context.GetType().Name} '{context.name}'" : string.Empty;

			switch (result)
			{
				case SetVariableResult.NotFound: Debug.LogWarningFormat(context, _missingAssignmentWarning, reference, description); break;
				case SetVariableResult.ReadOnly: Debug.LogWarningFormat(context, _readOnlyAssignmentWarning, reference, description); break;
				case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(context, _invalidAssignmentWarning, reference, description); break;
			}
		}

		#endregion
	}
}
