using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ListAttribute))]
	class ListDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PULDIT) invalid type for ListAttribute on field '{0}': List can only be applied to SerializedList or SerializedArray fields";
		private const string _missingAddMethodWarning = "(PULDMAM) invalid method for AddCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingRemoveMethodWarning = "(PULDMRM) invalid method for RemoveCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _missingReorderMethodWarning = "(PULDMROM) invalid method for ReorderCallback on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidAddMethodWarning = "(PULDIAM) invalid method for AddCallback on field '{0}': the method '{1}' should take no parameters";
		private const string _invalidRemoveMethodWarning = "(PULDIRM) invalid method for RemoveCallback on field '{0}': the method '{1}' should take an 0 or 1 int parameters";
		private const string _invalidReorderMethodWarning = "(PULDIROM) invalid method for ReorderCallback on field '{0}': the method '{1}' should take 0 or 2 int parameters";

		private static readonly object[] _oneParameter = new object[1];
		private static readonly object[] _twoParameters = new object[2];

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var items = property.FindPropertyRelative("_items");

			if (items != null && items.isArray)
			{
				var listAttribute = attribute as ListAttribute;
				var itemDrawer = this.GetNextDrawer();
				var tooltip = this.GetTooltip();

				var field = new ListField(items, itemDrawer)
				{
					Label = property.displayName,
					Tooltip = tooltip,
					EmptyLabel = listAttribute.EmptyLabel,
					AllowAdd = listAttribute.AllowAdd,
					AllowRemove = listAttribute.AllowRemove,
					AllowReorder = listAttribute.AllowReorder
				};

				if (!string.IsNullOrEmpty(listAttribute.AddCallback))
				{
					var method = fieldInfo.DeclaringType.GetMethod(listAttribute.AddCallback, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						if (method.HasSignature(null))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.AddCallback += () => NoneCallback(method, owner);
						}
						else
						{
							Debug.LogWarningFormat(_invalidAddMethodWarning, property.propertyPath, method.Name);
						}
					}
					else
					{
						Debug.LogWarningFormat(_missingAddMethodWarning, property.propertyPath, method.Name, fieldInfo.DeclaringType.Name);
					}
				}

				if (!string.IsNullOrEmpty(listAttribute.RemoveCallback))
				{
					var method = fieldInfo.DeclaringType.GetMethod(listAttribute.RemoveCallback, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						if (method.HasSignature(null))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.RemoveCallback += index => NoneCallback(method, owner);
						}
						else if (method.HasSignature(null, typeof(int)))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.RemoveCallback += index => OneCallback(index, method, owner);
						}
						else
						{
							Debug.LogWarningFormat(_invalidRemoveMethodWarning, property.propertyPath, method.Name);
						}
					}
					else
					{
						Debug.LogWarningFormat(_missingRemoveMethodWarning, property.propertyPath, method.Name, fieldInfo.DeclaringType.Name);
					}
				}

				if (!string.IsNullOrEmpty(listAttribute.ReorderCallback))
				{
					var method = fieldInfo.DeclaringType.GetMethod(listAttribute.ReorderCallback, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (method != null)
					{
						if (method.HasSignature(null))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.ReorderCallback += (from, to) => NoneCallback(method, owner);
						}
						else if (method.HasSignature(null, typeof(int)))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.ReorderCallback += (from, to) => OneCallback(to, method, owner);
						}
						else if (method.HasSignature(null, typeof(int), typeof(int)))
						{
							var owner = method.IsStatic ? null : property.GetParentObject<object>();
							field.ReorderCallback += (from, to) => TwoCallback(from, to, method, owner);
						}
						else
						{
							Debug.LogWarningFormat(_invalidReorderMethodWarning, property.propertyPath, method.Name);
						}
					}
					else
					{
						Debug.LogWarningFormat(_missingReorderMethodWarning, property.propertyPath, method.Name, fieldInfo.DeclaringType.Name);
					}
				}

				return field;
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return new FieldContainer(property.displayName, "");
			}
		}

		private void NoneCallback(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}

		private void OneCallback(int index, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_oneParameter[0] = index;
				method.Invoke(owner, _oneParameter);
			}
		}

		private void TwoCallback(int from, int to, MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
			{
				_twoParameters[0] = from;
				_twoParameters[1] = to;
				method.Invoke(owner, _twoParameters);
			}
		}
	}
}
