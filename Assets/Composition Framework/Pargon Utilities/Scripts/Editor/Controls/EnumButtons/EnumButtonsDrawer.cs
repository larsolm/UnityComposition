﻿using PiRhoSoft.PargonUtilities.Engine;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PiRhoSoft.PargonUtilities.Editor
{
	[CustomPropertyDrawer(typeof(EnumButtonsAttribute))]
	class EnumButtonsDrawer : PropertyDrawer
	{
		private const string _invalidTypeWarning = "(PUEBDIT) Invalid type for EnumButtonsDrawer on field '{0}': EnumButtons can only be applied to Enum fields";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (property.propertyType == SerializedPropertyType.Enum)
			{
				var type = fieldInfo.FieldType;
				var flags = attribute is EnumButtonsAttribute enumButtons ? enumButtons.Flags : null;
				var tooltip = ElementHelper.GetTooltip(fieldInfo);
				var element = new EnumButtonsField(property, property.displayName, type, flags);

				return ElementHelper.SetupPropertyField(element, tooltip);
			}
			else
			{
				Debug.LogWarningFormat(_invalidTypeWarning, property.propertyPath);
				return ElementHelper.CreatePropertyContainer(property.displayName);
			}
		}
	}
}