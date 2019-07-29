﻿using PiRhoSoft.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PiRhoSoft.Composition
{
	public class BindingAnimationStatus
	{
		private int _count = 0;

		public void Reset() => _count = 0;
		public bool IsFinished() => _count <= 0;
		public void Increment() =>  _count++;
		public void Decrement() => _count--;
	}

	public abstract class VariableBinding : MonoBehaviour
	{
		private const string _missingVariableWarning = "(CVBMV) Failed to resolve variable '{0}' on binding '{1}': the variable could not be found";
		private const string _invalidVariableWarning = "(CVBIV) Failed to resolve variable '{0}' on binding '{1}': the variable has type '{2}' and should have type '{3}'";
		private const string _invalidEnumWarning = "(CVBIE) Failed to resolve variable '{0}' on binding '{1}': the variable has enum type '{2}' and should have enum type '{3}'";
		private const string _invalidObjectWarning = "(CVBIO) Failed to resolve variable '{0}' on node '{1}': the object '{2}' is a '{3}' and cannot be converted to a '{4}'";
		private const string _invalidTypeWarning = "(CVBIT) Failed to resolve variable '{0}' on node '{1}': the value is a '{2}' and cannot be converted to a '{3}'";

		private const string _missingAssignmentWarning = "(CVBMA) Failed to assign to variable '{0}' from binding '{1}': the variable could not be found";
		private const string _readOnlyAssignmentWarning = "(CVBROA) Failed to assign to variable '{0}' from binding '{1}': the variable is read only";
		private const string _invalidAssignmentWarning = "(CVBIA) Failed to assign to variable '{0}' from binding '{1}': the variable has an incompatible type";

		[Tooltip("The group to which this binding belongs (empty means it will update with all groups)")]
		public string BindingGroup = string.Empty;

		[Tooltip("When set, the binding will update automatically when the variable changes")]
		public bool AutoUpdate = true;

		[Tooltip("When set, errors in resolving the binding will be treated as a valid condition that hides or disables corresponding components")]
		public bool SuppressErrors = false;

		private IVariableCollection _variables;

		public IVariableCollection Variables
		{
			get
			{
				// can't look up in awake because it's possible to update bindings before the component is enabled

				if (_variables == null)
					_variables = BindingRoot.FindParent(gameObject);

				return _variables;
			}
		}

		protected virtual void Awake()
		{
			CompositionManager.Instance.AddBinding(this);
		}

		protected virtual void OnDestroy()
		{
			if (CompositionManager.Exists)
				CompositionManager.Instance.RemoveBinding(this);
		}

		public void UpdateBinding(string group, BindingAnimationStatus status)
		{
			if (string.IsNullOrEmpty(group) || BindingGroup == group)
				UpdateBinding(Variables, status ?? _ignoredStatus);
		}

		protected abstract void UpdateBinding(IVariableCollection variables, BindingAnimationStatus status);

		#region Static Helpers

		private static BindingAnimationStatus _ignoredStatus = new BindingAnimationStatus();
		private static List<VariableBinding> _bindings = new List<VariableBinding>();

		public static void UpdateBinding(GameObject obj, string group, BindingAnimationStatus status)
		{
			UpdateBinding(obj, group, status, _bindings);
		}

		public static void UpdateBinding(GameObject obj, string group, BindingAnimationStatus status, List<VariableBinding> bindings)
		{
			GetBindings(obj, true, bindings);

			foreach (var binding in bindings)
				binding.UpdateBinding(group, status);
		}

		private static void GetBindings(GameObject obj, bool includeChildren, List<VariableBinding> bindings)
		{
			bindings.Clear();

			if (includeChildren)
				obj.GetComponentsInChildren(true, bindings); // includes components directly on obj
			else
				obj.GetComponents(bindings); // always includes inactive
		}

		#endregion

		#region Variable Lookup

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Variable result)
		{
			result = reference.GetValue(variables);
			return !result.IsEmpty;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out bool result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBool(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Bool);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetInt(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out float result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetFloat(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Int);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Vector2Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector2Int(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector2Int);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Vector3Int result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector3Int(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector3Int);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out RectInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetRectInt(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.RectInt);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out BoundsInt result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBoundsInt(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.BoundsInt);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Vector2 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector2(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector2);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Vector3 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector3(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector3);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Vector4 result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetVector4(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Vector4);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Quaternion result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetQuaternion(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Quaternion);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Rect result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetRect(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Rect);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Bounds result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetBounds(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Bounds);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out Color result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetColor(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Color);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out string result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetString(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.String);
			return false;
		}

		public bool Resolve<EnumType>(IVariableCollection variables, VariableReference reference, out EnumType result) where EnumType : struct, Enum
		{
			var value = reference.GetValue(variables);

			if (value.TryGetEnum(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Enum, typeof(EnumType));
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out IVariableDictionary result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetDictionary(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Dictionary);
			return false;
		}

		public bool Resolve(IVariableCollection variables, VariableReference reference, out IVariableList result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetList(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.List);
			return false;
		}

		public bool ResolveObject<ObjectType>(IVariableCollection variables, VariableReference reference, out ObjectType result) where ObjectType : Object
		{
			var value = reference.GetValue(variables);

			if (value.HasObject<Object>())
			{
				result = ComponentHelper.GetAsObject<ObjectType>(value.GetObject<Object>());

				if (result != null)
					return true;
			}

			result = null;
			LogResolveWarning(value, reference, VariableType.Object, typeof(ObjectType));
			return false;
		}

		public bool ResolveDictionary(IVariableCollection variables, VariableReference reference, out IVariableDictionary result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetDictionary(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Dictionary);
			return false;
		}

		public bool ResolveList(IVariableCollection variables, VariableReference reference, out IVariableList result)
		{
			var value = reference.GetValue(variables);

			if (value.TryGetList(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.List);
			return false;
		}

		public bool ResolveInterface<InterfaceType>(IVariableCollection variables, VariableReference reference, out InterfaceType result) where InterfaceType : class
		{
			var value = reference.GetValue(variables);

			if (value.TryGet(out result))
				return true;

			LogResolveWarning(value, reference, VariableType.Object, typeof(InterfaceType));
			return false;
		}

		public bool ResolveReference(IVariableCollection variables, VariableReference reference, out object result)
		{
			var value = reference.GetValue(variables);

			if (value.IsObject)
			{
				result = value.AsObject;
				return true;
			}

			result = null;
			LogResolveWarning(value, reference, VariableType.Object);
			return false;
		}

		private void LogResolveWarning(Variable value, VariableReference reference, VariableType expectedType, Type resolveType = null)
		{
			if (!SuppressErrors)
			{
				if (value.IsEmpty || value.IsNullObject)
					Debug.LogWarningFormat(this, _missingVariableWarning, reference, name);
				else if (value.Type == VariableType.Enum && resolveType != null)
					Debug.LogWarningFormat(this, _invalidEnumWarning, reference, name, value.EnumType.Name, resolveType.Name);
				else if (value.Type == VariableType.Object && resolveType != null)
					Debug.LogWarningFormat(this, _invalidObjectWarning, reference, name, value.AsObject, value.ObjectType.Name, resolveType.Name);
				else
					Debug.LogWarningFormat(this, _invalidVariableWarning, reference, name, value.Type, expectedType);
			}
		}

		#endregion

		#region Variable Assignment

		public void Assign(IVariableCollection variables, VariableReference reference, Variable value)
		{
			if (reference.IsAssigned)
			{
				var result = reference.SetValue(variables, value);

				switch (result)
				{
					case SetVariableResult.NotFound: Debug.LogWarningFormat(this, _missingAssignmentWarning, reference, name); break;
					case SetVariableResult.ReadOnly: Debug.LogWarningFormat(this, _readOnlyAssignmentWarning, reference, name); break;
					case SetVariableResult.TypeMismatch: Debug.LogWarningFormat(this, _invalidAssignmentWarning, reference, name); break;
				}
			}
		}

		#endregion
	}
}
