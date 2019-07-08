﻿using PiRhoSoft.Composition.Engine;
using PiRhoSoft.PargonUtilities.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace PiRhoSoft.Composition.Editor
{
	[CustomPropertyDrawer(typeof(GraphNode))]
	public class GraphNodeDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var node = property.objectReferenceValue as GraphNode;
			var container = ElementHelper.CreatePropertyContainer(property.displayName, ElementHelper.GetTooltip(fieldInfo));
			
			var label = new Label(node == null ? "Unconnected" : node.Name);
			label.style.flexGrow = new StyleFloat(1);
			label.style.flexShrink = new StyleFloat(1);
			label.style.overflow = new StyleEnum<Overflow>(Overflow.Hidden);

			var icon = ElementHelper.CreateIconButton(Icon.Inspect.Content, "Select and edit this node", () => GraphEditor.SelectNode(node));

			container.Add(label);
			container.Add(icon);

			return container;
		}
	}
}