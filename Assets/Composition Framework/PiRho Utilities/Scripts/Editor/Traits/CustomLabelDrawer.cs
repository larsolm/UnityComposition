using PiRhoSoft.PargonUtilities.Engine;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(CustomLabelAttribute))]
	class CustomLabelDrawer : PropertyDrawer
	{
		private const string _invalidMethodWarning = "(PITCLIM) Invalid method for CustomLabel on '{0}': the method '{1}' could not be found";
		private const string _invalidReturnWarning = "(PITCLIR) Invalid method for CustomLabel on '{0}': the method '{1}' must return a string";
		private const string _invalidParametersWarning = "(PITCLIPS) Invalid method for CustomLabel on '{0}': the method '{1}' must be parameterless";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var text = GetText(property, attribute as CustomLabelAttribute);
			var container = ElementHelper.GetPropertyContainer(property, text, fieldInfo, attribute);
			var label = container.Query<Label>(className: PropertyField.labelUssClassName).First();

			if (label != null)
				label.text = text;

			return container;
		}

		private string GetText(SerializedProperty property, CustomLabelAttribute label)
		{
			if (!string.IsNullOrEmpty(label.Method))
			{
				var method = fieldInfo.DeclaringType.GetMethod(label.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					var obj = PropertyHelper.GetParent<Object>(property);

					if (method.ReturnType != typeof(string))
						Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, label.Method);
					else if (method.GetParameters().Length > 0)
						Debug.LogWarningFormat(_invalidParametersWarning, property.propertyPath, label.Method);
					else
						return (string)method.Invoke(obj, null);
				}
				else
				{
					Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, label.Method);
				}
			}

			return label.Label;
		}
	}
}
