using PiRhoSoft.PargonUtilities.Engine;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(ConditionalAttribute))]
	class ConditionalDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PITCDIT) Invalid reference type for Conditional on '{0}': the type '{1}' must be an int, bool, float, string, enum, or Object";
		private const string _invalidPropertyWarning = "(PITCDIP) Invalid property for Conditional on '{0}': the referenced field '{1}' could not be found";
		private const string _invalidMethodWarning = "(PITCDIM) Invalid method for Conditional on '{0}': the method '{1}' could not be found";
		private const string _invalidReturnWarning = "(PITCDIR) Invalid method for Conditional on '{0}': the method '{1}' must return a bool";
		private const string _invalidParametersWarning = "(PITCDIPS) Invalid method for Conditional on '{0}': the method '{1}' must be parameterless";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var conditional = attribute as ConditionalAttribute;
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);

			if (!string.IsNullOrEmpty(conditional.Method))
			{
				var method = fieldInfo.DeclaringType.GetMethod(conditional.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (method != null)
				{
					if (method.ReturnType != typeof(bool))
						Debug.LogWarningFormat(_invalidReturnWarning, property.propertyPath, conditional.Method);
					else if (method.GetParameters().Length > 0)
						Debug.LogWarningFormat(_invalidParametersWarning, property.propertyPath, conditional.Method);
					else
						SetVisible(container, property, method);
				}
				else
				{
					Debug.LogWarningFormat(_invalidMethodWarning, property.propertyPath, conditional.Method);
				}
			}
			else if (!string.IsNullOrEmpty(conditional.Property))
			{
				var reference = PropertyHelper.GetSibling(property, conditional.Property);

				if (reference != null)
					SetVisible(container, reference, conditional);
				else
					Debug.LogWarningFormat(_invalidPropertyWarning, property.propertyPath, conditional.Property);
			}

			return container;
		}


		private void SetVisible(VisualElement container, SerializedProperty property, MethodInfo method)
		{
			var obj = PropertyHelper.GetParent<Object>(property);

			container.schedule.Execute(() =>
			{
				var visible = (bool)method.Invoke(obj, null);
				ElementHelper.SetVisible(container, visible);
			}).Every(100);
		}

		private void SetVisible(VisualElement container, SerializedProperty property, ConditionalAttribute conditional)
		{
			container.schedule.Execute(() =>
			{
				var visible = IsVisible(property, conditional);
				ElementHelper.SetVisible(container, visible);
			}).Every(100);
		}

		private bool IsVisible(SerializedProperty property, ConditionalAttribute conditional)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer: return conditional.Invert ? property.intValue != conditional.IntValue : property.intValue == conditional.IntValue;
				case SerializedPropertyType.Boolean: return conditional.Invert ? !property.boolValue : property.boolValue;
				case SerializedPropertyType.Float: return conditional.Invert ? property.floatValue != conditional.FloatValue : property.floatValue == conditional.FloatValue;
				case SerializedPropertyType.String: return conditional.Invert ? property.stringValue != conditional.StringValue : property.stringValue == conditional.StringValue;
				case SerializedPropertyType.ObjectReference: return conditional.Invert ? property.objectReferenceValue == null : property.objectReferenceValue != null;
				case SerializedPropertyType.Enum: return conditional.Invert ? property.intValue != conditional.IntValue : property.intValue == conditional.IntValue;
				default: Debug.LogWarningFormat(_invalidTypeWarning, fieldInfo.Name, property.propertyPath); break;
			}

			return true;
		}
	}
}

