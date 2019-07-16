using PiRhoSoft.Utilities;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ChangeTriggerAttribute))]
	class ChangeTriggerDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITCTDIT) Invalid type for ChangeTrigger on '{0}': the drawn element doesn't trigger changes";
		private const string _invalidMethodWarning = "(PITCTDIM) Invalid method for ChangeTrigger on '{0}': the method '{1}' could not be found";
		private const string _invalidReturnWarning = "(PITCTDIR) Invalid method for ChangeTrigger on '{0}': the method '{1}' must return void";
		private const string _invalidParametersWarning = "(PITTCDIPS) Invalid method for ChangeTrigger on '{0}': the method '{1}' must be parameterless";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var change = attribute as ChangeTriggerAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);
			var method = fieldInfo.DeclaringType.GetMethod(change.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var obj = PropertyHelper.GetParent<Object>(property);

			if (method != null)
			{
				if (method.ReturnType != typeof(void))
					Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, change.Method);
				else if (method.GetParameters().Length > 0)
					Debug.LogWarningFormat(_invalidParametersWarning, property.propertyPath, change.Method);
				else if (!ElementHelper.RegisterChangeEvent(container, () => method.Invoke(method.IsStatic ? null : obj, null))) // TODO: this invokes before the actual property changes and thus doesn't work
					Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, change.Method);
			}

			return container;
		}
	}
}
