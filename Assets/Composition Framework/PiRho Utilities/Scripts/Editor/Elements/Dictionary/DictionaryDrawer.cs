using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(DictionaryAttribute))]
	class DictionaryDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUDDIT) invalid type for DictionaryAttribute on field '{0}': Dictionary can only be applied to SerializedDictionary fields";

		private const string _missingAllowAddMethodWarning = "(PUDDMAAM) invalid method for AllowAdd on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingAllowRemoveMethodWarning = "(PUDDMARM) invalid method for AllowRemove on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingAllowReorderMethodWarning = "(PUDDMAROM) invalid method for AllowReorder on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAllowAddMethodWarning = "(PUDDIAAM) invalid method for AllowAdd on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidAllowRemoveMethodWarning = "(PUDDIARM) invalid method for AllowRemove on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidAllowReorderMethodWarning = "(PUDDIAROM) invalid method for AllowReorder on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";

		private const string _missingAddMethodWarning = "(PUDDMAM) invalid method for AddCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingRemoveMethodWarning = "(PUDDMRM) invalid method for RemoveCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingReorderMethodWarning = "(PUDDMROM) invalid method for ReorderCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAddMethodWarning = "(PUDDIAM) invalid method for AddCallback on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidRemoveMethodWarning = "(PUDDIRM) invalid method for RemoveCallback on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidReorderMethodWarning = "(PUDDIROM) invalid method for ReorderCallback on field '{0}': the method '{1}' should take 0, 1, or 2 int parameters";

		private static readonly object[] _oneParameter = new object[1];

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var keys = property.FindPropertyRelative("_keys");
			var values = property.FindPropertyRelative("_values");

			if (keys != null && keys.isArray && values != null && values.isArray && keys.arrayElementType == "string")
			{
				var dictionaryAttribute = attribute as DictionaryAttribute;
				var itemDrawer = this.GetNextDrawer();
				var tooltip = this.GetTooltip();
				var parent = property.GetParentObject<object>();
				var path = property.propertyPath;

				var field = new DictionaryField(property, keys, values, itemDrawer)
				{
					Tooltip = tooltip,
					EmptyLabel = dictionaryAttribute.EmptyLabel,
					AllowAdd = dictionaryAttribute.AllowAdd != null,
					AllowRemove = dictionaryAttribute.AllowRemove != null,
					AllowReorder = dictionaryAttribute.AllowReorder != null
				};

				if (TryGetMethod(dictionaryAttribute.AllowAdd, _missingAllowAddMethodWarning, path, out var allowAddMethod))
					AddConditional(field.CanAdd, parent, allowAddMethod, _invalidAllowAddMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.AllowRemove, _missingAllowRemoveMethodWarning, path, out var allowRemoveMethod))
					AddConditional(field.CanRemove, parent, allowRemoveMethod, _invalidAllowRemoveMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.AllowReorder, _missingAllowReorderMethodWarning, path, out var allowReorderMethod))
					AddConditional(field.CanReorder, parent, allowReorderMethod, _invalidAllowReorderMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.AddCallback, _missingAddMethodWarning, path, out var addMethod))
					AddCallback(field.AddCallback, parent, addMethod, _invalidAddMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.RemoveCallback, _missingRemoveMethodWarning, path, out var removeMethod))
					AddCallback(field.RemoveCallback, parent, removeMethod, _invalidRemoveMethodWarning, path);

				if (TryGetMethod(dictionaryAttribute.ReorderCallback, _missingReorderMethodWarning, path, out var reorderMethod))
					AddCallback(field.ReorderCallback, parent, reorderMethod, _invalidReorderMethodWarning, path);

				return field;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName);
			}
		}

		private bool TryGetMethod(string name, string warning, string propertyPath, out MethodInfo method)
		{
			method = null;

			if (!string.IsNullOrEmpty(name))
			{
				method = fieldInfo.DeclaringType.GetMethod(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (method == null)
					Debug.LogWarningFormat(warning, propertyPath, name, fieldInfo.DeclaringType.Name);
			}

			return method != null;
		}

		private void AddCallback(Action<string> callback, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(null))
				callback += key => NoneCallback(method, owner);
			else if (method.HasSignature(null, typeof(string)))
				callback += key => OneCallback(key, method, owner);
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void AddConditional(Func<string, bool> callback, object parent, MethodInfo method, string warning, string propertyPath)
		{
			var owner = method.IsStatic ? null : parent;

			if (method.HasSignature(typeof(bool)))
				callback += key => NoneConditional(method, owner);
			else if (method.HasSignature(typeof(bool), typeof(string)))
				callback += key => OneConditional(key, method, owner);
			else
				Debug.LogWarningFormat(warning, propertyPath, method.Name);
		}

		private void NoneCallback(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}

		private void OneCallback(string key, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = key;
				method.Invoke(owner, _oneParameter);
			}
		}

		private bool NoneConditional(MethodInfo method, object owner)
		{
			return (bool)method.Invoke(owner, null);
		}

		private bool OneConditional(string key, MethodInfo method, object owner)
		{
			_oneParameter[0] = key;
			return (bool)method.Invoke(owner, _oneParameter);
		}
	}
}
