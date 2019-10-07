﻿using PiRhoSoft.Utilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(GraphNode))]
	public class GraphNodeDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var field = new GraphNodeField(property.displayName, property.GetObject<GraphNode>());
			return field.ConfigureProperty(property, this.GetTooltip());
		}
	}
}