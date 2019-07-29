﻿using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(InspectTriggerAttribute))]
	class InspectTriggerDrawer : PropertyDrawer
	{
		private const string _missingMethodWarning = "(PUITDMM) invalid method for InspectTriggerAttribute on field '{0}': the method '{1}' could not be found on type '{2}'";
		private const string _invalidMethodWarning = "(PUITDIM) invalid method for InspectTriggerAttribute on field '{0}': the method '{1}' should take no parameters";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var element = this.CreateNextElement(property);

			var change = attribute as InspectTriggerAttribute;
			var method = fieldInfo.DeclaringType.GetMethod(change.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (method.HasSignature(null))
				{
					var owner = method.IsStatic ? null : property.GetParentObject<object>();
					method.Invoke(owner, null);
				}
				else
				{
					Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, method.Name);
				}
			}
			else
			{
				Debug.LogWarningFormat(_missingMethodWarning, property.propertyPath, change.Method, fieldInfo.DeclaringType.Name);
			}

			return element;
		}

		private static void OnChanged(MethodInfo method, object owner)
		{
			if (!EditorApplication.isPlaying)
				method.Invoke(owner, null);
		}
	}
}