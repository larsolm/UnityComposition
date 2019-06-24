﻿using PiRhoSoft.CompositionEngine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableValue))]
	public class VariableValueDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			container.Add(new ValueDefinitionElement(property));
			return container;
		}
	}
}
