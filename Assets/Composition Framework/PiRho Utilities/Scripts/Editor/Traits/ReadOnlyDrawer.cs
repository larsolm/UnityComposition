﻿using PiRhoSoft.Utilities.Engine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Utilities.Editor
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	class ReadOnlyDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.GetPropertyContainer(property, property.displayName, fieldInfo, attribute);
			container.SetEnabled(false);
			return container;
		}
	}
}
