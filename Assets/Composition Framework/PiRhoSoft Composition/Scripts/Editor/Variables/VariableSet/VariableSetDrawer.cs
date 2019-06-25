﻿using PiRhoSoft.CompositionEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.CompositionEditor
{
	[CustomPropertyDrawer(typeof(VariableSet))]
	public class VariableSetDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			return new VariableSetElement(property);
		}
	}
}
