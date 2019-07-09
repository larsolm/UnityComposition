using PiRhoSoft.Utilities.Engine;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ValidateAttribute))]
	class ValidateDrawer : PropertyDrawer
	{
		private const string _invalidMethodWarning = "(PITVDIM) Invalid method for Validate on '{0}': the method '{1}' could not be found";
		private const string _invalidReturnWarning = "(PITVDIR) Invalid method for Validate on '{0}': the method '{1}' must return a bool";
		private const string _invalidParametersWarning = "(PITVDIPS) Invalid method for Validate on '{0}': the method '{1}' must be parameterless";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var validate = attribute as ValidateAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);
			var method = fieldInfo.DeclaringType.GetMethod(validate.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (method != null)
			{
				if (method.ReturnType != typeof(bool))
				{
					Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, validate.Method);
				}
				else if (method.GetParameters().Length > 0)
				{
					Debug.LogWarningFormat(_invalidParametersWarning, property.propertyPath, validate.Method);
				}
				else
				{
					var obj = PropertyHelper.GetParent<Object>(property);
					var column = new VisualElement();
					var message = new MessageBox(validate.Type, validate.Message);

					column.Add(container);
					column.Add(message);
					column.schedule.Execute(() =>
					{
						var valid = (bool)method.Invoke(obj, null);
						ElementHelper.SetVisible(message, !valid);
					}).Every(100);

					return column;
				}
			}
			else
			{
				Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, validate.Method);
			}

			return container;
		}
	}
}
